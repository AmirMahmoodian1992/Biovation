﻿using System.Collections.Generic;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.API.v2;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Server.Controllers.v1
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class TimeZoneController : Controller
    {
        //private readonly CommunicationManager<List<ResultViewModel>> _communicationManager = new CommunicationManager<List<ResultViewModel>>();
        private readonly TimeZoneService _timeZoneService;
        private readonly DeviceService _deviceService;
        private readonly RestClient _restClient;

        public TimeZoneController(TimeZoneService timeZoneService, DeviceService deviceService, RestClient restClient)
        {
            _deviceService = deviceService;
            _timeZoneService = timeZoneService;
            _restClient = restClient;
            //_communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        }

        [HttpGet]
        [Route("TimeZones")]
        public List<TimeZone> TimeZones()
        {
            return _timeZoneService.GetTimeZones().Data;
        }

        [HttpGet]
        [Route("TimeZone")]
        public TimeZone TimeZone(int id)
        {
            return _timeZoneService.TimeZones(id).Data;
        }

        [HttpPost]
        [Route("DeleteTimeZone")]
        public ResultViewModel DeleteTimeZone(int id)
        {
            return _timeZoneService.DeleteTimeZone(id);
        }

        [HttpPost]
        [Route("ModifyTimeZone")]
        public ResultViewModel ModifyTimeZone(TimeZone timeZone)
        {
            return _timeZoneService.ModifyTimeZone(timeZone);
        }

        [HttpPost]
        [Route("SendTimeZoneToAllDevices")]
        public List<ResultViewModel> SendTimeZoneToAllDevices(int timeZone)
        {
            var resultList = new List<ResultViewModel>();
            var deviceBrands = _deviceService.GetDeviceBrands().Data;

            foreach (var deviceBrand in deviceBrands)
            {
                //resultList.AddRange(_communicationManager.CallRest($"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}TimeZone/SendTimeZoneToAllDevices", "Post", null,
                //                                                    $"{JsonConvert.SerializeObject(timeZone)}"));
                var restRequest =
                    new RestRequest(
                        $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}TimeZone/SendTimeZoneToAllDevices",
                        Method.POST);
                resultList.AddRange(_restClient.ExecuteAsync<List<ResultViewModel>>(restRequest).Result.Data);
            }

            return resultList;
        }

        [HttpPost]
        [Route("SendTimeZoneToDevice")]
        public ResultViewModel SendTimeZoneToDevice(int timeZoneId, int deviceId)
        {
            var device = _deviceService.GetDevice(deviceId).Data;
            //var parameters = new List<object> { $"code={device.Code}", $"timeZoneId={timeZoneId}" };
            //_communicationManager.CallRest(
            //    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}TimeZone/SendTimeZoneToDevice", "Get", parameters);
            var restRequest =
                new RestRequest(
                    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}TimeZone/SendTimeZoneToDevice",
                    Method.GET);
            restRequest.AddParameter("code", device.Code);
            restRequest.AddParameter("timeZoneId", timeZoneId);
            _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
            return new ResultViewModel { Validate = 1 };
        }
    }
}
