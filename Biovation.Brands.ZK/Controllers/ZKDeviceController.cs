using Biovation.Brands.ZK.Command;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biovation.CommonClasses.Models.ConstantValues;
using Newtonsoft.Json.Linq;
using DeviceBrands = Biovation.CommonClasses.Models.ConstantValues.DeviceBrands;

namespace Biovation.Brands.ZK.ApiControllers
{
    public class ZKDeviceController : ApiController
    {
        private readonly DeviceService _deviceService = new DeviceService();
        private readonly AccessGroupService _accessGroupService = new AccessGroupService();
        private readonly UserService _userService = new UserService();
        private readonly TaskService _taskService = new TaskService();
        [HttpGet]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            var onlineDevices = new List<DeviceBasicInfo>();

            foreach (var onlineDevice in ZKTecoServer.GetOnlineDevices())
            {
                if (string.IsNullOrEmpty(onlineDevice.Value.GetDeviceInfo().Name))
                {
                    onlineDevice.Value.GetDeviceInfo().Name = _deviceService.GetDeviceBasicInfoWithCode(onlineDevice.Key, DeviceBrands.MaxaCode).Name;
                }
                onlineDevices.Add(onlineDevice.Value.GetDeviceInfo());
            }

            return onlineDevices;
        }

        [HttpPost]
        public Task<ResultViewModel> ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            var zkTecoServer = ZKTecoServer.FactoryZKServer();
            var dbDevice = _deviceService.GetDeviceBasicInfoWithCode(device.Code,
                CommonClasses.Models.ConstantValues.DeviceBrands.ZkTecoCode);
            device.DeviceId = dbDevice.DeviceId;
            device.TimeSync = dbDevice.TimeSync;
            device.DeviceLockPassword = dbDevice.DeviceLockPassword;
            var creatorUser = _userService.GetUser(123456789, false);

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
                            TaskType = TaskTypes.UnlockDevice,
                            Priority = TaskPriorities.Medium,
                            DeviceBrand = DeviceBrands.ZkTeco,
                            TaskItems = new List<TaskItem>()
                        };
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = TaskStatuses.Queued,
                            TaskItemType = TaskItemTypes.UnlockDevice,
                            Priority = TaskPriorities.Medium,
                            DueDate = DateTime.Today,
                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(device.DeviceId),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1
                        });
                        zkTecoServer.ConnectToDevice(device);
                        _taskService.InsertTask(task).Wait();
                        ZKTecoServer.ProcessQueue();
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
                        TaskType = TaskTypes.LockDevice,
                        Priority = TaskPriorities.Medium,
                        TaskItems = new List<TaskItem>(),
                        DeviceBrand = DeviceBrands.ZkTeco
                    };
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = TaskStatuses.Queued,
                        TaskItemType = TaskItemTypes.LockDevice,
                        Priority = TaskPriorities.Medium,
                        DueDate = DateTime.Today,
                        DeviceId = device.DeviceId,
                        Data = JsonConvert.SerializeObject(device.DeviceId),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1,

                    });
                    _taskService.InsertTask(task).Wait();
                    ZKTecoServer.ProcessQueue();
                    return new ResultViewModel { Validate = 1, Message = "locking Device queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });

        }


        [HttpPost]
        public ResultViewModel SendUsersOfDevice([FromBody]DeviceBasicInfo device)
        {
            try
            {
                var accessGroups = _accessGroupService.GetAccessGroupsOfDevice((uint)device.DeviceId);

                foreach (var accessGroup in accessGroups)
                {
                    foreach (var userGroup in accessGroup.UserGroup)
                    {
                        foreach (var userGroupMember in userGroup.Users)
                        {
                            var addUserToTerminalCommand = CommandFactory.Factory(CommandType.SendUserToDevice,
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
        public Task<ResultViewModel> ReadOfflineOfDevice(uint code, DateTime? fromDate, DateTime? toDate)
        {
            return Task.Run(() =>
            {
                try
                {
                    var creatorUser = _userService.GetUser(123456789, false);
                    var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.ZkTecoCode);

                    try
                    {
                        if (fromDate.HasValue && toDate.HasValue)
                        {
                            var task = new TaskInfo
                            {
                                CreatedAt = DateTimeOffset.Now,
                                CreatedBy = creatorUser,
                                TaskType = TaskTypes.GetLogsInPeriod,
                                Priority = TaskPriorities.Medium,
                                TaskItems = new List<TaskItem>(),
                                DeviceBrand = DeviceBrands.ZkTeco,
                            };
                            task.TaskItems.Add(new TaskItem
                            {
                                Status = TaskStatuses.Queued,
                                TaskItemType = TaskItemTypes.GetLogsInPeriod,
                                Priority = TaskPriorities.Medium,
                                DueDate = DateTimeOffset.Now,
                                DeviceId = devices.DeviceId,
                                Data = JsonConvert.SerializeObject(new { fromDate, toDate }),
                                IsParallelRestricted = true,
                                IsScheduled = false,
                                OrderIndex = 1,

                            });
                            _taskService.InsertTask(task).Wait();
                            ZKTecoServer.ProcessQueue();
                            return new ResultViewModel { Validate = 1, Message = "Retriving Log queued" };
                        }

                        else
                        {
                            var task = new TaskInfo
                            {
                                CreatedAt = DateTimeOffset.Now,
                                CreatedBy = creatorUser,
                                TaskType = TaskTypes.GetLogs,
                                Priority = TaskPriorities.Medium,
                                TaskItems = new List<TaskItem>(),
                                DeviceBrand = DeviceBrands.ZkTeco,
                            };

                            task.TaskItems.Add(new TaskItem
                            {
                                Status = TaskStatuses.Queued,
                                TaskItemType = TaskItemTypes.GetLogs,
                                Priority = TaskPriorities.Medium,
                                DueDate = DateTime.Today,
                                DeviceId = devices.DeviceId,
                                Data = JsonConvert.SerializeObject(devices.DeviceId),
                                IsParallelRestricted = true,
                                IsScheduled = false,
                                OrderIndex = 1
                            });
                            _taskService.InsertTask(task).Wait();
                            ZKTecoServer.ProcessQueue();
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
        public Task<List<ResultViewModel>> RetrieveUserFromDevice(uint code, [FromBody]JArray userId)
        {
            return Task.Run(() =>
                {
                    var result = new List<ResultViewModel>();
                    try
                    {
                        var creatorUser = _userService.GetUser(123456789, false);
                        var task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            DeviceBrand = DeviceBrands.ZkTeco,
                            TaskType = TaskTypes.RetrieveUserFromTerminal,
                            Priority = TaskPriorities.Medium,
                            TaskItems = new List<TaskItem>()
                        };
                        var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.ZkTecoCode);
                        var deviceId = devices.DeviceId;

                        var userIds = JsonConvert.DeserializeObject<int[]>(userId.ToString());

                        foreach (var numericUserId in userIds)
                        {

                            task.TaskItems.Add(new TaskItem
                            {
                                Status = TaskStatuses.Queued,
                                TaskItemType = TaskItemTypes.RetrieveUserFromTerminal,
                                Priority = TaskPriorities.Medium,
                                DueDate = DateTime.Today,
                                DeviceId = deviceId,
                                Data = JsonConvert.SerializeObject(new { userId = numericUserId }),
                                IsParallelRestricted = true,
                                IsScheduled = false,
                                OrderIndex = 1,

                            });

                        }

                        _taskService.InsertTask(task).Wait();
                        ZKTecoServer.ProcessQueue();

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
        public Task<ResultViewModel> DeleteUserFromDevice(uint code, [FromBody] JArray userId, bool updateServerSideIdentification = false)
        {
            return Task.Run(() =>
            {
                try
                {
                    var creatorUser = _userService.GetUser(123456789, false);
                    var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.ZkTecoCode);
                    var deviceId = devices.DeviceId;
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.DeleteUsers,
                        Priority = TaskPriorities.Medium,
                        DeviceBrand = DeviceBrands.ZkTeco,
                        TaskItems = new List<TaskItem>()
                    };
                    var userIds = JsonConvert.DeserializeObject<List<uint>>(JsonConvert.SerializeObject(userId));

                    foreach (var id in userIds)
                    {

                        task.TaskItems.Add(new TaskItem
                        {
                            Status = TaskStatuses.Queued,
                            TaskItemType = TaskItemTypes.DeleteUserFromTerminal,
                            Priority = TaskPriorities.Medium,
                            DueDate = DateTime.Today,
                            DeviceId = devices.DeviceId,
                            Data = JsonConvert.SerializeObject(new { userId = id }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,

                        });
                    }

                    _taskService.InsertTask(task).Wait();
                    ZKTecoServer.ProcessQueue();

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
        public ResultViewModel<List<User>> RetrieveUsersListFromDevice(uint code)
        {
            /*var retrieveUserFromTerminalCommand = CommandFactory.Factory(CommandType.RetrieveUsersListFromDevice,
                new List<object> { code });

            var result = (List<User>)retrieveUserFromTerminalCommand.Execute();
            var resultViewModel = new ResultViewModel<List<User>> { Validate = 1, Data = result };
            return resultViewModel;
        }*/
            var result = new ResultViewModel<List<User>>();
            try
            {
                var creatorUser = _userService.GetUser(123456789, false);
                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    DeviceBrand = DeviceBrands.ZkTeco,
                    TaskType = TaskTypes.RetrieveAllUsersFromTerminal,
                    Priority = TaskPriorities.Medium,
                    TaskItems = new List<TaskItem>()
                };
                var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.ZkTecoCode);
                var deviceId = devices.DeviceId;

                task.TaskItems.Add(new TaskItem
                {
                    Status = TaskStatuses.Queued,
                    TaskItemType = TaskItemTypes.RetrieveAllUsersFromTerminal,
                    Priority = TaskPriorities.Medium,
                    DueDate = DateTime.Today,
                    DeviceId = deviceId,
                    Data = JsonConvert.SerializeObject(new { deviceId }),
                    IsParallelRestricted = true,
                    IsScheduled = false,
                    OrderIndex = 1,

                });
                //_taskService.InsertTask(task).Wait();
               // ZKTecoServer.ProcessQueue();
                 result = (ResultViewModel<List<User>>)CommandFactory.Factory(CommandType.RetrieveUsersListFromDevice,
                    new List<object> { task.TaskItems.FirstOrDefault().DeviceId, task.TaskItems.FirstOrDefault().Id }).Execute();
                 return result;
            }
            catch (Exception exception)
            {
                return new ResultViewModel<List<User>> { Validate = 0, Message = exception.ToString() };
            }


        }

        [HttpGet]
        public Dictionary<string, string> GetAdditionalData(uint code)
        {
            var getAdditionalData = CommandFactory.Factory(CommandType.GetDeviceAdditionalData,
               new List<object> { code });

            var result = getAdditionalData.Execute();

            return (Dictionary<string, string>)result;
        }

        [HttpPost]
        public Dictionary<uint, bool> DeleteDevices([FromBody]List<uint> deviceIds)
        {
            var resultList = new Dictionary<uint, bool>();

            foreach (var deviceId in deviceIds)
            {
                lock (ZKTecoServer.GetOnlineDevices())
                {
                    if (ZKTecoServer.GetOnlineDevices().ContainsKey(deviceId))
                    {
                        ZKTecoServer.GetOnlineDevices()[deviceId].Disconnect();
                        ZKTecoServer.GetOnlineDevices().Remove(deviceId);
                    }
                }

                resultList.Add(deviceId, true);
            }

            return resultList;
        }
    }
}
