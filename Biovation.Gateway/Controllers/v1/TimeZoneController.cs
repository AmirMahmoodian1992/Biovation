using System.Collections.Generic;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Biovation.Gateway.Controllers.v1
{
    [Route("[controller]")]
    [ApiController]
    public class TimeZoneController : ControllerBase
    {
        //private readonly CommunicationManager<List<ResultViewModel>> _communicationManager = new CommunicationManager<List<ResultViewModel>>();
        private readonly TimeZoneService _timeZoneService = new TimeZoneService();
        private readonly DeviceService _deviceService = new DeviceService();
        private readonly RestClient _restServer;

        public TimeZoneController()
        {
            _restServer =
                (RestClient)new RestClient($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
            //_communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        }

        [HttpGet]
        [Route("TimeZones")]
        public List<TimeZone> TimeZones()
        {
            return _timeZoneService.GetAllTimeZones();
        }

        [HttpGet]
        [Route("TimeZone")]
        public TimeZone TimeZone(int id)
        {
            return _timeZoneService.GetTimeZoneById(id);
        }

        [HttpPost]
        [Route("DeleteTimeZone")]
        public ResultViewModel DeleteTimeZone(int id)
        {
            return _timeZoneService.DeleteTimeZoneById(id);
        }

        [HttpPost]
        [Route("ModifyTimeZone")]
        public ResultViewModel ModifyTimeZone(TimeZone timeZone)
        {
            return _timeZoneService.ModifyTimeZoneById(timeZone);
        }

        [HttpPost]
        [Route("SendTimeZoneToAllDevices")]
        public List<ResultViewModel> SendTimeZoneToAllDevices(int timeZone)
        {
            var resultList = new List<ResultViewModel>();
            var deviceBrands = _deviceService.GetDeviceBrands();

            foreach (var deviceBrand in deviceBrands)
            {
                //resultList.AddRange(_communicationManager.CallRest($"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}TimeZone/SendTimeZoneToAllDevices", "Post", null,
                //                                                    $"{JsonConvert.SerializeObject(timeZone)}"));
                var restRequest =
                    new RestRequest(
                        $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}TimeZone/SendTimeZoneToAllDevices",
                        Method.POST);
                resultList.AddRange(_restServer.ExecuteAsync<List<ResultViewModel>>(restRequest).Result.Data);
            }

            return resultList;
        }

        [HttpPost]
        [Route("SendTimeZoneToDevice")]
        public ResultViewModel SendTimeZoneToDevice(int timeZoneId, int deviceId)
        {
            var device = _deviceService.GetDeviceInfo(deviceId);
            //var parameters = new List<object> { $"code={device.Code}", $"timeZoneId={timeZoneId}" };
            //_communicationManager.CallRest(
            //    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}TimeZone/SendTimeZoneToDevice", "Get", parameters);
            var restRequest =
                new RestRequest(
                    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}TimeZone/SendTimeZoneToDevice",
                    Method.GET);
            restRequest.AddParameter("code", device.Code);
            restRequest.AddParameter("timeZoneId", timeZoneId);
            _restServer.ExecuteAsync<List<ResultViewModel>>(restRequest);
            return new ResultViewModel { Validate = 1 };
        }
    }
}
