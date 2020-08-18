﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Brands.Virdi.Command;
using Biovation.Brands.Virdi.Manager;
using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Constants;
using Biovation.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Biovation.Brands.Virdi.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class VirdiUserController : Controller
    {
        private readonly Callbacks _callbacks;
        private readonly VirdiServer _virdiServer;
        private readonly TaskService _taskService;
        private readonly TaskManager _taskManager;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly CommandFactory _commandFactory;
        private readonly AccessGroupService _accessGroupService;

        public VirdiUserController(TaskService taskService, UserService userService, DeviceService deviceService, VirdiServer virdiServer, Callbacks callbacks, AccessGroupService accessGroupService, CommandFactory commandFactory, TaskManager taskManager)
        {
            _taskService = taskService;
            _userService = userService;
            _deviceService = deviceService;
            _virdiServer = virdiServer;
            _callbacks = callbacks;
            _accessGroupService = accessGroupService;
            _commandFactory = commandFactory;
            _taskManager = taskManager;
        }

        [HttpPost]
        public Task<ResultViewModel> EnrollFromTerminal([FromBody]uint deviceId)
        {
            return Task.Run(() =>
            {
                try
                {
                    var devices = _deviceService.GetDeviceInfo((int)deviceId);

                    var creatorUser = _userService.GetUser(123456789, false);

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.EnrollFromTerminal,
                        Priority = TaskPriorities.Medium,
                        TaskItems = new List<TaskItem>()
                    };
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = TaskStatuses.Queued,
                        TaskItemType = TaskItemTypes.EnrollFromTerminal,
                        Priority = TaskPriorities.Medium,
                        DueDate = DateTime.Today,
                        DeviceId = devices.DeviceId,
                        Data = JsonConvert.SerializeObject(new { deviceId }),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1
                    });
                    _taskService.InsertTask(task).Wait();
                    _taskManager.ProcessQueue();

                    var result = new ResultViewModel { Validate = 1, Message = "Enrolling User queued" };
                    return result;

                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 1, Message = $"Error ,Enrolling User not queued!{exception}" };
                }
            });

        }

        [HttpPost]
        public ResultViewModel ModifyUser([FromBody]User user)
        {
            try
            {
                _callbacks.LoadFingerTemplates();
                return new ResultViewModel { Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }

        [HttpGet]
        public Task<List<ResultViewModel>> SendUserToDevice(uint code, string userId, bool updateServerSideIdentification = false)
        {
            return Task.Run(() =>
            {
                var resultList = new List<ResultViewModel>();
                try
                {
                    var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.VirdiCode);
                    var deviceId = devices.DeviceId;
                    var userIds = JsonConvert.DeserializeObject<long[]>(userId);

                    var creatorUser = _userService.GetUser(123456789, false);
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.SendUsers,
                        Priority = TaskPriorities.Medium,
                        DeviceBrand = DeviceBrands.Virdi,
                        TaskItems = new List<TaskItem>()
                    };
                    foreach (var id in userIds)
                    {
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = TaskStatuses.Queued,
                            TaskItemType = TaskItemTypes.SendUser,
                            Priority = TaskPriorities.Medium,
                            DueDate = DateTime.Today,
                            DeviceId = deviceId,

                            Data = JsonConvert.SerializeObject(new { UserId = id }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1
                        });

                        if (!updateServerSideIdentification) continue;
                        _callbacks.AddUserToDeviceFastSearch(code, (int)id);
                    }

                    _taskService.InsertTask(task).Wait();
                    _taskManager.ProcessQueue();

                    resultList.Add(new ResultViewModel { Message = "Sending user queued", Validate = 1 });
                    return resultList;
                }
                catch (Exception exception)
                {
                    resultList.Add(new ResultViewModel { Message = exception.ToString(), Validate = 0 });
                    return resultList;
                }
            });
        }

        [HttpPost]
        public ResultViewModel SendUserToAllDevices([FromBody]User user)
        {
            var accessGroups = _accessGroupService.GetAccessGroupsOfUser(user.Id);
            if (!accessGroups.Any())
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
                            new List<object> { deviceGroupMember.Code, user.Id });

                        addUserToTerminalCommand.Execute();
                    }
                }
            }

            return new ResultViewModel { Id = user.Id, Validate = 1 };
        }

        [HttpGet]
        public ResultViewModel DeleteUserFromTerminal(uint code, int userId)
        {
            var deleteUserFromTerminalCommand = _commandFactory.Factory(CommandType.DeleteUserFromTerminal,
                new List<object> { code, userId });

            var result = deleteUserFromTerminalCommand.Execute();

            return new ResultViewModel { Id = userId, Validate = Convert.ToInt32(result) };
        }

        [HttpPost]
        public List<ResultViewModel> DeleteUserFromAllTerminal(int[] ids)
        {
            var onlineDevice = _virdiServer.GetOnlineDevices();
            var result = new List<ResultViewModel>();
            foreach (var device in onlineDevice)
            {
                foreach (var userId in ids)
                {
                    var deleteUserFromTerminalCommand = _commandFactory.Factory(CommandType.DeleteUserFromTerminal,
                        new List<object> { device.Key, userId });
                    var deleteResult = (bool)deleteUserFromTerminalCommand.Execute();
                    result.Add(new ResultViewModel { Id = userId, Validate = Convert.ToInt32(deleteResult) });
                }
            }
            return result;
        }

        [HttpPost]
        public Task<ResultViewModel> EnrollFaceTemplate(int userId, int deviceId)
        {
            return Task.Run(() =>
            {
                try
                {
                    var creatorUser = _userService.GetUser(123456789, false);
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.EnrollFaceFromTerminal,
                        Priority = TaskPriorities.Medium,
                        DeviceBrand = DeviceBrands.Virdi,
                        TaskItems = new List<TaskItem>()
                    };

                    task.TaskItems.Add(new TaskItem
                    {
                        Status = TaskStatuses.Queued,
                        TaskItemType = TaskItemTypes.EnrollFaceFromTerminal,
                        Priority = TaskPriorities.Medium,
                        DueDate = DateTime.Today,
                        DeviceId = deviceId,

                        Data = JsonConvert.SerializeObject(new { UserId = userId }),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1
                    });

                    _taskService.InsertTask(task).Wait();
                    _taskManager.ProcessQueue();

                    return new ResultViewModel { Id = userId, Validate = 1, Message = $"Enrolling face from device {deviceId} started successfuly." };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Id = userId, Validate = 0, Message = $"Enrolling face from device {deviceId} encountered an error: {exception}." };
                }
            });
        }
    }
}
