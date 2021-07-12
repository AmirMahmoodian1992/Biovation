using Biovation.Domain;
using Biovation.Service.Api.v2.RelayController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

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
        private readonly CameraService _cameraService;

        public PfkDeviceController(CameraService cameraService, PfkServer pfkServer)
        {
            _pfkServer = pfkServer;
            _cameraService = cameraService;
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
                    onlineDevice.Value.GetCameraInfo().Name = _cameraService.GetCamera(code: onlineDevice.Key).Result?.Data?.Data?.FirstOrDefault()?.Name;
                }

                var cameraInfo = onlineDevice.Value.GetCameraInfo();
                onlineDevices.Add(new DeviceBasicInfo { DeviceId = (int)cameraInfo.Id });
            }

            return onlineDevices;
        }

        [HttpGet]
        [AllowAnonymous]
        public List<Camera> GetOnlineCameras()
        {
            var onlineDevices = new List<Camera>();

            foreach (var onlineDevice in _pfkServer.GetOnlineDevices())
            {
                if (string.IsNullOrEmpty(onlineDevice.Value.GetCameraInfo().Name))
                {
                    onlineDevice.Value.GetCameraInfo().Name = _cameraService.GetCamera(code: onlineDevice.Key).Result?.Data?.Data?.FirstOrDefault()?.Name;
                }

                onlineDevices.Add(onlineDevice.Value.GetCameraInfo());
            }

            return onlineDevices;
        }

        [HttpPost]
        public async Task<ResultViewModel> ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            var camera = (await _cameraService.GetCamera(code: device.Code))?.Data?.Data?.FirstOrDefault();

            return await Task.Run(() =>
            {
                try
                {
                    if (device.Active && !_pfkServer.GetOnlineDevices().ContainsKey(device.Code))
                        _pfkServer.ConnectToDevice(camera);
                    else if (!device.Active && _pfkServer.GetOnlineDevices().ContainsKey(device.Code))
                        _pfkServer.DisconnectDevice(camera);

                    return new ResultViewModel { Validate = 1, Message = "Unlocking Device queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }

        [HttpPost]
        public async Task<ResultViewModel> ModifyCamera([FromBody] Camera modifiedCamera)
        {
            //var camera = (await _cameraService.GetCamera(id: modifiedCamera.Id))?.Data?.Data?.FirstOrDefault();

            return await Task.Run(() =>
            {
                try
                {
                    if (modifiedCamera.Active && !_pfkServer.GetOnlineDevices().ContainsKey(modifiedCamera.Code))
                        _pfkServer.ConnectToDevice(modifiedCamera);
                    else if (!modifiedCamera.Active && _pfkServer.GetOnlineDevices().ContainsKey(modifiedCamera.Code))
                        _pfkServer.DisconnectDevice(modifiedCamera);

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