using Biovation.Brands.Virdi.Command;
using Biovation.Brands.Virdi.Manager;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly DeviceBrands _deviceBrands;
        private readonly DeviceService _deviceService;
        private readonly CommandFactory _commandFactory;
        private readonly AccessGroupService _accessGroupService;

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;

        public VirdiUserController(TaskService taskService, UserService userService, DeviceService deviceService, VirdiServer virdiServer, Callbacks callbacks, AccessGroupService accessGroupService, CommandFactory commandFactory, TaskManager taskManager, DeviceBrands deviceBrands, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities)
        {
            _taskService = taskService;
            _userService = userService;
            _deviceService = deviceService;
            _virdiServer = virdiServer;
            _callbacks = callbacks;
            _accessGroupService = accessGroupService;
            _commandFactory = commandFactory;
            _taskManager = taskManager;
            _deviceBrands = deviceBrands;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
        }

        [HttpPost]
        public Task<ResultViewModel> EnrollFromTerminal([FromBody] uint deviceId)
        {
            return Task.Run(() =>
            {
                try
                {
                    var devices = _deviceService.GetDevice(id: deviceId);

                    var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();


                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.EnrollFromTerminal,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = _deviceBrands.Virdi,
                        TaskItems = new List<TaskItem>(),
                        DueDate = DateTime.Today
                    };

                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.EnrollFromTerminal,
                        Priority = _taskPriorities.Medium,
                        DeviceId = devices.DeviceId,
                        Data = JsonConvert.SerializeObject(new { deviceId }),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1,
                        CurrentIndex = 0,
                        TotalCount = 1
                    });

                    _taskService.InsertTask(task);
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
        public ResultViewModel ModifyUser([FromBody] User user)
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
                    var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.VirdiCode).FirstOrDefault();
                    var deviceId = devices.DeviceId;
                    var userIds = JsonConvert.DeserializeObject<long[]>(userId);

                    var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.SendUsers,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = _deviceBrands.Virdi,
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
                            DeviceId = deviceId,
                            Data = JsonConvert.SerializeObject(new { UserId = id }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,
                            CurrentIndex = 0,
                            TotalCount = 1
                        });

                        if (!updateServerSideIdentification) continue;
                        _callbacks.AddUserToDeviceFastSearch(code, (int)id);
                    }

                    _taskService.InsertTask(task);
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
        public ResultViewModel SendUserToAllDevices([FromBody] User user)
        {
            var accessGroups = _accessGroupService.GetAccessGroups(user.Id);
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
                    var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.EnrollFaceFromTerminal,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = _deviceBrands.Virdi,
                        TaskItems = new List<TaskItem>(),
                        DueDate = DateTime.Today
                    };

                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.EnrollFaceFromTerminal,
                        Priority = _taskPriorities.Medium,
                        DeviceId = deviceId,
                        Data = JsonConvert.SerializeObject(new { UserId = userId }),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1,
                        TotalCount = 1,
                        CurrentIndex = 0
                    });

                    _taskService.InsertTask(task);
                    _taskManager.ProcessQueue();

                    return new ResultViewModel { Id = userId, Validate = 1, Message = $"Enrolling face from device {deviceId} started successfully." };
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
