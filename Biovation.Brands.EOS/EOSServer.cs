using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.Brands.EOS
{
    public class EosServer
    {
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly List<DeviceBasicInfo> _eosDevices;

        private readonly LogEvents _logEvents;
        private readonly LogService _logService;
        private readonly DeviceFactory _deviceFactory;

        public int Count = 0;

        public EosServer(LogService logService, DeviceService deviceService, Dictionary<uint, Device> onlineDevices, LogEvents logEvents, DeviceFactory deviceFactory)
        {
            _logService = logService;
            _onlineDevices = onlineDevices;
            _logEvents = logEvents;
            _deviceFactory = deviceFactory;
            _eosDevices = deviceService.GetDevices(brandId: DeviceBrands.EosCode).Result?.Data?.Data?.Where(x => x.Active).ToList() ?? new List<DeviceBasicInfo>();
        }

        public async void ConnectToDevice(DeviceBasicInfo deviceInfo, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                lock (_onlineDevices)
                {
                    if (_onlineDevices.ContainsKey(deviceInfo.Code))
                    {
                        _onlineDevices[deviceInfo.Code].Disconnect();
                        _onlineDevices.Remove(deviceInfo.Code);
                    }
                }

                if (!deviceInfo.Active) return;

                var device = _deviceFactory.Factory(deviceInfo);
                var connectResult = device.Connect(cancellationToken);
                if (!connectResult) return;

                lock (_onlineDevices)
                {
                    if (!_onlineDevices.ContainsKey(deviceInfo.Code))
                    {
                        _onlineDevices.Add(deviceInfo.Code, device);
                    }
                }
            }, cancellationToken);
        }

        public async void DisconnectFromDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                if (_onlineDevices.ContainsKey(deviceInfo.Code))
                {
                    _onlineDevices[deviceInfo.Code].Disconnect();
                    _onlineDevices.Remove(deviceInfo.Code);
                }
                try
                {
                    _ = _logService.AddLog(new Log
                    {
                        DeviceId = deviceInfo.DeviceId,
                        LogDateTime = DateTime.Now,
                        EventLog = _logEvents.Disconnect
                    }).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //ignore
                }
            });
        }

        public void StopServer(CancellationToken cancellationToken)
        {
            Parallel.ForEach(_onlineDevices, onlineDevice =>
            {
                onlineDevice.Value.Disconnect();
            });
        }

        public void StartServer(CancellationToken cancellationToken)
        {
            Logger.Log("EOS Server Started!");
            Parallel.ForEach(_eosDevices, eosDevice =>
            {
                ConnectToDevice(eosDevice, cancellationToken);
            });
        }
    }
}
