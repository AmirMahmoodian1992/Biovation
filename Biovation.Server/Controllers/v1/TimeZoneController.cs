using System;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Newtonsoft.Json;
using TimeZone = Biovation.Domain.TimeZone;

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

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskService _taskService;

        public TimeZoneController(TimeZoneService timeZoneService, DeviceService deviceService, RestClient restClient, BiovationConfigurationManager biovationConfigurationManager, TaskTypes taskTypes, TaskItemTypes taskItemTypes, TaskStatuses taskStatuses, TaskPriorities taskPriorities, TaskService taskService)
        {
            _deviceService = deviceService;
            _timeZoneService = timeZoneService;
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
            _kasraAdminToken = _biovationConfigurationManager.KasraAdminToken;

            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _taskService = taskService;
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
            var creatorUser = HttpContext.GetUser();
            var resultList = new List<ResultViewModel>();
            var deviceBrands = _deviceService.GetDeviceBrands(token: _kasraAdminToken);

            foreach (var deviceBrand in deviceBrands)
            {

                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = _taskTypes.SendTimeZoneToTerminal,
                    Priority = _taskPriorities.Medium,
                    DeviceBrand = deviceBrand,
                    TaskItems = new List<TaskItem>()
                };
                var devices = _deviceService.GetDevices(brandId: deviceBrand.Code);
                foreach (var device in devices)
                {
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.SendTimeZoneToTerminal,
                        Priority = _taskPriorities.Medium,
                        DeviceId = device.DeviceId,
                        Data = JsonConvert.SerializeObject(new { timeZone }),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1
                    });
                }
                _taskService.InsertTask(task);
                _taskService.ProcessQueue(deviceBrand).ConfigureAwait(false);

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
            var creatorUser = HttpContext.GetUser();
            var device = _deviceService.GetDevice(deviceId, token: _kasraAdminToken);
            var task = new TaskInfo
            {
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = creatorUser,
                TaskType = _taskTypes.SendTimeZoneToTerminal,
                Priority = _taskPriorities.Medium,
                DeviceBrand = device.Brand,
                TaskItems = new List<TaskItem>()
            };

            task.TaskItems.Add(new TaskItem
            {
                Status = _taskStatuses.Queued,
                TaskItemType = _taskItemTypes.SendTimeZoneToTerminal,
                Priority = _taskPriorities.Medium,
                DeviceId = device.DeviceId,
                Data = JsonConvert.SerializeObject(new { timeZoneId }),
                IsParallelRestricted = true,
                IsScheduled = false,

                OrderIndex = 1
            });

            _taskService.InsertTask(task);
            _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);

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
