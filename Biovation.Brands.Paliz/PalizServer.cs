﻿using Biovation.Brands.Paliz.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using PalizTiara.Api;
using PalizTiara.Api.CallBacks;
using PalizTiara.Api.Models;
using PalizTiara.Protocol.Utility;
using RestSharp;
using System.Collections.Generic;

namespace Biovation.Brands.Paliz
{
    public class PalizServer
    {
        private readonly UserService _commonUserService;
        private readonly DeviceService _commonDeviceService;
        private readonly UserCardService _commonUserCardService;
        private readonly AccessGroupService _commonAccessGroupService;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly PalizCodeMappings _codeMappings;
        private readonly LogService _logService;
        private readonly LogEvents _logEvents;
        private readonly TaskStatuses _taskStatuses;
        private readonly DeviceBrands _deviceBrands;
        private readonly BlackListService _blackListService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly RestClient _monitoringRestClient;
        private readonly TaskService _taskService;
        private readonly AccessGroupService _accessGroupService;

        private BiovationConfigurationManager biovationConfiguration { get; }
        //private const string BiovationTopicName = "BiovationTaskStatusUpdateEvent";
        public static bool GetLogTaskFinished = true;

        private static Dictionary<uint, DeviceBasicInfo> _onlineDevices;
        internal readonly TiaraServerManager _serverManager;

        public PalizServer(TiaraServerManager serverManager, Dictionary<uint, DeviceBasicInfo> onlineDevices, UserService commonUserService
            , DeviceService commonDeviceService , UserCardService commonUserCardService, AccessGroupService commonAccessGroupService, FingerTemplateService fingerTemplateService
            , LogService logService, BlackListService blackListService, FaceTemplateService faceTemplateService, TaskService taskService
            , AccessGroupService accessGroupService, BiovationConfigurationManager biovationConfiguration
            , FingerTemplateTypes fingerTemplateTypes, PalizCodeMappings codeMappings, DeviceBrands deviceBrands
            , LogEvents logEvents, FaceTemplateTypes faceTemplateTypes, BiometricTemplateManager biometricTemplateManager
            , TaskStatuses taskStatuses)
        {
            _onlineDevices = onlineDevices;
            _commonDeviceService = commonDeviceService;
            _commonUserService = commonUserService;
            _commonUserCardService = commonUserCardService;
            _commonAccessGroupService = commonAccessGroupService;
            _fingerTemplateService = fingerTemplateService;
            _logService = logService;
            _blackListService = blackListService;
            _faceTemplateService = faceTemplateService;
            _taskService = taskService;
            _taskStatuses = taskStatuses;
            _accessGroupService = accessGroupService;
            this.biovationConfiguration = biovationConfiguration;
            //_virdiLogService = virdiLogService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _codeMappings = codeMappings;
            _onlineDevices = onlineDevices;

            _monitoringRestClient = (RestClient)new RestClient(biovationConfiguration.LogMonitoringApiUrl).UseSerializer(() => new RestRequestJsonSerializer());

            Trace.TraceLevel = TraceLevel.Error;
            Trace.TraceListener += Listen;

            _serverManager = serverManager;

            // initialize events
            _serverManager.LiveTrafficLogEvent += OnLiveTrafficLogEvent;
            _serverManager.DeviceLogEvent += ServerManagerOnDeviceLogEvent;
            _serverManager.DeviceInfoEvent += DeviceInfoEvent;
            _serverManager.TiaraSettingsEvent += TiaraSettingsEvent;

            //foreach (var device in _onlineDevices)
            //{
            //    if (device.IsOnline)
            //    {
            //        await this.serverManager.GetLiveTrafficLogAsyncTask(device.TerminalName);
            //    }
            //}
        }

        private void TiaraSettingsEvent(object sender, TiaraSettingsEventArgs args)
        {
            // pass
        }

        private void DeviceInfoEvent(object sender, DeviceInfoEventArgs args)
        {
            // pass
        }

        /// <summary>
        /// <En>Make or return the unique instance of Zk Server.</En>
        /// <Fa>یک نمونه واحد از سرور ساخته و باز میگرداند.</Fa>
        /// </summary>
        /// <returns></returns>
        public void StartServer()
        {
            Logger.Log("Service started.");

            _serverManager.Start();
        }

        public void StopServer()
        {
            _serverManager.Stop();
        }

        public Dictionary<uint, DeviceBasicInfo> GetOnlineDevices()
        {
            return _onlineDevices;
        }

        private void OnLiveTrafficLogEvent(object sender, LiveTrafficEventArgs args)
        {
            var device = (DeviceSender)sender;
            if (sender == null || args?.LiveTraffic == null)
            {
                return;
            }
            var log = args.LiveTraffic;
        }

        private void ServerManagerOnDeviceLogEvent(object sender, DeviceLogEventArgs args)
        {
            if (args?.DeviceLogModel?.Logs?.Length > 0)
            {
                // Do something
            }
        }

        private async void Listen(string format, params object[] args)
        {
            if(args.Length < 1)
            {
                return;
            }

            if (args.Length != 3)
            {
                return;
            }

            var terminalName = args[1].ToString();
            //var existedDevice = _commonDeviceService.GetDevices(deviceName: terminalName).Data.Data.FirstOrDefault();
            await _serverManager.GetTiaraSettingsAsyncTask(terminalName);

            //await Task.Run(async () =>
            //{

            //    var connectionStatus = new ConnectionStatus
            //    {
            //        DeviceId = existedDevice?.DeviceId ?? 0,
            //        IsConnected = true
            //    };

            //    try
            //    {
            //        var restRequest = new RestRequest("DeviceConnectionState/DeviceConnectionState", Method.POST);
            //        restRequest.AddQueryParameter("jsonInput", JsonConvert.SerializeObject(connectionStatus));

            //        await _monitoringRestClient.ExecuteAsync<ResultViewModel>(restRequest);
            //        //integration
            //        var connectionStatusList = new List<ConnectionStatus> { connectionStatus };
            //        var biovationBrokerMessageData = new List<DataChangeMessage<ConnectionStatus>>
            //            {
            //                new DataChangeMessage<ConnectionStatus>
            //                {
            //                    Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
            //                    TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = connectionStatusList
            //                }
            //            };

            //        //_deviceConnectionStateInternalSource.PushData(biovationBrokerMessageData);

            //        await _logService.AddLog(new Log
            //        {
            //            DeviceId = existedDevice?.DeviceId ?? 0,
            //            LogDateTime = DateTime.Now,
            //            EventLog = _logEvents.Connect
            //        });
            //    }
            //    catch (Exception)
            //    {
            //        //ignore
            //    }
            //}).ConfigureAwait(false);

            //DeviceBasicInfo device;
            //if (existedDevice != null)
            //{
            //    device = new DeviceBasicInfo
            //    {
            //        Code = (uint)terminalId,
            //        DeviceId = existDevice.DeviceId,
            //        Name = existDevice.Name,
            //        Brand = existDevice.Brand,
            //        Model = existDevice.Model,
            //        IpAddress = terminalIp,
            //        Port = BiovationConfiguration.VirdiDevicesConnectionPort,
            //        MacAddress = existDevice.MacAddress,
            //        RegisterDate = existDevice.RegisterDate,
            //        TimeSync = existDevice.TimeSync,
            //        Active = existDevice.Active,
            //        DeviceTypeId = existDevice.DeviceTypeId
            //    };

            //    if (existDevice.Code != (uint)terminalId || !string.Equals(existDevice.IpAddress, terminalIp, StringComparison.InvariantCultureIgnoreCase) || existDevice.Port != BiovationConfiguration.VirdiDevicesConnectionPort)
            //        _commonDeviceService.ModifyDevice(device);
            //}
            //else
            //{
            //    device = new DeviceBasicInfo
            //    {
            //        Code = (uint)terminalId,
            //        Brand = _deviceBrands.Virdi,
            //        Model = new DeviceModel { Id = 1001 },
            //        IpAddress = terminalIp,
            //        Port = BiovationConfiguration.VirdiDevicesConnectionPort,
            //        MacAddress = "",
            //        RegisterDate = DateTime.Now,
            //        TimeSync = true,
            //        Active = true
            //    };

            //    device.Name = terminalId + "[" + device.IpAddress + "]";
            //    var result = _commonDeviceService.ModifyDevice(device);
            //    if (result.Validate == 1)
            //        device.DeviceId = (int)result.Id;
            //}

            //if (!_onlineDevices.ContainsKey((string)terminalName))
            //    _onlineDevices.Add((string)terminalName, new DeviceBasicInfo());

            await _serverManager.GetLiveTrafficLogAsyncTask(terminalName);
        }
    }
}
