using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.ZK
{
    public class ZkTecoServer
    {
        /// <summary>
        /// نمونه ی ساخته شده از سرور
        /// </summary>
        private readonly Dictionary<uint, Device> _onlineDevices;

        private readonly DeviceFactory _deviceFactory;
        private readonly List<DeviceBasicInfo> _zkDevices;
        /// 

        /// <summary>
        /// <En>Make or return the unique instance of Zk Server.</En>
        /// <Fa>یک نمونه واحد از سرور ساخته و باز میگرداند.</Fa>
        /// </summary>
        /// <returns></returns>

        public ZkTecoServer(Dictionary<uint, Device> onlineDevices, DeviceService deviceService, DeviceFactory deviceFactory)
        {
            _onlineDevices = onlineDevices;
            _deviceFactory = deviceFactory;
            _zkDevices = deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).Where(x => x.Active).ToList();
        }

        /// <summary>
        /// <En>Make or return the unique instance of Zk Server.</En>
        /// <Fa>یک نمونه واحد از سرور ساخته و باز میگرداند.</Fa>
        /// </summary>
        /// <returns></returns>
        public void StartServer()
        {
            Logger.Log("Service started.");
            Parallel.ForEach(_zkDevices, ConnectToDevice);
            //var connectToDeviceTasks = _zkDevices.Select(ConnectToDevice).ToList();
            //if (connectToDeviceTasks.Count == 0)
            //    return;

            //await Task.WhenAny(connectToDeviceTasks).Result;
        }

        public void ConnectToDevice(DeviceBasicInfo deviceInfo)
        {
            Task.Run(() =>
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

                var device = _deviceFactory.Factory(deviceInfo);

                var connectResult = false;

                while (!connectResult)
                {
                    try
                    {
                        connectResult = device.Connect();
                        if (!connectResult)
                            Logger.Log($"Cannot connect to device {deviceInfo.Code}.", logType: LogType.Warning);
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception, $"Exception on connection to device {deviceInfo.Code}", LogType.Fatal);
                    }
                }
            });
        }

        public async Task DisconnectFromDevice(DeviceBasicInfo deviceInfo)
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

        public async Task StopServer()
        {
            await Task.Run(() =>
            {
                Dictionary<uint, Device> onlineDevices;
                lock (_onlineDevices)
                    onlineDevices = new Dictionary<uint, Device>(_onlineDevices);

                Parallel.ForEach(onlineDevices, onlineDevice =>
                {
                    try
                    {
                        onlineDevice.Value.Disconnect();
                    }
                    catch
                    {
                        // ignored
                    }
                });
            });
        }
    }
}
