using Biovation.Brands.ZK.Command;
using Biovation.Brands.ZK.Devices;
using Biovation.Brands.ZK.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.ZK.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class ZkDeviceController : Controller
    {
        private readonly DeviceService _deviceService;
        private readonly AccessGroupService _accessGroupService;
        private readonly TaskService _taskService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly CommandFactory _commandFactory;
        private readonly ZkTecoServer _zkTecoServer;

        public ZkDeviceController(DeviceService deviceService, AccessGroupService accessGroupService, TaskService taskService, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskItemTypes taskItemTypes, DeviceBrands deviceBrands, Dictionary<uint, Device> onlineDevices, TaskManager taskManager, TaskStatuses taskStatuses, CommandFactory commandFactory, ZkTecoServer zkTecoServer)
        {
            _deviceService = deviceService;
            _accessGroupService = accessGroupService;
            _taskService = taskService;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskItemTypes = taskItemTypes;
            _deviceBrands = deviceBrands;
            _onlineDevices = onlineDevices;
            _taskManager = taskManager;
            _taskStatuses = taskStatuses;
            _commandFactory = commandFactory;
            _zkTecoServer = zkTecoServer;
        }

        [HttpGet]
        [AllowAnonymous]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            var onlineDevices = new List<DeviceBasicInfo>();

            foreach (var onlineDevice in _onlineDevices)
            {
                if (string.IsNullOrEmpty(onlineDevice.Value.GetDeviceInfo().Name))
                {
                    onlineDevice.Value.GetDeviceInfo().Name = _deviceService.GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault()?.Name;
                }
                onlineDevices.Add(onlineDevice.Value.GetDeviceInfo());
            }

            return onlineDevices;
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            var dbDevice = _deviceService.GetDevices(code: device.Code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();
            device.DeviceId = dbDevice.DeviceId;
            device.TimeSync = dbDevice.TimeSync;
            device.DeviceLockPassword = dbDevice.DeviceLockPassword;
            //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
            var creatorUser = HttpContext.GetUser();

            if (device.Active)
            {
                return Task.Run(() =>
                {
                    try
                    {
                        var task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = _taskTypes.UnlockDevice,
                            Priority = _taskPriorities.Medium,
                            DeviceBrand = _deviceBrands.ZkTeco,
                            TaskItems = new List<TaskItem>()
                        };
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.UnlockDevice,
                            Priority = _taskPriorities.Medium,

                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(device.DeviceId),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1
                        });
                        _zkTecoServer.ConnectToDevice(device);
                        _taskService.InsertTask(task);
                        _taskManager.ProcessQueue();
                        return new ResultViewModel { Validate = 1, Message = "Unlocking Device queued" };
                    }
                    catch (Exception exception)
                    {
                        return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                    }
                });
            }


            return Task.Run(() =>
            {
                try
                {
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.LockDevice,
                        Priority = _taskPriorities.Medium,
                        TaskItems = new List<TaskItem>(),
                        DeviceBrand = _deviceBrands.ZkTeco
                    };
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.LockDevice,
                        Priority = _taskPriorities.Medium,

                        DeviceId = device.DeviceId,
                        Data = JsonConvert.SerializeObject(device.DeviceId),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1,

                    });
                    _taskService.InsertTask(task);
                    _taskManager.ProcessQueue();
                    return new ResultViewModel { Validate = 1, Message = "locking Device queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });

        }


        [HttpPost]
        [Authorize]
        public ResultViewModel SendUsersOfDevice([FromBody] DeviceBasicInfo device)
        {
            try
            {
                var accessGroups = _accessGroupService.GetAccessGroups((uint)device.DeviceId);

                foreach (var accessGroup in accessGroups)
                {
                    foreach (var userGroup in accessGroup.UserGroup)
                    {
                        foreach (var userGroupMember in userGroup.Users)
                        {
                            var addUserToTerminalCommand = _commandFactory.Factory(CommandType.SendUserToDevice,
                                        new List<object> { device.Code, userGroupMember.UserCode });
                            addUserToTerminalCommand.Execute();
                        }
                    }
                }

                return new ResultViewModel { Validate = 1, Id = device.DeviceId };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Message = "SendUsersToDevice Failed." };
            }
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel> ReadOfflineOfDevice(uint code, DateTime? fromDate, DateTime? toDate)
        {
            return Task.Run(() =>
            {
                try
                {
                    var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();

                    //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                    var creatorUser = HttpContext.GetUser();

                    try
                    {
                        if (fromDate.HasValue && toDate.HasValue)
                        {
                            var task = new TaskInfo
                            {
                                CreatedAt = DateTimeOffset.Now,
                                CreatedBy = creatorUser,
                                TaskType = _taskTypes.GetLogsInPeriod,
                                Priority = _taskPriorities.Medium,
                                TaskItems = new List<TaskItem>(),
                                DeviceBrand = _deviceBrands.ZkTeco,
                            };
                            if (devices != null)
                                task.TaskItems.Add(new TaskItem
                                {
                                    Status = _taskStatuses.Queued,
                                    TaskItemType = _taskItemTypes.GetLogsInPeriod,
                                    Priority = _taskPriorities.Medium,
                                    DeviceId = devices.DeviceId,
                                    Data = JsonConvert.SerializeObject(new { fromDate, toDate }),
                                    IsParallelRestricted = true,
                                    IsScheduled = false,
                                    OrderIndex = 1,
                                });
                            _taskService.InsertTask(task);
                            _taskManager.ProcessQueue();
                            return new ResultViewModel { Validate = 1, Message = "Retriving Log queued" };
                        }

                        else
                        {
                            var task = new TaskInfo
                            {
                                CreatedAt = DateTimeOffset.Now,
                                CreatedBy = creatorUser,
                                TaskType = _taskTypes.GetLogs,
                                Priority = _taskPriorities.Medium,
                                TaskItems = new List<TaskItem>(),
                                DeviceBrand = _deviceBrands.ZkTeco,
                            };

                            task.TaskItems.Add(new TaskItem
                            {
                                Status = _taskStatuses.Queued,
                                TaskItemType = _taskItemTypes.GetLogs,
                                Priority = _taskPriorities.Medium,

                                DeviceId = devices.DeviceId,
                                Data = JsonConvert.SerializeObject(devices.DeviceId),
                                IsParallelRestricted = true,
                                IsScheduled = false,
                                OrderIndex = 1
                            });
                            _taskService.InsertTask(task);
                            _taskManager.ProcessQueue();
                        }

                        return new ResultViewModel { Validate = 1, Message = "Retriving Log queued" };
                    }
                    catch (Exception)
                    {
                        return new ResultViewModel { Validate = 0, Id = code, Message = "Retriving Log not queued" };

                    }
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Id = code, Message = "Retriving Log not queued" };
                }
            });
        }

        [HttpPost]
        [Authorize]
        public Task<List<ResultViewModel>> RetrieveUserFromDevice(uint code, [FromBody] JArray userId)
        {
            return Task.Run(() =>
            {
                try
                {
                    //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                    var creatorUser = HttpContext.GetUser();
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        DeviceBrand = _deviceBrands.ZkTeco,
                        TaskType = _taskTypes.RetrieveUserFromTerminal,
                        Priority = _taskPriorities.Medium,
                        TaskItems = new List<TaskItem>()
                    };
                    var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();
                    var deviceId = devices.DeviceId;

                    var userIds = JsonConvert.DeserializeObject<int[]>(userId.ToString());

                    foreach (var numericUserId in userIds)
                    {

                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.RetrieveUserFromTerminal,
                            Priority = _taskPriorities.Medium,

                            DeviceId = deviceId,
                            Data = JsonConvert.SerializeObject(new { userId = numericUserId }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,

                        });

                    }

                    _taskService.InsertTask(task);
                    _taskManager.ProcessQueue();

                    return new List<ResultViewModel>
                        {new ResultViewModel {Validate = 1, Message = "Retriving users queued"}};
                }

                catch (Exception exception)
                {
                    return new List<ResultViewModel>
                            {new ResultViewModel { Validate = 0, Message = exception.ToString() }};
                }
            });
        }
        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> DeleteUserFromDevice(uint code, [FromBody] JArray userId, bool updateServerSideIdentification = false)
        {
            return Task.Run(() =>
            {
                try
                {
                    var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();

                    //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                    var creatorUser = HttpContext.GetUser();

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.DeleteUsers,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = _deviceBrands.ZkTeco,
                        TaskItems = new List<TaskItem>()
                    };
                    var userIds = JsonConvert.DeserializeObject<List<uint>>(JsonConvert.SerializeObject(userId));

                    foreach (var id in userIds)
                    {

                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.DeleteUserFromTerminal,
                            Priority = _taskPriorities.Medium,

                            DeviceId = devices.DeviceId,
                            Data = JsonConvert.SerializeObject(new { userId = id }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,

                        });
                    }

                    _taskService.InsertTask(task);
                    _taskManager.ProcessQueue();

                    var result = new ResultViewModel { Validate = 1, Message = "Removing User queued" };
                    return result;
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 1, Message = $"Error ,Removing User not queued!{exception}" };

                }

            });
        }
        [HttpGet]
        [Authorize] 
        public ResultViewModel<List<User>> RetrieveUsersListFromDevice(uint code, bool embedTemplate = false)
        {
            /*var retrieveUserFromTerminalCommand = _commandFactory.Factory(CommandType.RetrieveUsersListFromDevice,
                new List<object> { code });

            var result = (List<User>)retrieveUserFromTerminalCommand.Execute();
            var resultViewModel = new ResultViewModel<List<User>> { Validate = 1, Data = result };
            return resultViewModel;
        }*/
            try
            {
                //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                var creatorUser = HttpContext.GetUser();

                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    DeviceBrand = _deviceBrands.ZkTeco,
                    TaskType = _taskTypes.RetrieveAllUsersFromTerminal,
                    Priority = _taskPriorities.Medium,
                    TaskItems = new List<TaskItem>()
                };
                var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();
                var deviceId = devices.DeviceId;

                task.TaskItems.Add(new TaskItem
                {
                    Status = _taskStatuses.Queued,
                    TaskItemType = _taskItemTypes.RetrieveAllUsersFromTerminal,
                    Priority = _taskPriorities.Medium,

                    DeviceId = deviceId,
                    Data = JsonConvert.SerializeObject(new { deviceId, embedTemplate }),
                    IsParallelRestricted = true,
                    IsScheduled = false,
                    OrderIndex = 1,

                });
                //_taskService.InsertTask(task);
                // ZKTecoServer.ProcessQueue();
                var result = (ResultViewModel<List<User>>)_commandFactory.Factory(CommandType.RetrieveUsersListFromDevice,
                    new List<object> { task.TaskItems.FirstOrDefault().DeviceId, task.TaskItems.FirstOrDefault().Id }).Execute();
                return result;
            }
            catch (Exception exception)
            {
                return new ResultViewModel<List<User>> { Validate = 0, Message = exception.ToString() };
            }


        }

        [HttpGet]
        [Authorize]
        public Dictionary<string, string> GetAdditionalData(uint code)
        {
            var getAdditionalData = _commandFactory.Factory(CommandType.GetDeviceAdditionalData,
               new List<object> { code });

            var result = getAdditionalData.Execute();

            return (Dictionary<string, string>)result;
        }

        [HttpPost]
        [Authorize]
        public Dictionary<uint, bool> DeleteDevices([FromBody] List<uint> deviceIds)
        {
            var resultList = new Dictionary<uint, bool>();

            foreach (var deviceId in deviceIds)
            {
                lock (_onlineDevices)
                {
                    if (_onlineDevices.ContainsKey(deviceId))
                    {
                        _onlineDevices[deviceId].Disconnect();
                        _onlineDevices.Remove(deviceId);
                    }
                }

                resultList.Add(deviceId, true);
            }

            return resultList;
        }

    }
}
