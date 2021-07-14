using Biovation.Brands.ZK.Command;
using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.ZK.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceService _deviceService;
        private readonly DeviceBrands _deviceBrands;
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly CommandFactory _commandFactory;
        private readonly ZkTecoServer _zkTecoServer;
        private readonly ILogger _logger;

        public DeviceController(DeviceService deviceService, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskItemTypes taskItemTypes, DeviceBrands deviceBrands, Dictionary<uint, Device> onlineDevices , TaskStatuses taskStatuses, CommandFactory commandFactory, ZkTecoServer zkTecoServer, ILogger logger)
        {
            _deviceService = deviceService;
           
            _deviceBrands = deviceBrands;
            _onlineDevices = onlineDevices;
            _commandFactory = commandFactory;
            _zkTecoServer = zkTecoServer;
            _logger = logger.ForContext<DeviceController>();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<List<DeviceBasicInfo>> GetOnlineDevices()
        {
            return await Task.Run(() =>
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
                                    .GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault()
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
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            var storedDevice = _deviceService.GetDevices(code: device.Code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();
            if (storedDevice is null)
                return new ResultViewModel { Success = false, Message = $"Device {device.Code} does not exists" };

            device.DeviceId = storedDevice.DeviceId;
            device.TimeSync = storedDevice.TimeSync;
            device.DeviceLockPassword = storedDevice.DeviceLockPassword;
            //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();

            if (device.Active)
            {
                //return await Task.Run(async () =>
                //{
                try
                {

                    await Task.Run(() => _zkTecoServer.ConnectToDevice(device));
                    
                    //_taskManager.ProcessQueue(device.DeviceId);
                    return new ResultViewModel { Validate = 1, Message = "Unlocking Device queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
                //});
            }


            //return await Task.Run(() =>
            //{
            try
            {
                //_taskManager.ProcessQueue(device.DeviceId);
                await _zkTecoServer.DisconnectFromDevice(device);
                return new ResultViewModel { Validate = 1, Message = "locking Device queued" };
            }
            catch (Exception exception)
            {
                return new ResultViewModel { Validate = 0, Message = exception.ToString() };
            }


            //});
        }


        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> SendUsersOfDevice([FromBody] DeviceBasicInfo device)
        {
            return await Task.Run(() =>
            {
                try
                {

                    return new ResultViewModel { Validate = 1, Id = device.DeviceId };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = "SendUsersToDevice Failed." };
                }
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel> ReadOfflineOfDevice(uint code, DateTime? fromDate, DateTime? toDate)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return new ResultViewModel { Validate = 1, Message = "Retriving Log queued" };
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
        public Task<List<ResultViewModel>> RetrieveUserFromDevice(uint code, [FromBody] List<int> userIds)
        {
            return Task.Run(() =>
            {
                try
                {
                    return new List<ResultViewModel>
                        {new ResultViewModel {Validate = 1, Message = "Retrieving users queued"}};
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
        public async Task<ResultViewModel> DeleteUserFromDevice(uint code, [FromBody] List<int> userCodes, bool updateServerSideIdentification = false)
        {
            return await Task.Run(() =>
            {
                try
                {
                    //var creatorUser = HttpContext.GetUser();
                    var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();
                    if (device is null)
                        return new ResultViewModel { Success = false, Message = $"Device {code} does not exists." };

                    //var task = new TaskInfo
                    //{
                    //    CreatedAt = DateTimeOffset.Now,
                    //    CreatedBy = creatorUser,
                    //    TaskType = _taskTypes.DeleteUsers,
                    //    Priority = _taskPriorities.Medium,
                    //    DeviceBrand = _deviceBrands.ZkTeco,
                    //    TaskItems = new List<TaskItem>()
                    //};
                    ////var userIds = JsonConvert.DeserializeObject<List<uint>>(JsonConvert.SerializeObject(userId));

                    //foreach (var userCode in userCodes)
                    //{

                    //    task.TaskItems.Add(new TaskItem
                    //    {
                    //        Status = _taskStatuses.Queued,
                    //        TaskItemType = _taskItemTypes.DeleteUserFromTerminal,
                    //        Priority = _taskPriorities.Medium,
                    //        DeviceId = device.DeviceId,
                    //        Data = JsonConvert.SerializeObject(new { userCode }),
                    //        IsParallelRestricted = true,
                    //        IsScheduled = false,
                    //        OrderIndex = 1
                    //    });
                    //}

                    //_taskService.InsertTask(task);
                    //_taskService.ProcessQueue(_deviceBrands.ZkTeco).ConfigureAwait(false);
                    //_taskManager.ProcessQueue();

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
        public ResultViewModel<List<User>> RetrieveUsersListFromDevice([FromBody] TaskInfo task)
        {
            try
            {
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


        [HttpPost]
        [Authorize]
        public async Task<Dictionary<string, string>> GetAdditionalData([FromBody] TaskInfo task)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = (Dictionary<string, string>)_commandFactory.Factory(
                            CommandType.GetDeviceAdditionalData,
                            new List<object>
                                {task.TaskItems?.FirstOrDefault()})
                        .Execute();

                    return result;

                }
                catch (Exception)
                {
                    return new Dictionary<string, string>();
                }
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<Dictionary<uint, bool>> DeleteDevices([FromBody] List<uint> deviceIds)
        {
            return await Task.Run(() =>
            {
                var resultList = new Dictionary<uint, bool>();

                foreach (var deviceId in deviceIds)
                {
                    var device = _deviceService.GetDevice(deviceId);
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
            });
        }

        [HttpPost]
        [Authorize]
        [Route("{id:int}/DownloadAllUserPhotos")]
        public async Task<ResultViewModel> DownloadAllUserPhotos([FromRoute] int id)
        {
            return await Task.Run(() =>
            {
                var result = (ResultViewModel)_commandFactory
                    .Factory(CommandType.DownloadUserPhotos, new List<object> { id })?.Execute();
                return result;
            });
        }

        [HttpPost]
        [Authorize]
        [Route("{id:int}/UploadAllUserPhotos")]
        public async Task<ResultViewModel> UploadAllUserPhotos([FromRoute] int id, string folderPath = default)
        {
            return await Task.Run(() =>
            {
                var result = (ResultViewModel)_commandFactory
                    .Factory(CommandType.UploadUserPhotos, new List<object> { id, folderPath })?.Execute();
                return result;
            });
        }
    }
}
