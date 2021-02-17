﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Biovation.Brands.PFK.Devices;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using RestSharp;

namespace Biovation.Brands.PFK
{
    public class PfkServer
    {
        private readonly Dictionary<uint, Camera> _onlineCameras;
        private readonly PFKParkingLibrary.Data.Logger _pfkLogger = new PFKParkingLibrary.Data.Logger();

        private readonly List<DeviceBasicInfo> _cameras;
        private readonly PlateDetectionService _plateDetectionService;

        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private readonly DeviceFactory _deviceFactory;
        private readonly MatchingTypes _matchingTypes;
        private readonly RestClient _logExternalSubmissionRestClient;
        private readonly Dictionary<int, Timer> _timers = new Dictionary<int, Timer>();



        public PfkServer(DeviceService deviceService, PlateDetectionService plateDetectionService, RestClient logExternalSubmissionRestClient, Dictionary<uint, Camera> onlineCameras, LogEvents logEvents, MatchingTypes matchingTypes, LogSubEvents logSubEvents, DeviceFactory deviceFactory)
        {
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _onlineCameras = onlineCameras;
            _matchingTypes = matchingTypes;
            _deviceFactory = deviceFactory;
            _plateDetectionService = plateDetectionService;
            _logExternalSubmissionRestClient = logExternalSubmissionRestClient;

            _cameras = deviceService.GetDevices(brandId: DeviceBrands.PfkCode).Where(x => x.Active).ToList();
            _pfkLogger.LogArose += OnLogHappened;
        }

        //public static Dictionary<uint, Device> GetOnlineDevices()
        //{
        //    lock (_onlineDevices)
        //    {
        //        return _onlineDevices;
        //    }
        //}

        public void StartServer()
        {
            Logger.Log("Service started.");
            //CheckExit();
            foreach (var device in _cameras)
            {
                ConnectToDevice(device);
                //CheckConnectionStatus(device);
            }
        }

        public async void ConnectToDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                try
                {
                    lock (_onlineCameras)
                    {
                        if (_onlineCameras.ContainsKey(deviceInfo.Code))
                        {
                            try
                            {
                                _onlineCameras[deviceInfo.Code].Disconnect();
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception);
                            }
                        }
                    }

                    if (!deviceInfo.Active) return;

                    var device = _deviceFactory.Factory(deviceInfo);
                    var connectResult = device.Connect();
                    if (!connectResult)
                        Logger.Log($"Cannot connect to device {deviceInfo.Code}.", logType: LogType.Warning);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }
            });
        }

        public async void DisconnectDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                lock (_onlineCameras)
                {
                    if (!_onlineCameras.ContainsKey(deviceInfo.Code)) return;
                    try
                    {
                        _onlineCameras[deviceInfo.Code].Disconnect();
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception);
                    }
                }
            });
        }


        public void OnLogHappened(object source, PFKParkingLibrary.Data.Log log)
        {
            Logger.Log($@"Log write by :   {source} 
    Log detail:
[ Log Id ] = {log.ID}
[ Log DateTime ] = {log.DateTime}
[ Log Comment ] = {log.Comment}
[ Log Item ] = {log.Item}
", string.Empty,
                log.Type switch
                {
                    PFKParkingLibrary.Data.LogType.Errore => LogType.Error,
                    PFKParkingLibrary.Data.LogType.Warning => CommonClasses.LogType.Warning,
                    _ => CommonClasses.LogType.Information
                });
        }
    }
}
