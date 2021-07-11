using Biovation.Brands.EOS.Commands;
using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.EOS.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class EosDeviceController : ControllerBase
    {
        private readonly EosServer _eosServer;
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;

        private readonly TaskTypes _taskTypes;
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly CommandFactory _commandFactory;
        private readonly Dictionary<uint, Device> _onlineDevices;

        private readonly ILogger _logger;

        public EosDeviceController(DeviceService deviceService, Dictionary<uint, Device> onlineDevices,
            EosServer eosServer, CommandFactory commandFactory, DeviceBrands deviceBrands, TaskTypes taskTypes,
            TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, TaskService taskService, ILogger logger)
        {
            _eosServer = eosServer;
            _deviceService = deviceService;
            _onlineDevices = onlineDevices;
            _commandFactory = commandFactory;
            _taskService = taskService;

            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _deviceBrands = deviceBrands;

            _logger = logger.ForContext<EosDeviceController>();
        }

        [HttpGet]
        [AllowAnonymous]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            var onlineDevices = new List<DeviceBasicInfo>();

            lock (_onlineDevices)
            {
                foreach (var onlineDevice in _onlineDevices)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(onlineDevice.Value.GetDeviceInfo().Name))
                        {
                            onlineDevice.Value.GetDeviceInfo().Name = _deviceService
                                .GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.EosCode).Result?.Data?.Data?.FirstOrDefault()
                                ?.Name;
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, exception.Message);
                    }

                    try
                    {
                        onlineDevices.Add(onlineDevice.Value.GetDeviceInfo());
                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, exception.Message);
                    }
                }
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
        public Dictionary<uint, bool> DeleteDevices([FromBody] List<uint> deviceIds)
        {
            var resultList = new Dictionary<uint, bool>();

            foreach (var deviceId in deviceIds)
            {
                var device = _deviceService.GetDevice(deviceId).Result?.Data;
                if (device is null)
                    continue;

                lock (_onlineDevices)
                {
                    if (_onlineDevices.ContainsKey(device.Code))
                    {
                        _onlineDevices[device.Code].Disconnect();
                        if (_onlineDevices.ContainsKey(device.Code))
                            _onlineDevices.Remove(device.Code);
                    }
                }

                resultList.Add(deviceId, true);
            }

            return resultList;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> DeleteUserFromDevice(uint code, [FromBody] List<int> userIds,
            bool updateServerSideIdentification = false)
        {
            try
            {
                var device = (await _deviceService.GetDevices(code: code, brandId: DeviceBrands.EosCode))?.Data?.Data
                    ?.FirstOrDefault();
                if (device is null)
                    return new ResultViewModel { Validate = 1, Message = $"Wrong device code is provided : {code}." };

                //var creatorUser = HttpContext.GetUser();

                //var task = new TaskInfo
                //{
                //    CreatedAt = DateTimeOffset.Now,
                //    CreatedBy = creatorUser,
                //    TaskType = _taskTypes.DeleteUsers,
                //    Priority = _taskPriorities.Medium,
                //    DeviceBrand = _deviceBrands.Eos,
                //    TaskItems = new List<TaskItem>(),
                //    DueDate = DateTime.Today
                //};

                ////var userIds = JsonConvert.DeserializeObject<int[]>(userId.ToString());

                //foreach (var id in userIds)
                //{
                //    task.TaskItems.Add(new TaskItem
                //    {
                //        Status = _taskStatuses.Queued,
                //        TaskItemType = _taskItemTypes.DeleteUserFromTerminal,
                //        Priority = _taskPriorities.Medium,
                //        DeviceId = device.DeviceId,
                //        Data = JsonConvert.SerializeObject(new { userCode = id }),
                //        IsParallelRestricted = true,
                //        IsScheduled = false,
                //        OrderIndex = 1,
                //        CurrentIndex = 0,
                //        TotalCount = 1
                //    });

                //}

                //_taskService.InsertTask(task);
                //await _taskService.ProcessQueue(_deviceBrands.Eos, device.DeviceId);

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
        }

        [HttpPost]
        [Authorize]
        public async Task<List<ResultViewModel>> RetrieveUserFromDevice(uint code, [FromBody] List<int> userIds)
        {
            try
            {
                return new List<ResultViewModel> {new ResultViewModel {Validate = 1, Message = "Retrieving users queued"}};
            }

            catch (Exception exception)
            {
                return new List<ResultViewModel>
                        {new ResultViewModel {Validate = 0, Message = exception.ToString()}};
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel<List<User>>> RetrieveUsersListFromDevice(uint code, bool embedTemplate = false)
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

                var devices = (await _deviceService.GetDevices(code: code, brandId: DeviceBrands.EosCode))?.Data?.Data
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
        public async Task<ResultViewModel> ReadOfflineOfDevice(uint code, DateTime? fromDate, DateTime? toDate)
        {
            return new ResultViewModel { Validate = 1 };
        }

        [HttpGet]
        [Authorize]
        public async Task<Dictionary<string, string>> GetAdditionalData(uint code)
        {
            var creatorUser = HttpContext.GetUser();

            var task = new TaskInfo
            {
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = creatorUser,
                TaskType = _taskTypes.GetLogsInPeriod,
                Priority = _taskPriorities.Immediate,
                DeviceBrand = _deviceBrands.Eos,
                TaskItems = new List<TaskItem>(),
                DueDate = DateTime.Today
            };
            var device = (await _deviceService.GetDevices(code: code, brandId: DeviceBrands.EosCode))?.Data?.Data
                ?.FirstOrDefault();

            if (device is null)
            {
                return null;
            }

            var deviceId = device.DeviceId;
            task.TaskItems.Add(new TaskItem
            {
                Status = _taskStatuses.Done,
                TaskItemType = _taskItemTypes.GetLogsInPeriod,
                Priority = _taskPriorities.Immediate,
                DeviceId = deviceId,
                Data = JsonConvert.SerializeObject(new { deviceId }),
                IsParallelRestricted = true,
                IsScheduled = false,
                OrderIndex = 1,
                CurrentIndex = 0
            });

            var getAdditionalData = _commandFactory.Factory(CommandType.GetDeviceAdditionalData,
                new List<object> { task.TaskItems.FirstOrDefault() });

            var result = getAdditionalData.Execute();

            return (Dictionary<string, string>)result;
        }
    }
}
