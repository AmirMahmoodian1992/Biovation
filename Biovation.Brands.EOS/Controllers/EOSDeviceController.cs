using Biovation.Brands.EOS.Commands;
using Biovation.Brands.EOS.Devices;
using Biovation.Brands.EOS.Manager;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.EOS.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class EosDeviceController : Controller
    {
        private readonly EosServer _eosServer;
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;

        private readonly TaskTypes _taskTypes;
        private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly CommandFactory _commandFactory;
        private readonly Dictionary<uint, Device> _onlineDevices;

        public EosDeviceController(DeviceService deviceService, Dictionary<uint, Device> onlineDevices,
            EosServer eosServer, CommandFactory commandFactory, TaskManager taskManager, DeviceBrands deviceBrands,
            TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities,
            TaskService taskService)
        {
            _eosServer = eosServer;
            _deviceService = deviceService;
            _onlineDevices = onlineDevices;
            _commandFactory = commandFactory;
            _taskService = taskService;
            _taskManager = taskManager;

            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _deviceBrands = deviceBrands;
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
                    onlineDevice.Value.GetDeviceInfo().Name = _deviceService
                        .GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.EosCode)?.Data?.Data?.FirstOrDefault()
                        ?.Name;
                }

                onlineDevices.Add(onlineDevice.Value.GetDeviceInfo());
            }

            return onlineDevices;
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            if (device.Active)
            {
                _eosServer.ConnectToDevice(device);
            }

            else
            {
                _eosServer.DisconnectFromDevice(device);
            }

            return new ResultViewModel { Validate = 0, Id = device.DeviceId };
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> DeleteUserFromDevice(uint code, [FromBody] List<int> userIds,
            bool updateServerSideIdentification = false)
        {
            return Task.Run(() =>
            {
                try
                {
                    var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.EosCode)?.Data?.Data
                        ?.FirstOrDefault();
                    if (device is null)
                        return new ResultViewModel { Validate = 1, Message = $"Wrong device code is provided : {code}." };

                    var creatorUser = HttpContext.GetUser();

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.DeleteUsers,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = _deviceBrands.Eos,
                        TaskItems = new List<TaskItem>(),
                        DueDate = DateTime.Today
                    };

                    //var userIds = JsonConvert.DeserializeObject<int[]>(userId.ToString());

                    foreach (var id in userIds)
                    {
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.DeleteUserFromTerminal,
                            Priority = _taskPriorities.Medium,
                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(new { userCode = id }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,
                            CurrentIndex = 0,
                            TotalCount = 1
                        });

                    }

                    _taskService.InsertTask(task);
                    _taskManager.ProcessQueue();

                    //foreach (var id in userIds)
                    //{
                    //    var deleteUser = _commandFactory.Factory(CommandType.DeleteUserFromTerminal,
                    //                            new List<object> { code, id });
                    //    var deleteResult = deleteUser.Execute();
                    //}

                    var result = new ResultViewModel { Validate = 1, Message = "Removing User queued" };
                    return result;

                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 1, Message = $"Error ,Removing User not queued!{exception}" };
                }
            });
        }

        [HttpPost]
        [Authorize]
        public Task<List<ResultViewModel>> RetrieveUserFromDevice(uint code, [FromBody] List<int> userIds)
        {

            return Task.Run(() =>
            {
                try
                {
                    var creatorUser = HttpContext.GetUser();

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        DeviceBrand = _deviceBrands.Eos,
                        TaskType = _taskTypes.RetrieveUserFromTerminal,
                        Priority = _taskPriorities.Medium,
                        TaskItems = new List<TaskItem>(),
                        DueDate = DateTime.Today
                    };

                    var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.EosCode)?.Data?.Data
                        ?.FirstOrDefault();
                    if (device is null)
                        return new List<ResultViewModel>
                            {new ResultViewModel {Validate = 1, Message = $"Wrong device code is provided : {code}."}};

                    foreach (var id in userIds)
                    {
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.RetrieveUserFromTerminal,
                            Priority = _taskPriorities.Medium,
                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(new { userCode = id }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,
                            CurrentIndex = 0,
                            TotalCount = userIds.Count
                        });
                    }

                    _taskService.InsertTask(task);
                    _taskManager.ProcessQueue();

                    //    foreach (var id in userIds)
                    //    {
                    //        var getUser = _commandFactory.Factory(CommandType.RetrieveUserFromDevice,
                    //new List<object> { deviceId, id });
                    //        var getUserResult = getUser.Execute();
                    //    }

                    return new List<ResultViewModel>
                        {new ResultViewModel {Validate = 1, Message = "Retrieving users queued"}};
                }

                catch (Exception exception)
                {
                    return new List<ResultViewModel>
                        {new ResultViewModel {Validate = 0, Message = exception.ToString()}};
                }
            });
        }

        [HttpGet]
        [Authorize]
        public ResultViewModel<List<User>> RetrieveUsersListFromDevice(uint code, bool embedTemplate)
        {
            try
            {
                //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                var creatorUser = HttpContext.GetUser();

                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = _taskTypes.RetrieveAllUsersFromTerminal,
                    Priority = _taskPriorities.Medium,
                    DeviceBrand = _deviceBrands.Eos,
                    TaskItems = new List<TaskItem>(),
                    DueDate = DateTime.Today
                };

                var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.EosCode)?.Data?.Data
                    ?.FirstOrDefault();
                if (devices is null)
                    return new ResultViewModel<List<User>>
                    { Validate = 0, Message = $"Wrong device code is provided : {code}." };

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
                    CurrentIndex = 0
                });

                //_taskService.InsertTask(task);
                //_taskManager.ProcessQueue();

                var result = (ResultViewModel<List<User>>)_commandFactory.Factory(
                        CommandType.RetrieveUsersListFromDevice,
                        new List<object>
                            {task.TaskItems?.FirstOrDefault()})
                    .Execute();

                return result;

            }
            catch (Exception exception)
            {
                return new ResultViewModel<List<User>> { Validate = 0, Message = exception.ToString() };
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
                    var creatorUser = HttpContext.GetUser();

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.GetLogsInPeriod,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = _deviceBrands.Eos,
                        TaskItems = new List<TaskItem>(),
                        DueDate = DateTime.Today
                    };

                    var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.EosCode)?.Data?.Data
                        ?.FirstOrDefault();
                    if (device is null)
                    {
                        return new ResultViewModel { Validate = 0, Message = "Device with code is not exist" };
                    }

                    if (fromDate is null && toDate is null)
                    {
                        fromDate = new DateTime(1970);
                        toDate = new DateTime(2050);
                    }
                    else if (fromDate is null)
                    {
                        fromDate = new DateTime(1970);
                    }
                    else if (toDate is null)
                    {
                        toDate = new DateTime(2050);
                    }

                    var deviceId = device.DeviceId;
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.GetLogsInPeriod,
                        Priority = _taskPriorities.Medium,
                        DeviceId = deviceId,
                        Data = JsonConvert.SerializeObject(new { fromDate, toDate }),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1,
                        CurrentIndex = 0
                    });

                    _taskService.InsertTask(task);
                    _taskManager.ProcessQueue();

                    return new ResultViewModel { Validate = 1 };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.Message };
                }
            });
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel> ManualActivationProcessQueue()
        {
            return Task.Run(() =>
            {
                try
                {
                    _taskManager.ProcessQueue();
                    return new ResultViewModel {Success = true};
                }
                catch(Exception exception)
                {
                    return new ResultViewModel {Success = false, Message = exception.ToString()};
                }
            });
        }

    }
}
