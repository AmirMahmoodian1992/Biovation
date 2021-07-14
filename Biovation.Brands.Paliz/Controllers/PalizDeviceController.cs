﻿//using Biovation.Brands.Paliz.Command;
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
using DeviceBrands = Biovation.Constants.DeviceBrands;
using TaskItem = Biovation.Domain.TaskItem;

namespace Biovation.Brands.Paliz.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class PalizDeviceController : ControllerBase
    {
        //private readonly PalizServer _palizServer;
        private readonly DeviceBrands _deviceBrands;
        private readonly DeviceService _deviceService;
        private readonly AccessGroupService _accessGroupService;
        private readonly BiovationConfigurationManager _configurationManager;
        private readonly CommandFactory _commandFactory;
        private readonly Dictionary<uint, DeviceBasicInfo> _onlineDevices;

        public PalizDeviceController( DeviceService deviceService,
            AccessGroupService accessGroupService, DeviceBrands deviceBrands,
            BiovationConfigurationManager configurationManager, CommandFactory commandFactory, Dictionary<uint, DeviceBasicInfo> onlineDevices)
        {
            //_palizServer = palizServer;
            _deviceService = deviceService;
            _commandFactory = commandFactory;
            _deviceBrands = deviceBrands;
            _accessGroupService = accessGroupService;
            _configurationManager = configurationManager;
            _onlineDevices = onlineDevices;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<List<DeviceBasicInfo>> GetOnlineDevices()
        {
            return await Task.Run(() =>
            {
                var onlineDevices = new List<DeviceBasicInfo>();

                foreach (var onlineDevice in _onlineDevices)
                {
                    if (string.IsNullOrEmpty(onlineDevice.Value.Name) || onlineDevice.Value.DeviceId == 0)
                    {
                        var device = _deviceService.GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.PalizCode)
                            .FirstOrDefault();
                        if (device is null)
                            continue;

                        onlineDevice.Value.Name = device.Name;
                        onlineDevice.Value.DeviceId = device.DeviceId;
                    }

                    onlineDevices.Add(onlineDevice.Value);
                }

                return onlineDevices;
            });
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
        public async Task<ResultViewModel> RetrieveLogs([FromBody] uint code)
        {
            var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.PalizCode).FirstOrDefault();
            if (device is null)
                return new ResultViewModel {Success = false, Message = "Wrong device code is provided"};

            try
            {
                //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                //var creatorUser = HttpContext.GetUser();

                //var task = new TaskInfo
                //{
                //    CreatedAt = DateTimeOffset.Now,
                //    CreatedBy = creatorUser,
                //    TaskType = _taskTypes.GetLogs,
                //    Priority = _taskPriorities.Medium,
                //    TaskItems = new List<TaskItem>(),
                //    DeviceBrand = _deviceBrands.Paliz,
                //    DueDate = DateTime.Today
                //};

                //if (code != default)
                //    task.TaskItems.Add(new TaskItem
                //    {
                //        Status = _taskStatuses.Queued,
                //        TaskItemType = _taskItemTypes.GetLogs,
                //        Priority = _taskPriorities.Medium,
                //        DeviceId = device.DeviceId,
                //        Data = JsonConvert.SerializeObject(device.DeviceId),
                //        IsParallelRestricted = true,
                //        IsScheduled = false,
                //        OrderIndex = 1,

                //    });

                //else
                //{
                //    var palizDevices = _deviceService.GetDevices(brandId: DeviceBrands.PalizCode);
                //    foreach (var palizDevice in palizDevices)
                //    {
                //        task.TaskItems.Add(new TaskItem
                //        {
                //            Status = _taskStatuses.Queued,
                //            TaskItemType = _taskItemTypes.GetLogs,
                //            Priority = _taskPriorities.Medium,
                //            DeviceId = palizDevice.DeviceId,
                //            Data = JsonConvert.SerializeObject(device.DeviceId),
                //            IsParallelRestricted = true,
                //            IsScheduled = false,
                //            OrderIndex = 1
                //        });
                //    }
                //}

                //_taskService.InsertTask(task);
                //await _taskService.ProcessQueue(_deviceBrands.Paliz).ConfigureAwait(false);

                return new ResultViewModel {Validate = 1, Message = "Retrieving Log queued"};
            }
            catch (Exception exception)
            {
                return new ResultViewModel {Validate = 0, Message = exception.ToString()};
            }
        }


        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel> ReadOfflineOfDevice(uint code, DateTime? fromDate, DateTime? toDate)
        {

            try
            {
                var creatorUser = HttpContext.GetUser();
                var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.PalizCode).FirstOrDefault();
                if (device is null)
                    return new ResultViewModel {Success = false, Message = "Wrong device code is provided"};

                try
                {
                    //if (fromDate.HasValue && toDate.HasValue)
                    //{
                    //    var task = new TaskInfo
                    //    {
                    //        CreatedAt = DateTimeOffset.Now,
                    //        CreatedBy = creatorUser,
                    //        TaskType = _taskTypes.GetLogsInPeriod,
                    //        Priority = _taskPriorities.Medium,
                    //        TaskItems = new List<TaskItem>(),
                    //        DeviceBrand = _deviceBrands.Paliz,
                    //        DueDate = DateTimeOffset.Now
                    //    };

                    //    if (code != default)
                    //        task.TaskItems.Add(new TaskItem
                    //        {
                    //            Status = _taskStatuses.Queued,
                    //            TaskItemType = _taskItemTypes.GetLogsInPeriod,
                    //            Priority = _taskPriorities.Medium,
                    //            DeviceId = device.DeviceId,
                    //            Data = JsonConvert.SerializeObject(new {fromDate, toDate}),
                    //            IsParallelRestricted = true,
                    //            IsScheduled = false,
                    //            OrderIndex = 1,
                    //        });

                    //    else
                    //    {
                    //        var palizDevices = _deviceService.GetDevices(brandId: DeviceBrands.PalizCode);
                    //        foreach (var palizDevice in palizDevices)
                    //        {
                    //            task.TaskItems.Add(new TaskItem
                    //            {
                    //                Status = _taskStatuses.Queued,
                    //                TaskItemType = _taskItemTypes.GetLogsInPeriod,
                    //                Priority = _taskPriorities.Medium,
                    //                DeviceId = palizDevice.DeviceId,
                    //                Data = JsonConvert.SerializeObject(new {fromDate, toDate}),
                    //                IsParallelRestricted = true,
                    //                IsScheduled = false,
                    //                OrderIndex = 1
                    //            });
                    //        }
                    //    }

                    //    _taskService.InsertTask(task);
                    //    await _taskService.ProcessQueue(_deviceBrands.Paliz).ConfigureAwait(false);

                    //}
                    //else
                    //{
                    //    var task = new TaskInfo
                    //    {
                    //        CreatedAt = DateTimeOffset.Now,
                    //        CreatedBy = creatorUser,
                    //        TaskType = _taskTypes.GetLogs,
                    //        Priority = _taskPriorities.Medium,
                    //        TaskItems = new List<TaskItem>(),
                    //        DeviceBrand = _deviceBrands.Paliz,
                    //        DueDate = DateTime.Today
                    //    };

                    //    if (code != default)
                    //        task.TaskItems.Add(new TaskItem
                    //        {
                    //            Status = _taskStatuses.Queued,
                    //            TaskItemType = _taskItemTypes.GetLogs,
                    //            Priority = _taskPriorities.Medium,
                    //            DeviceId = device.DeviceId,
                    //            Data = JsonConvert.SerializeObject(device.DeviceId),
                    //            IsParallelRestricted = true,
                    //            IsScheduled = false,
                    //            OrderIndex = 1,
                    //        });

                    //    else
                    //    {
                    //        var palizDevices =
                    //            _deviceService.GetDevices(brandId: DeviceBrands.PalizCode);
                    //        foreach (var palizDevice in palizDevices)
                    //        {
                    //            task.TaskItems.Add(new TaskItem
                    //            {
                    //                Status = _taskStatuses.Queued,
                    //                TaskItemType = _taskItemTypes.GetLogs,
                    //                Priority = _taskPriorities.Medium,
                    //                DeviceId = palizDevice.DeviceId,
                    //                Data = JsonConvert.SerializeObject(palizDevice.DeviceId),
                    //                IsParallelRestricted = true,
                    //                IsScheduled = false,
                    //                OrderIndex = 1
                    //            });
                    //        }
                    //    }

                    //    _taskService.InsertTask(task);
                    //    await _taskService.ProcessQueue(_deviceBrands.Paliz).ConfigureAwait(false);
                    //}

                    return new ResultViewModel {Validate = 1, Message = "Retrieving Log queued"};
                }
                catch (Exception)
                {
                    return new ResultViewModel {Validate = 0, Id = code, Message = "Retrieving Log not queued"};
                }

                //int deviceId = devices.FirstOrDefault(dev => dev.Code == code).DeviceId;
            }

            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel {Validate = 0, Id = code, Message = "Retrieving Log not queued"};
            }
        }
        /*
        [HttpGet]
        public ResultViewModel ReadOfflineOfDevice(uint code, DateTime? fromDate, DateTime? toDate)
        {
            var devices =_deviceService.GetDevices(code: code, brandId: DeviceBrands.PalizCode).FirstOrDefault();
            var deviceId = devices.DeviceId;
            try
            {
                ResultViewModel result;
                if (fromDate.HasValue && toDate.HasValue)
                {
                    var retrieveLogsOfDeviceInPeriodCommand = CommandFactory.Factory(CommandType.RetrieveLogsOfDeviceInPeriod,
                       new List<object> { code, fromDate, toDate });
                    result = (ResultViewModel)retrieveLogsOfDeviceInPeriodCommand.Execute();
                    return result;
                }
                var retrieveLogsOfDeviceCommand = CommandFactory.Factory(CommandType.RetrieveAllLogsOfDevice,
                      new List<object> { code});
                result = (ResultViewModel)retrieveLogsOfDeviceCommand.Execute();
                return result;

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = code, Message = "0" };
            }
        }
        */

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> LockDevice([FromBody] uint code)
        {
            try
            {
                //var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.PalizCode).FirstOrDefault();

                ////var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                //var creatorUser = HttpContext.GetUser();


                //var task = new TaskInfo
                //{
                //    CreatedAt = DateTimeOffset.Now,
                //    CreatedBy = creatorUser,
                //    TaskType = _taskTypes.LockDevice,
                //    Priority = _taskPriorities.Medium,
                //    DeviceBrand = _deviceBrands.Paliz,
                //    TaskItems = new List<TaskItem>(),
                //    DueDate = DateTime.Today
                //};

                //task.TaskItems.Add(new TaskItem
                //{
                //    Status = _taskStatuses.Queued,
                //    TaskItemType = _taskItemTypes.LockDevice,
                //    Priority = _taskPriorities.Medium,
                //    DeviceId = devices.DeviceId,
                //    Data = JsonConvert.SerializeObject(devices.DeviceId),
                //    IsParallelRestricted = true,
                //    IsScheduled = false,
                //    OrderIndex = 1
                //});

                //_taskService.InsertTask(task);
                //await _taskService.ProcessQueue(_deviceBrands.Paliz, devices.DeviceId).ConfigureAwait(false);

                return new ResultViewModel {Validate = 1, Message = "locking Device queued"};
            }
            catch (Exception exception)
            {
                return new ResultViewModel {Validate = 0, Message = exception.ToString()};
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> UnlockDevice([FromBody] uint code)
        {
            try
            {
                //var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.PalizCode).FirstOrDefault();

                ////var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                //var creatorUser = HttpContext.GetUser();

                //var task = new TaskInfo
                //{
                //    CreatedAt = DateTimeOffset.Now,
                //    CreatedBy = creatorUser,
                //    TaskType = _taskTypes.UnlockDevice,
                //    Priority = _taskPriorities.Medium,
                //    DeviceBrand = _deviceBrands.Paliz,
                //    TaskItems = new List<TaskItem>(),
                //    DueDate = DateTime.Today
                //};

                //task.TaskItems.Add(new TaskItem
                //{
                //    Status = _taskStatuses.Queued,
                //    TaskItemType = _taskItemTypes.UnlockDevice,
                //    Priority = _taskPriorities.Medium,
                //    DeviceId = devices.DeviceId,
                //    Data = JsonConvert.SerializeObject(devices.DeviceId),
                //    IsParallelRestricted = true,
                //    IsScheduled = false,
                //    OrderIndex = 1,
                //    TotalCount = 1,
                //    CurrentIndex = 0
                //});

                //_taskService.InsertTask(task);
                //await _taskService.ProcessQueue(_deviceBrands.Paliz, devices.DeviceId).ConfigureAwait(false);
                return new ResultViewModel {Validate = 1, Message = "Unlocking Device queued"};
            }
            catch (Exception exception)
            {
                return new ResultViewModel {Validate = 0, Message = exception.ToString()};
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            if (device.Active)
            {
                return await Task.Run(() =>
                {
                    try
                    {
                        return new ResultViewModel { Validate = 1, Message = "Unlocking Device queued" };
                    }
                    catch (Exception exception)
                    {
                        return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                    }
                });
            }

            if (_configurationManager.LockDevice)
            {
                return await Task.Run(() =>
                {
                    try
                    {
                        return new ResultViewModel { Validate = 1, Message = "locking Device queued" };
                    }
                    catch (Exception exception)
                    {
                        return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                    }
                });
            }

            return new ResultViewModel { Validate = 0, Message = "The LockDevice option is False" };
        }

        /*
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
                                        new List<object> { device.Code, userGroupMember.UserId });
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
        */
        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> SendUsersOfDevice([FromBody] DeviceBasicInfo device)
        {
            return await Task.Run(() =>
            {
                try
                {
                    //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();

                    return new ResultViewModel { Validate = 1, Message = "Sending users queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<List<ResultViewModel>> RetrieveUserFromDevice(uint code, [FromBody] List<int> userIds)
        {
            return await Task.Run(() =>
            {
                try
                {
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

        /*
        [HttpGet]
        public List<ResultViewModel> RetrieveUserFromDevice(uint code, string userId)
        {
            var userIds = JsonConvert.DeserializeObject<int[]>(userId);
            var result = new List<ResultViewModel>();
            foreach (var id in userIds)
            {
            var retrieveUserFromTerminalCommand = CommandFactory.Factory(CommandType.RetrieveUserFromDevice,
                    new List<object> { code, id });

                var commandResult = (bool)retrieveUserFromTerminalCommand.Execute();

                result.Add(new ResultViewModel { Id = id, Validate = commandResult ? 1 : 0, Message = "" });
            }
            }

            return result;
        }

        */
           [HttpGet]
        [Authorize]
        public ResultViewModel<List<User>> RetrieveUsersListFromDevice([FromBody]TaskInfo task)
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
        public async Task<bool> OpenDoorTerminal(uint code)
        {
            try
            {  //TODO Should be task on BiovationServer
                //var creatorUser = HttpContext.GetUser();
                //var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.PalizCode).FirstOrDefault();

                //var task = new TaskInfo
                //{
                //    CreatedAt = DateTimeOffset.Now,
                //    CreatedBy = creatorUser,
                //    TaskType = _taskTypes.OpenDoor,
                //    Priority = _taskPriorities.Medium,
                //    TaskItems = new List<TaskItem>(),
                //    DeviceBrand = _deviceBrands.Paliz,
                //    DueDate = DateTime.Today
                //};

                //task.TaskItems.Add(new TaskItem
                //{
                //    Status = _taskStatuses.Queued,
                //    TaskItemType = _taskItemTypes.OpenDoor,
                //    Priority = _taskPriorities.Medium,
                //    DeviceId = devices.DeviceId,
                //    Data = JsonConvert.SerializeObject(devices.DeviceId),
                //    IsParallelRestricted = true,
                //    IsScheduled = false,
                //    OrderIndex = 1,
                //    TotalCount = 1,
                //    CurrentIndex = 0

                //});

                //_taskService.InsertTask(task);
                //await _taskService.ProcessQueue(_deviceBrands.Paliz).ConfigureAwait(false);

                var result = new ResultViewModel {Validate = 1, Message = "Unlocking Device queued"};
                if (result.Validate == 1)
                {
                    return true;
                }

                return false;

            }
            catch (Exception)
            {
                return false;
            }




            /*  var openDoorToTerminalCommand = CommandFactory.Factory(CommandType.OpenDoor,
                  new List<object> { code });

              var result = openDoorToTerminalCommand.Execute();

              return (bool)result;*/
        }

        //Todo: change for .net core
        //[HttpPost]
        //public Task<ResultViewModel> UpgradeFirmware(int deviceCode)
        //{
        //    return Task.Run(async () =>
        //    {
        //        var devices = _deviceService.GetDeviceBasicInfoWithCode((uint)deviceCode, DeviceBrands.PalizCode);
        ////        var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
        //        var creatorUser = HttpContext.GetUser();
        //        if (!Request.Content.IsMimeMultipartContent())
        //            throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

        //        if (_palizServer.GetOnlineDevices().Values.All(device => device.Code != deviceCode))
        //            return new ResultViewModel { Validate = 0, Code = 400, Id = deviceCode, Message = $"The device {deviceCode} is not online." };

        //        //var tempFolderPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "TempFiles");
        //        var tempFolderPath = Path.Combine(Path.GetTempPath(), "Biovation");
        //        if (!Directory.Exists(tempFolderPath))
        //            Directory.CreateDirectory(tempFolderPath);

        //        var provider = new MultipartFormDataStreamProvider(tempFolderPath);

        //        var result = await Request.Content.ReadAsMultipartAsync(provider).ContinueWith(o =>
        //        {
        //            if (provider.FileData is null || provider.FileData.Count == 0)
        //                return new ResultViewModel { Validate = 0, Code = 400, Id = deviceCode, Message = "No files uploaded." };

        //            var binaryFirmwareFiles =
        //                provider.FileData.Where(fileData =>
        //                    fileData.Headers.ContentType.MediaType == MediaTypeNames.Application.Octet).ToList();

        //            if (binaryFirmwareFiles.Count == 0)
        //                return new ResultViewModel { Validate = 0, Code = 400, Id = deviceCode, Message = "No compatible .bin files uploaded, please check the uploaded firmware files." };

        //            try
        //            {
        //                foreach (var multipartFileData in binaryFirmwareFiles)
        //                {


        //                    var task = new TaskInfo
        //                    {
        //                        CreatedAt = DateTimeOffset.Now,
        //                        CreatedBy = creatorUser,

        //                        TaskType = _taskTypes.UpgradeDeviceFirmware,
        //                        Priority = _taskPriorities.Medium,
        //                        DeviceBrand = DeviceBrands.Paliz,
        //                        TaskItems = new List<TaskItem>()
        //                    };
        //                    task.TaskItems.Add(new TaskItem
        //                    {
        //                        Status = _taskStatuses.Queued,

        //                        TaskItemType = _taskItemTypes.UpgradeDeviceFirmware,
        //                        Priority = _taskPriorities.Medium,
        //                        DueDate = DateTime.Today,
        //                        DeviceId = devices.DeviceId,
        //                        Data = JsonConvert.SerializeObject(new { multipartFileData.LocalFileName }),
        //                        IsParallelRestricted = true,
        //                        IsScheduled = false,
        //                        OrderIndex = 1
        //                    });
        //                    _taskService.InsertTask(task);
        //                    _taskManager.ProcessQueue();
        //                    return new ResultViewModel { Validate = 1, Message = "Upgrading Device queued" };
        //                }

        //                return new ResultViewModel { Validate = 1, Code = 200, Id = deviceCode, Message = "Files uploaded and upgrading firmware started." };
        //            }
        //            catch (Exception exception)
        //            {
        //                Logger.Log(exception);
        //                return new ResultViewModel { Validate = 0, Code = 500, Id = deviceCode, Message = $"An error occured: {exception.Message}" };
        //            }
        //        }
        //        );

        //        return result;
        //    });
        //}

        /* public Task<ResultViewModel> UpgradeFirmware(int deviceCode)
         {
             return Task.Run(async () =>
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                if (_palizServer.GetOnlineDevices().Values.All(device => device.Code != deviceCode))
                    return new ResultViewModel { Validate = 0, Code = 400, Id = deviceCode, Message = $"The device {deviceCode} is not online." };

                //var tempFolderPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "TempFiles");
                var tempFolderPath = Path.Combine(Path.GetTempPath(), "Biovation");
                if (!Directory.Exists(tempFolderPath))
                    Directory.CreateDirectory(tempFolderPath);

                var provider = new MultipartFormDataStreamProvider(tempFolderPath);

                var result = await Request.Content.ReadAsMultipartAsync(provider).ContinueWith(o =>
                    {
                        if (provider.FileData is null || provider.FileData.Count == 0)
                            return new ResultViewModel { Validate = 0, Code = 400, Id = deviceCode, Message = "No files uploaded." };

                        var binaryFirmwareFiles =
                            provider.FileData.Where(fileData =>
                                fileData.Headers.ContentType.MediaType == MediaTypeNames.Application.Octet).ToList();

                        if (binaryFirmwareFiles.Count == 0)
                            return new ResultViewModel { Validate = 0, Code = 400, Id = deviceCode, Message = "No compatible .bin files uploaded, please check the uploaded firmware files." };

                        try
                        {
                            foreach (var multipartFileData in binaryFirmwareFiles)
                            {
                                var upgradeDeviceFirmwareCommand = CommandFactory.Factory(CommandType.UpgradeFirmware,
                                    new List<object> { deviceCode, multipartFileData.LocalFileName });

                                var commandResult = (ResultViewModel)upgradeDeviceFirmwareCommand.Execute();
                                if (commandResult.Validate == 0)
                                    return commandResult;
                            }

                            return new ResultViewModel { Validate = 1, Code = 200, Id = deviceCode, Message = "Files uploaded and upgrading firmware started." };
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception);
                            return new ResultViewModel { Validate = 0, Code = 500, Id = deviceCode, Message = $"An error occured: {exception.Message}" };
                        }
                    }
                );

                return result;
            });
         }
     */


        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> DeleteUserFromDevice(uint code, [FromBody] List<int> userCodes,
            bool updateServerSideIdentification = false)
        {
            return await Task.Run(() =>
            {
                try
                {
                    //var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.VirdiCode).FirstOrDefault();

                    ////var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                    //var creatorUser = HttpContext.GetUser();

                    /*var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.DeleteUserFromTerminal,
                        Priority = _taskPriorities.Medium,
                        TaskItems = new List<TaskItem>()
                    };*/
                    //var task = new TaskInfo
                    //{
                    //    CreatedAt = DateTimeOffset.Now,
                    //    CreatedBy = creatorUser,
                    //    TaskType = _taskTypes.DeleteUsers,
                    //    Priority = _taskPriorities.Medium,
                    //    DeviceBrand = _deviceBrands.Virdi,
                    //    TaskItems = new List<TaskItem>(),
                    //    DueDate = DateTime.Today
                    //};

                    ////var userIds = JsonConvert.DeserializeObject<int[]>(userId.ToString());
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
                    //        OrderIndex = 1,
                    //        CurrentIndex = 0,
                    //        TotalCount = 1
                    //    });
                    //}

                    //_taskService.InsertTask(task);
                    //await _taskService.ProcessQueue(_deviceBrands.Virdi).ConfigureAwait(false);


                    //if (updateServerSideIdentification)
                    //{
                    //    foreach (var id in userCodes)
                    //    {
                    //        _palizeServer.DeleteUserFromDeviceFastSearch(code, id);
                    //    }
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
    }
}
