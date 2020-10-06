﻿using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Constants;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.ZK
{
    public class ZkTecoServer
    {
        /// <summary>
        /// نمونه ی ساخته شده از سرور
        /// </summary>
        private readonly Dictionary<uint, Device> _onlineDevices;

        private readonly List<DeviceBasicInfo> _zkDevices;
        /// 

        /// <summary>
        /// <En>Make or return the unique instance of Zk Server.</En>
        /// <Fa>یک نمونه واحد از سرور ساخته و باز میگرداند.</Fa>
        /// </summary>
        /// <returns></returns>

        private ZkTecoServer(Dictionary<uint, Device> onlineDevices, DeviceService deviceService)
        {
            _onlineDevices = onlineDevices;
            _zkDevices = deviceService.GetDevices(brandId:int.Parse(DeviceBrands.ZkTecoCode)).Where(x => x.Active).ToList();
        }

        public void StartServer()
        {
            Logger.Log("Service started.");

            foreach (var zkDevice in _zkDevices)
            {
                ConnectToDevice(zkDevice);
            }
        }

        public async void ConnectToDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                lock (_onlineDevices)
                {
                    if (_onlineDevices.ContainsKey(deviceInfo.Code))
                    {
                        try
                        {
                            _onlineDevices[deviceInfo.Code].Disconnect();
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception);
                        }
                    }
                }

                if (!deviceInfo.Active) return;

                var device = DeviceFactory.Factory(deviceInfo);
                var connectResult = device.Connect();
                if (!connectResult)
                    Logger.Log($"Cannot connect to device {deviceInfo.Code}.", logType: LogType.Warning);
            });
        }
        public async void DisconnectFromDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                lock (_onlineDevices)
                {
                    if (!_onlineDevices.ContainsKey(deviceInfo.Code)) return;
                    _onlineDevices[deviceInfo.Code].Disconnect();
                }

                //lock (OnlineDevices)
                //    OnlineDevices.Remove(deviceInfo.Code);
            });
        }

        public void StopServer()
        {
            lock (_onlineDevices)
            {
                foreach (var onlineDevice in _onlineDevices)
                {
                    onlineDevice.Value.Disconnect();
                }
            }
        }


    }
}
