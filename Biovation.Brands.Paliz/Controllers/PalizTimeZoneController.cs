//using Biovation.Brands.Paliz.Command;
using Biovation.Brands.Paliz.Command;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Extension;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using DeviceBrands = Biovation.Constants.DeviceBrands;
using TaskItem = Biovation.Domain.TaskItem;

namespace Biovation.Brands.Paliz.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class PalizTimeZoneController : ControllerBase
    {
        private readonly TaskService _taskService;
        private readonly DeviceBrands _deviceBrands;
        private readonly DeviceService _deviceService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;

        public PalizTimeZoneController(TaskService taskService, DeviceService deviceService,
            AccessGroupService accessGroupService, DeviceBrands deviceBrands,
            TaskTypes taskTypes, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses,
            BiovationConfigurationManager configurationManager, CommandFactory commandFactory, Dictionary<uint, DeviceBasicInfo> onlineDevices)
        {
            //_palizServer = palizServer;
            _taskService = taskService;
            _deviceService = deviceService;
            _deviceBrands = deviceBrands;
            _taskTypes = taskTypes;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _taskStatuses = taskStatuses;
        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel> SendTimeZoneToDevice(uint code, int timeZoneId)
        {
            try
            {
                var creatorUser = HttpContext.GetUser();
                var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.PalizCode).FirstOrDefault();

                if (device is null)
                {
                    return new ResultViewModel { Success = false, Message = "Wrong device code is provided" };
                }

                try
                {
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.SendTimeZoneToTerminal,
                        Priority = _taskPriorities.Medium,
                        TaskItems = new List<TaskItem>(),
                        DeviceBrand = _deviceBrands.Paliz,
                        DueDate = DateTimeOffset.Now
                    };

                    if (code != default)
                    {
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.SendTimeZoneToTerminal,
                            Priority = _taskPriorities.Medium,
                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(new { code, timeZoneId }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,
                        });
                    }

                    else
                    {
                        var palizDevices = _deviceService.GetDevices(brandId: DeviceBrands.PalizCode);
                        foreach (var palizDevice in palizDevices)
                        {
                            task.TaskItems.Add(new TaskItem
                            {
                                Status = _taskStatuses.Queued,
                                TaskItemType = _taskItemTypes.SendTimeZoneToTerminal,
                                Priority = _taskPriorities.Medium,
                                DeviceId = palizDevice.DeviceId,
                                Data = JsonConvert.SerializeObject(new { code, timeZoneId }),
                                IsParallelRestricted = true,
                                IsScheduled = false,
                                OrderIndex = 1
                            });
                        }
                    }

                    _taskService.InsertTask(task);
                    await _taskService.ProcessQueue(_deviceBrands.Paliz).ConfigureAwait(false);

                    return new ResultViewModel { Validate = 1, Message = "Sending time zone to device queued" };
                }
                catch (Exception)
                {
                    return new ResultViewModel { Validate = 0, Id = code, Message = "Sending time zone to device not queued" };
                }
            }

            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = code, Message = "Sending time zone to device not queued" };
            }
        }
    }
}
