using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.Suprema.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class SupremaTimeZoneController : ControllerBase
    {
        private readonly DeviceService _deviceService;
        private readonly TaskService _taskService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly DeviceBrands _deviceBrands;

        public SupremaTimeZoneController(DeviceService deviceService, TaskService taskService, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, DeviceBrands deviceBrands)
        {
            _deviceService = deviceService;
            _taskService = taskService;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _deviceBrands = deviceBrands;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> SendTimeZoneToAllDevices([FromBody] int timeZoneId)
        {
            try
            {
                return new ResultViewModel { Validate = 1, Message = "Sending TimeZoneToTerminal queued" };
            }
            catch (Exception exception)
            {
                return new ResultViewModel
                {
                    Validate = 0,
                    Message = exception.ToString()
                };
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel> SendTimeZoneToDevice(int timeZoneId, uint code)
        {
            try
            {
                var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.SupremaCode).FirstOrDefault();

                //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                var creatorUser = HttpContext.GetUser();
                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = _taskTypes.SendTimeZoneToTerminal,
                    Priority = _taskPriorities.Medium,
                    DeviceBrand = _deviceBrands.Suprema,
                    TaskItems = new List<TaskItem>()
                };

                if (device != null)
                {
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
                    await _taskService.ProcessQueue(_deviceBrands.Suprema, device.DeviceId)
                        .ConfigureAwait(false);
                }

                return new ResultViewModel { Validate = 1, Message = "Sending TimeZoneToTerminal queued" };
            }
            catch (Exception exception)
            {
                return new ResultViewModel
                {
                    Validate = 0,
                    Message = exception.ToString()
                };
            }
        }
    }
}
