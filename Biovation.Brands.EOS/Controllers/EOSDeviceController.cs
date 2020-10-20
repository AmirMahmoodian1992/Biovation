using System.Collections.Generic;
using System.Linq;
using Biovation.Brands.EOS.Devices;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Brands.EOS.Controllers
{
    public class EosDeviceController : Controller
    {
        private readonly EosServer _eosServer;
        private readonly DeviceService _deviceService;
        private readonly Dictionary<uint, Device> _onlineDevices;


        public EosDeviceController(DeviceService deviceService, Dictionary<uint, Device> onlineDevices, EosServer eosServer)
        {
            _eosServer = eosServer;
            _deviceService = deviceService;
            _onlineDevices = onlineDevices;
        }

        [HttpGet]
        [AllowAnonymous]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            var onlineDevices = new List<DeviceBasicInfo>();

            foreach (var onlineDevice in _onlineDevices)
            {
                if (string.IsNullOrEmpty(onlineDevice.Value.GetDeviceInfo().Name))
                {
                    onlineDevice.Value.GetDeviceInfo().Name = _deviceService.GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.EosCode)?.FirstOrDefault()?.Name;
                }
                onlineDevices.Add(onlineDevice.Value.GetDeviceInfo());
            }

            return onlineDevices;
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            if (device.Active)
            {
                _eosServer.ConnectToDevice(device);
            }

            else
            {
                _eosServer.DisconnectFromDevice(device);
            }

            return new ResultViewModel { Validate = 0, Id = device.DeviceId };
        }
    }
}
