using Biovation.Brands.Eos.Manager;
using Biovation.Brands.EOS.Commands;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.EOS.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class EosUserController : Controller
    {
        private readonly UserService _userService;
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;
        private readonly AccessGroupService _accessGroupService;

        private readonly TaskTypes _taskTypes;
        private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly CommandFactory _commandFactory;

        public EosUserController(UserService userService, AccessGroupService accessGroupService, CommandFactory commandFactory, TaskService taskService, TaskManager taskManager, TaskTypes taskTypes,
            TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, DeviceBrands deviceBrands, DeviceService deviceService)
        {
            _userService = userService;
            _accessGroupService = accessGroupService;
            _commandFactory = commandFactory;

            _taskService = taskService;
            _taskManager = taskManager;
            _taskItemTypes = taskItemTypes;
            _taskStatuses = taskStatuses;
            _taskPriorities = taskPriorities;
            _taskTypes = taskTypes;
            _deviceBrands = deviceBrands;
            _deviceService = deviceService;
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel> SendUserToDevice(uint code, string userId)
        {
            return Task.Run(() =>
            {
                try
                {
                    var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.EosCode)?.Data?.Data?.FirstOrDefault();
                    if (device is null)
                        return new ResultViewModel { Validate = 0, Message = $"Wrong device code is provided : {code}." }; 
                    
                    var userIds = JsonConvert.DeserializeObject<uint[]>(userId);
                    var creatorUser = HttpContext.GetUser();

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.SendUsers,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = _deviceBrands.Eos,
                        TaskItems = new List<TaskItem>(),
                        DueDate = DateTime.Today
                    };

                    foreach (var id in userIds)
                    {
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.SendUser,
                            Priority = _taskPriorities.Medium,
                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(new { UserId = id }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,
                            CurrentIndex = 0,
                            TotalCount = 1
                        });
                    }

                    _taskService.InsertTask(task);
                    _taskManager.ProcessQueue();

                    //foreach (var receivedUserId in userIds)
                    //{
                    //    _commandFactory.Factory(CommandType.SendUserToDevice, new List<object> {code, receivedUserId})
                    //        .Execute();
                    //}

                    return new ResultViewModel { Validate = 1 };
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    return new ResultViewModel { Validate = 0, Message = e.Message };
                }
            });
        }


        [HttpPost]
        [Authorize]
        public ResultViewModel SendUserToAllDevices([FromBody] User user)
        {
            var accessGroups = _accessGroupService.GetAccessGroups(user.Id)?.Data?.Data;
            if (accessGroups == null || !accessGroups.Any())
            {
                return new ResultViewModel { Id = user.Id, Validate = 0 };
            }
            foreach (var accessGroup in accessGroups)
            {
                foreach (var deviceGroup in accessGroup.DeviceGroup)
                {
                    foreach (var deviceGroupMember in deviceGroup.Devices)
                    {
                        var addUserToTerminalCommand = _commandFactory.Factory(CommandType.SendUserToDevice,
                            new List<object> { deviceGroupMember.Code, user.Code });

                        addUserToTerminalCommand.Execute();
                    }
                }
            }

            return new ResultViewModel { Id = user.Id, Validate = 1 };
        }
    }
}
