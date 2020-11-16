using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;

namespace Biovation.Server.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class TimeZoneController : ControllerBase
    {
        private readonly TimeZoneService _timeZoneService;
        private readonly DeviceService _deviceService;
        private readonly RestClient _restClient;
        private readonly string _kasraAdminToken;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public TimeZoneController(TimeZoneService timeZoneService, DeviceService deviceService, RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _deviceService = deviceService;
            _timeZoneService = timeZoneService;
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
            _kasraAdminToken = _biovationConfigurationManager.KasraAdminToken;
        }

        [HttpGet]
        [Route("TimeZones")]
        public List<TimeZone> TimeZones()
        {
            return _timeZoneService.GetTimeZones(token: _kasraAdminToken);
        }

        [HttpGet]
        [Route("TimeZone")]
        public TimeZone TimeZone(int id)
        {
            return _timeZoneService.TimeZones(id, token: _kasraAdminToken);
        }

        [HttpPost]
        [Route("DeleteTimeZone")]
        public ResultViewModel DeleteTimeZone(int id)
        {
            return _timeZoneService.DeleteTimeZone(id, token: _kasraAdminToken);
        }

        [HttpPost]
        [Route("ModifyTimeZone")]
        public ResultViewModel ModifyTimeZone([FromBody] TimeZone timeZone)
        {
            return _timeZoneService.ModifyTimeZone(timeZone, token: _kasraAdminToken);
        }

        [HttpPost]
        [Route("SendTimeZoneToAllDevices")]
        public List<ResultViewModel> SendTimeZoneToAllDevices(int timeZone)
        {
            var resultList = new List<ResultViewModel>();
            var deviceBrands = _deviceService.GetDeviceBrands(token: _kasraAdminToken);

            foreach (var deviceBrand in deviceBrands)
            {
                var restRequest =
                    new RestRequest(
                        $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}TimeZone/SendTimeZoneToAllDevices",
                        Method.POST);
                restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
                resultList.AddRange(_restClient.ExecuteAsync<List<ResultViewModel>>(restRequest).Result.Data);
            }

            return resultList;
        }

        [HttpPost]
        [Route("SendTimeZoneToDevice")]
        public ResultViewModel SendTimeZoneToDevice(int timeZoneId, int deviceId)
        {
            var device = _deviceService.GetDevice(deviceId, token: _kasraAdminToken);
            var restRequest =
                new RestRequest(
                    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}TimeZone/SendTimeZoneToDevice",
                    Method.GET);
            restRequest.AddParameter("code", device.Code);
            restRequest.AddParameter("timeZoneId", timeZoneId);
            restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
            _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
            return new ResultViewModel { Validate = 1 };
        }
    }
}
