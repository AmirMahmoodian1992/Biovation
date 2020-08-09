using System.Collections.Generic;
using System.Web.Http;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Newtonsoft.Json;

namespace Biovation.WebService.APIControllers
{
    public class TimeZoneController : ApiController
    {
        private readonly CommunicationManager<List<ResultViewModel>> _communicationManager = new CommunicationManager<List<ResultViewModel>>();
        private readonly TimeZoneService _timeZoneService = new TimeZoneService();
        private readonly DeviceService _deviceService = new DeviceService();

        public TimeZoneController()
        {
            _communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        }

        [HttpGet]
        public List<TimeZone> TimeZones()
        {
            return _timeZoneService.GetAllTimeZones();
        }

        [HttpGet]
        public TimeZone TimeZone(int id)
        {
            return _timeZoneService.GetTimeZoneById(id);
        }

        [HttpPost]
        public ResultViewModel DeleteTimeZone(int id)
        {
            return _timeZoneService.DeleteTimeZoneById(id);
        }

        [HttpPost]
        public ResultViewModel ModifyTimeZone(TimeZone timeZone)
        {
            return _timeZoneService.ModifyTimeZoneById(timeZone);
        }

        [HttpPost]
        public List<ResultViewModel> SendTimeZoneToAllDevices(int timeZone)
        {
            var resultList = new List<ResultViewModel>();
            var deviceBrands = _deviceService.GetDeviceBrands();

            foreach (var deviceBrand in deviceBrands)
            {
                resultList.AddRange(_communicationManager.CallRest($"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}TimeZone/SendTimeZoneToAllDevices", "Post", null,
                                                                    $"{JsonConvert.SerializeObject(timeZone)}"));
            }

            return resultList;
        }

        [HttpPost]
        public ResultViewModel SendTimeZoneToDevice(int timeZoneId, int deviceId)
        {
            var device = _deviceService.GetDeviceInfo(deviceId);
            var parameters = new List<object> { $"code={device.Code}", $"timeZoneId={timeZoneId}"};
            _communicationManager.CallRest(
                $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}TimeZone/SendTimeZoneToDevice", "Get", parameters);
            return new ResultViewModel { Validate = 1 };
        }
    }
}
