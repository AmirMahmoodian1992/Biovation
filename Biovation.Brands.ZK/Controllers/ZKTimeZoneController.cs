using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Brands.ZK.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Biovation.Brands.ZK.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class ZkTimeZoneController : Controller
    {
        private readonly DeviceService _deviceService;
        private readonly TaskService _taskService;
        private readonly UserService _userService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;

        public ZkTimeZoneController(DeviceService deviceService, TaskService taskService, UserService userService, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskManager taskManager, DeviceBrands deviceBrands)
        {
            _deviceService = deviceService;
            _taskService = taskService;
            _userService = userService;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskManager = taskManager;
            _deviceBrands = deviceBrands;
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> SendTimeZoneToAllDevices([FromBody] int timeZoneId)
        {
            return Task.Run(() =>
            {
                try
                {
                    var devices = _deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode);
                    var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.SendTimeZoneToTerminal,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = _deviceBrands.ZkTeco,
                        TaskItems = new List<TaskItem>()
                    };

                    foreach (var device in devices)
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
                    }
                    _taskService.InsertTask(task);
                    _taskManager.ProcessQueue();


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
            });

        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel> SendTimeZoneToDevice(int timeZoneId, uint code)
        {
            return Task.Run(() =>
                {
                    try
                    {
                        var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();

                        var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                        var task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = _taskTypes.SendTimeZoneToTerminal,
                            Priority = _taskPriorities.Medium,
                            DeviceBrand = _deviceBrands.ZkTeco,
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
                        _taskManager.ProcessQueue();
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
                });
        }
    }
}
