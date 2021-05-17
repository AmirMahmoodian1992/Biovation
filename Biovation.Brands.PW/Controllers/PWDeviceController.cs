using Biovation.Brands.PW.Devices;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Serilog;

namespace Biovation.Brands.PW.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class PwDeviceController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly PwServer _pwServer;
        private readonly DeviceService _deviceService;
        private readonly Dictionary<uint, Device> _onlineDevices;

        public PwDeviceController(DeviceService deviceService, Dictionary<uint, Device> onlineDevices, PwServer pwServer, ILogger logger)
        {
            _deviceService = deviceService;
            _onlineDevices = onlineDevices;
            _pwServer = pwServer;
            _logger = logger.ForContext<PwDeviceController>();
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
                            onlineDevice.Value.GetDeviceInfo().Name = _deviceService.GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.ProcessingWorldCode)?.FirstOrDefault()?.Name;
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
        public async Task<ResultViewModel> ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            return await Task.Run(() =>
            {
                if (device.Active)
                    _pwServer.ConnectToDevice(device);

                else
                    _pwServer.DisconnectFromDevice(device);

                return new ResultViewModel { Validate = 0, Id = device.DeviceId };
            });
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel> ReadOfflineOfDevice(uint code, DateTime? fromDate, DateTime? toDate)
        {
            return Task.Run(() =>
            {
                var result = new ResultViewModel {Validate = 1, Message = $"Reading logs of device {code} queued"};
                return result;
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
