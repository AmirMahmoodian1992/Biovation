using Biovation.Brands.PW.Devices;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.Brands.PW
{
    public class PwServer
    {
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly List<DeviceBasicInfo> _pwDevices;
        private readonly LogService _logService;

        protected CancellationToken CancellationToken;

        private readonly LogEvents _logEvents;
        private readonly DeviceFactory _deviceFactory;

        public PwServer(DeviceService commonDeviceService, LogService commonLogService, Dictionary<uint, Device> onlineDevices, LogEvents logEvents, DeviceFactory deviceFactory)
        {
            _logService = commonLogService;
            _onlineDevices = onlineDevices;
            _logEvents = logEvents;
            _deviceFactory = deviceFactory;
            _pwDevices = commonDeviceService.GetDevices(brandId: DeviceBrands.ProcessingWorldCode)?.Where(x => x.Active).ToList();
        }

        public void StartServer(CancellationToken cancellationToken)
        {
            Logger.Log("Service started.");
            CancellationToken = cancellationToken;

            Task.Run(() =>
            {
                Parallel.ForEach(_pwDevices, ConnectToDevice);
            }, CancellationToken);
        }

        public async void ConnectToDevice(DeviceBasicInfo deviceInfo)
        {
            await Task.Run(() =>
            {
                lock (_onlineDevices)
                {
                    if (_onlineDevices.ContainsKey(deviceInfo.Code))
                    {
                        _onlineDevices[deviceInfo.Code].Disconnect();
                        _onlineDevices.Remove(deviceInfo.Code);

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
                    }
                }

                if (!deviceInfo.Active) return;

                var device = _deviceFactory.Factory(deviceInfo);
                var connectResult = device.Connect(CancellationToken);
                if (!connectResult) return;

                lock (_onlineDevices)
                {
                    if (!_onlineDevices.ContainsKey(deviceInfo.Code))
                    {
                        _onlineDevices.Add(deviceInfo.Code, device);
                    }
                }

                try
                {
                    _ = _logService.AddLog(new Log
                    {
                        DeviceId = deviceInfo.DeviceId,
                        LogDateTime = DateTime.Now,
                        EventLog = _logEvents.Connect
                    }).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //ignore
                }

            }, CancellationToken);
        }
        public async void DisconnectFromDevice(DeviceBasicInfo deviceInfo)
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
            }, CancellationToken);
        }

        public void StopServer()
        {
            lock (_onlineDevices)
            {
                Parallel.ForEach(_onlineDevices, onlineDevice =>
                {
                    onlineDevice.Value.Disconnect();
                });
            }
        }
    }
}
