using System;
using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Newtonsoft.Json;
using TimeZone = Biovation.Domain.TimeZone;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class TimeZoneController : ControllerBase
    {
        private readonly TimeZoneService _timeZoneService;
        private readonly DeviceService _deviceService;
        private readonly RestClient _restClient;

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskService _taskService;

        public TimeZoneController(TimeZoneService timeZoneService, DeviceService deviceService, RestClient restClient, TaskStatuses taskStatuses, TaskTypes taskTypes, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, TaskService taskService)
        {
            _deviceService = deviceService;
            _timeZoneService = timeZoneService;
            _restClient = restClient;

            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _taskService = taskService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ResultViewModel<TimeZone>> TimeZone([FromRoute] int id = default)
        {
            return await _timeZoneService.TimeZones(id, HttpContext.Items["Token"] as string);
        }

        [HttpGet]
        public async Task<ResultViewModel<PagingResult<TimeZone>>> TimeZones(int id = default, int accessGroupId = default, string name = default, int pageNumber = default, int pageSize = default)
        {
            return await _timeZoneService.GetTimeZones(id, accessGroupId, name, pageNumber, pageSize, HttpContext.Items["Token"] as string);
        }

        [HttpPost]
        public async Task<ResultViewModel> AddTimeZone([FromBody] TimeZone timeZone)
        {
            return await _timeZoneService.AddTimeZone(timeZone, HttpContext.Items["Token"] as string);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ResultViewModel> DeleteTimeZone([FromRoute] int id = default)
        {
            return await _timeZoneService.DeleteTimeZone(id, HttpContext.Items["Token"] as string);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ResultViewModel> ModifyTimeZone([FromRoute] int id, [FromBody] TimeZone timeZone = default)
        {
            return await _timeZoneService.ModifyTimeZone(id, timeZone, HttpContext.Items["Token"] as string);
        }

        //send to all device when deviceId is null
        [HttpPost]
        [Route("{id}/SendDevice/{deviceId}")]
        public async Task<ResultViewModel> SendTimeZoneDevice([FromRoute] int id = default, [FromRoute] int deviceId = default)
        {
            var creatorUser = HttpContext.GetUser();
            var token = HttpContext.Items["Token"] as string;
            var device = (await _deviceService.GetDevice(deviceId, token))?.Data;
            if (device is null)
                return new ResultViewModel
                {
                    Id = id,
                    Code = 404,
                    Message = "The provided device id is wrong",
                    Success = false,
                    Validate = 0
                };

            var task = new TaskInfo
            {
                Status = _taskStatuses.Queued,
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
                Data = JsonConvert.SerializeObject(new {id}),
                IsParallelRestricted = true,
                IsScheduled = false,

                OrderIndex = 1
            });

            await _taskService.InsertTask(task);
            await _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);

            _timeZoneService.SendTimeZoneDevice(id, deviceId, token);

            return new ResultViewModel { Validate = 1 };
        }

        // TODO - Verify Method.
        [HttpPost]
        [Route("{id}/SendTimeZoneToAllDevices")]
        public async Task<List<ResultViewModel>> SendTimeZoneToAllDevices([FromRoute] int id = default)
        {
            var creatorUser = HttpContext.GetUser();
            var devices = (await _deviceService.GetDevices())?.Data?.Data;
            if (devices is null)
                return new List<ResultViewModel> { new ResultViewModel { Id = id, Success = false, Validate = 0, Message = "No device is found", Code = 404 } };

            var result = new List<ResultViewModel>();
            foreach (var device in devices)
            {

                var task = new TaskInfo
                {
                    Status = _taskStatuses.Queued,
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
                    Data = JsonConvert.SerializeObject(new { id }),
                    IsParallelRestricted = true,
                    IsScheduled = false,

                    OrderIndex = 1
                });

                await _taskService.InsertTask(task);
                await _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);

                var sendTimeZoneResult = _timeZoneService.SendTimeZoneToAllDevices(id, device);

                //_restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
                //result.Add(new ResultViewModel { Validate = 1 });

                result.Add(sendTimeZoneResult);
            }

            return result;
        }
    }
}
