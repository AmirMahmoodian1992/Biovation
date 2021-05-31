using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

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
        private readonly RestClient _restClient;

        private CancellationToken _cancellationToken;
        /// <summary>
        /// <En>Make or return the unique instance of Zk Server.</En>
        /// <Fa>یک نمونه واحد از سرور ساخته و باز میگرداند.</Fa>
        /// </summary>
        /// <returns></returns>

        public ZkTecoServer(Dictionary<uint, Device> onlineDevices, DeviceService deviceService, DeviceFactory deviceFactory, RestClient restClient,SystemInfo systemInfo)
        {
            _onlineDevices = onlineDevices;
            _deviceFactory = deviceFactory;
            _restClient = restClient;
            _zkDevices = deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).Where(x => x.Active && x.ServiceInstance.Id == systemInfo.Services.FirstOrDefault()?.Id ).ToList();
        }

        /// <summary>
        /// <En>Make or return the unique instance of Zk Server.</En>
        /// <Fa>یک نمونه واحد از سرور ساخته و باز میگرداند.</Fa>
        /// </summary>
        /// <returns></returns>
        public Task StartServer(CancellationToken cancellationToken)
        { Logger.Log("Service started.");
            _cancellationToken = cancellationToken;
            var connectToDeviceTasks = new List<Task>();
            Parallel.ForEach(_zkDevices, device => connectToDeviceTasks.Add(ConnectToDevice(device, cancellationToken)));
            //var connectToDeviceTasks = _zkDevices.Select(ConnectToDevice).ToList();
            if (connectToDeviceTasks.Count == 0)
                return Task.CompletedTask;

            return Task.WhenAny(connectToDeviceTasks);
        }

        public async Task ConnectToDevice(DeviceBasicInfo deviceInfo, CancellationToken cancellationToken = default)
        {
            if (cancellationToken == default)
                cancellationToken = _cancellationToken;

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

            while (!connectResult && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    connectResult = await device.Connect(cancellationToken);
                    if (!connectResult)
                        Logger.Log($"Cannot connect to device {deviceInfo.Code}.", logType: LogType.Warning);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, $"Exception on connection to device {deviceInfo.Code}", LogType.Fatal);
                }
            }
        }

        public async Task DisconnectFromDevice(DeviceBasicInfo deviceInfo, CancellationToken cancellationToken = default)
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
            }, cancellationToken);
        }

        public async Task StopServer(CancellationToken cancellationToken)
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
            }, cancellationToken);
        }
    }
}
