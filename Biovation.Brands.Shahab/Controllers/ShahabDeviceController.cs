using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Brands.Shahab.Devices;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Brands.Shahab.Controllers
{
    public class ShahabDeviceController : Controller
    {
        private readonly ShahabServer _shahabServer;
        private readonly DeviceService _deviceService;
        private readonly Dictionary<uint, Device> _onlineDevices;

        public ShahabDeviceController(DeviceService deviceService, Dictionary<uint, Device> onlineDevices, ShahabServer shahabServer)
        {
            _shahabServer = shahabServer;
            _deviceService = deviceService;
            _onlineDevices = onlineDevices;
        }

        [HttpGet]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            var onlineDevices = new List<DeviceBasicInfo>();

            foreach (var onlineDevice in _onlineDevices)
            {
                if (string.IsNullOrEmpty(onlineDevice.Value.GetDeviceInfo().Name))
                {
                    onlineDevice.Value.GetDeviceInfo().Name = _deviceService.GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.MaxaCode)?.FirstOrDefault()?.Name;
                }
                onlineDevices.Add(onlineDevice.Value.GetDeviceInfo());
            }

            return onlineDevices;
        }

        [HttpPost]
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
    }
}
