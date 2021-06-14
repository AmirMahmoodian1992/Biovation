using Biovation.Brands.Shahab.Devices;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.Shahab.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class ShahabDeviceController : ControllerBase
    {
        private readonly ShahabServer _shahabServer;
        private readonly DeviceService _deviceService;
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly ILogger _logger;

        public ShahabDeviceController(DeviceService deviceService, Dictionary<uint, Device> onlineDevices, ShahabServer shahabServer, ILogger logger)
        {
            _shahabServer = shahabServer;
            _deviceService = deviceService;
            _onlineDevices = onlineDevices;
            _logger = logger.ForContext<ShahabDeviceController>();
        }

        [HttpGet]
        [AllowAnonymous]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            var onlineDevices = new List<DeviceBasicInfo>();

            lock (_onlineDevices)
            {
                foreach (var onlineDevice in _onlineDevices)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(onlineDevice.Value.GetDeviceInfo().Name))
                        {
                            onlineDevice.Value.GetDeviceInfo().Name = _deviceService.GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.MaxaCode)?.FirstOrDefault()?.Name;
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, exception.Message);
                    }

                    try
                    {
                        onlineDevices.Add(onlineDevice.Value.GetDeviceInfo());
                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, exception.Message);
                    }
                }
            }

            return onlineDevices;
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            var dbDevice = _deviceService.GetDevices(code: device.Code, brandId: DeviceBrands.ShahabCode)?.FirstOrDefault();
            if (dbDevice != null)
            {
                device.DeviceId = dbDevice.DeviceId;
                device.TimeSync = dbDevice.TimeSync;
                device.DeviceLockPassword = dbDevice.DeviceLockPassword;
            }

            return Task.Run(() =>
            {
                try
                {
                    if (device.Active)
                        _shahabServer.ConnectToDevice(device);
                    else
                        _shahabServer.DisconnectDevice(device);

                    return new ResultViewModel { Validate = 1, Message = "Unlocking Device queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }

        [HttpPost]
        [Authorize]
        public Dictionary<uint, bool> DeleteDevices([FromBody] List<uint> deviceIds)
        {
            var resultList = new Dictionary<uint, bool>();

            foreach (var deviceId in deviceIds)
            {
                var device = _deviceService.GetDevice(deviceId);
                lock (_onlineDevices)
                {
                    if (_onlineDevices.ContainsKey(device.Code))
                    {
                        _onlineDevices[device.Code].Disconnect();
                        if (_onlineDevices.ContainsKey(device.Code))
                            _onlineDevices.Remove(device.Code);
                    }
                }

                resultList.Add(deviceId, true);
            }

            return resultList;
        }
    }
}
