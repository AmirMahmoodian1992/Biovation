using Biovation.Brands.ZK.Manager;
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

namespace Biovation.Brands.ZK.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class ZkLogController : ControllerBase
    {
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;

        public ZkLogController(TaskService taskService, DeviceService deviceService, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskManager taskManager, DeviceBrands deviceBrands)
        {
            _taskService = taskService;
            _deviceService = deviceService;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskManager = taskManager;
            _deviceBrands = deviceBrands;
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> ClearLog(uint code, DateTime? fromDate, DateTime? toDate)
        {
            return Task.Run(() =>
            {
                try
                {
                    var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();

                    //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                    var creatorUser = HttpContext.GetUser();

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.ClearLog,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = _deviceBrands.ZkTeco,
                        TaskItems = new List<TaskItem>()
                    };
                    if (device != null)
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.ClearLog,
                            Priority = _taskPriorities.Medium,
                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(new
                            {
                                fromDate,
                                toDate
                            }),
                            IsParallelRestricted = true,
                            IsScheduled = false,

                            OrderIndex = 1
                        });


                    _taskService.InsertTask(task);
                    _taskManager.ProcessQueue();
                    return new ResultViewModel { Validate = 1, Message = "Clear LOg queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });

        }
    }
}