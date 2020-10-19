﻿using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;

namespace Biovation.Server.Controllers.v1
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class TimeZoneController : Controller
    {
        private readonly TimeZoneService _timeZoneService;
        private readonly DeviceService _deviceService;
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public TimeZoneController(TimeZoneService timeZoneService, DeviceService deviceService, RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _deviceService = deviceService;
            _timeZoneService = timeZoneService;
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        [HttpGet]
        [Route("TimeZones")]
        public List<TimeZone> TimeZones()
        {
            return _timeZoneService.GetTimeZones();
        }

        [HttpGet]
        [Route("TimeZone")]
        public TimeZone TimeZone(int id)
        {
            return _timeZoneService.TimeZones(id);
        }

        [HttpPost]
        [Route("DeleteTimeZone")]
        public ResultViewModel DeleteTimeZone(int id)
        {
            return _timeZoneService.DeleteTimeZone(id);
        }

        [HttpPost]
        [Route("ModifyTimeZone")]
        public ResultViewModel ModifyTimeZone([FromBody] TimeZone timeZone)
        {
            return _timeZoneService.ModifyTimeZone(timeZone);
        }

        [HttpPost]
        [Route("SendTimeZoneToAllDevices")]
        public List<ResultViewModel> SendTimeZoneToAllDevices(int timeZone)
        {
            var resultList = new List<ResultViewModel>();
            var deviceBrands = _deviceService.GetDeviceBrands();

            foreach (var deviceBrand in deviceBrands)
            {
                var restRequest =
                    new RestRequest(
                        $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}TimeZone/SendTimeZoneToAllDevices",
                        Method.POST);
                restRequest.AddHeader("Authorization", _biovationConfigurationManager.SecondDefaultToken);
                resultList.AddRange(_restClient.ExecuteAsync<List<ResultViewModel>>(restRequest).Result.Data);
            }

            return resultList;
        }

        [HttpPost]
        [Route("SendTimeZoneToDevice")]
        public ResultViewModel SendTimeZoneToDevice(int timeZoneId, int deviceId)
        {
            var device = _deviceService.GetDevice(deviceId);
            var restRequest =
                new RestRequest(
                    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}TimeZone/SendTimeZoneToDevice",
                    Method.GET);
            restRequest.AddParameter("code", device.Code);
            restRequest.AddParameter("timeZoneId", timeZoneId);
            restRequest.AddHeader("Authorization", _biovationConfigurationManager.SecondDefaultToken);
            _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
            return new ResultViewModel { Validate = 1 };
        }
    }
}
