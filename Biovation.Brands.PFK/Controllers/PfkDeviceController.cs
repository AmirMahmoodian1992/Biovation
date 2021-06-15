using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;

namespace Biovation.Brands.PFK.Controllers
{
#if NETCORE31
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class PfkDeviceController : ControllerBase
#elif NET472
    public class PfkDeviceController : ApiController
#endif    
    {
        private readonly PfkServer _pfkServer;
        private readonly DeviceService _deviceService;

        public PfkDeviceController(DeviceService deviceService, PfkServer pfkServer)
        {
            _pfkServer = pfkServer;
            _deviceService = deviceService;
        }

        [HttpGet]
        [AllowAnonymous]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            var onlineDevices = new List<DeviceBasicInfo>();

            foreach (var onlineDevice in _pfkServer.GetOnlineDevices())
            {
                if (string.IsNullOrEmpty(onlineDevice.Value.GetCameraInfo().Name))
                {
                    onlineDevice.Value.GetCameraInfo().Name = _deviceService.GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.ShahabCode).Result?.Data?.Data?.FirstOrDefault()?.Name;
                }
                onlineDevices.Add(onlineDevice.Value.GetCameraInfo());
            }

            return onlineDevices;
        }

        [HttpPost]
        public async Task<ResultViewModel> ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            var dbDevice = (await _deviceService.GetDevices(code: device.Code, brandId: DeviceBrands.ShahabCode))?.Data?.Data?.FirstOrDefault();
            device.DeviceId = dbDevice.DeviceId;
            device.TimeSync = dbDevice.TimeSync;
            device.DeviceLockPassword = dbDevice.DeviceLockPassword;

            return await Task.Run(() =>
            {
                try
                {
                    if (device.Active)
                        _pfkServer.ConnectToDevice(device);
                    else
                        _pfkServer.DisconnectDevice(device);

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