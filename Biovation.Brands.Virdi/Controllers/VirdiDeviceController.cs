﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Biovation.Brands.Virdi.Command;
using Biovation.Brands.Virdi.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DeviceBrands = Biovation.CommonClasses.Models.ConstantValues.DeviceBrands;
using TaskItem = Biovation.CommonClasses.Models.TaskItem;

namespace Biovation.Brands.Virdi.Controllers
{
    //[Route("Biovation/Api/Virdi/{controller}/{action}", Name = "VirdiDevice")]
    [Route("Biovation/Api/[controller]/[action]")]
    public class VirdiDeviceController : Controller
    {
        private readonly Callbacks _callbacks;
        private readonly VirdiServer _virdiServer;
        private readonly TaskService _taskService;
        private readonly TaskManager _taskManager;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly CommandFactory _commandFactory;
        private readonly AccessGroupService _accessGroupService;

        public VirdiDeviceController(TaskService taskService, UserService userService, DeviceService deviceService, VirdiServer virdiServer, Callbacks callbacks, AccessGroupService accessGroupService, CommandFactory commandFactory, TaskManager taskManager)
        {
            _callbacks = callbacks;
            _virdiServer = virdiServer;
            _taskService = taskService;
            _userService = userService;
            _deviceService = deviceService;
            _commandFactory = commandFactory;
            _taskManager = taskManager;
            _accessGroupService = accessGroupService;
        }

        [HttpGet]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            var onlineDevices = new List<DeviceBasicInfo>();

            foreach (var onlineDevice in _virdiServer.GetOnlineDevices())
            {
                if (string.IsNullOrEmpty(onlineDevice.Value.Name) || onlineDevice.Value.DeviceId == 0)
                {
                    var device = _deviceService.GetDeviceBasicInfoWithCode(onlineDevice.Key, DeviceBrands.VirdiCode);
                    onlineDevice.Value.Name = device.Name;
                    onlineDevice.Value.DeviceId = device.DeviceId;
                }
                onlineDevices.Add(onlineDevice.Value);
            }

            return onlineDevices;
        }
        /*
       [HttpPost]

       public ResultViewModel RetrieveLogs([FromBody]uint code)
       {
           var retrieveAllLogsCommand = CommandFactory.Factory(CommandType.RetrieveAllLogsOfDevice,
               new List<object> { code });

           retrieveAllLogsCommand.Execute();

           return new ResultViewModel { Validate = 0 };
       }
       */

        [HttpPost]
        public Task<ResultViewModel> RetrieveLogs([FromBody]uint code)
        {
            return Task.Run(() =>
            {

                var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.VirdiCode);
                int deviceId = devices.DeviceId;
                //int deviceId = devices.FirstOrDefault(dev => dev.Code == code).DeviceId;
                try
                {
                    var creatorUser = _userService.GetUser(123456789, false);

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.GetServeLogs,
                        Priority = TaskPriorities.Medium,
                        TaskItems = new List<TaskItem>(),
                        DeviceBrand = DeviceBrands.Virdi,
                    };

                    if (deviceId != default)
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = TaskStatuses.Queued,
                            TaskItemType = TaskItemTypes.GetServeLogs,
                            Priority = TaskPriorities.Medium,
                            DueDate = DateTime.Today,
                            DeviceId = deviceId,
                            Data = JsonConvert.SerializeObject(deviceId),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,

                        });

                    else
                    {
                        var virdidevices = _deviceService.GetAllDevicesBasicInfosByBrandId(DeviceBrands.VirdiCode);
                        foreach (var device in virdidevices)
                        {
                            task.TaskItems.Add(new TaskItem
                            {
                                Status = TaskStatuses.Queued,
                                TaskItemType = TaskItemTypes.GetServeLogs,
                                Priority = TaskPriorities.Medium,
                                DueDate = DateTime.Today,
                                DeviceId = device.DeviceId,
                                Data = JsonConvert.SerializeObject(deviceId),
                                IsParallelRestricted = true,
                                IsScheduled = false,
                                OrderIndex = 1
                            });
                        }
                    }

                    _taskService.InsertTask(task).Wait();
                    _taskManager.ProcessQueue();

                    return new ResultViewModel { Validate = 1, Message = "Retriving Log queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }


        [HttpGet]
        public Task<ResultViewModel> ReadOfflineOfDevice(uint code, DateTime? fromDate, DateTime? toDate)
        {
            return Task.Run(() =>
            {
                try
                {
                    var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.VirdiCode);
                    var deviceId = devices.DeviceId;
                    var creatorUser = _userService.GetUser(123456789, false);
                    try
                    {
                        if (fromDate.HasValue && toDate.HasValue)
                        {
                            var task = new TaskInfo
                            {
                                CreatedAt = DateTimeOffset.Now,
                                CreatedBy = creatorUser,
                                TaskType = TaskTypes.GetServeLogs,
                                Priority = TaskPriorities.Medium,
                                TaskItems = new List<TaskItem>(),
                                DeviceBrand = DeviceBrands.Virdi,

                            };

                            if (deviceId != default)
                                task.TaskItems.Add(new TaskItem
                                {
                                    Status = TaskStatuses.Queued,
                                    TaskItemType = TaskItemTypes.GetServeLogsInPeriod,
                                    Priority = TaskPriorities.Medium,
                                    DueDate = DateTimeOffset.Now,
                                    DeviceId = deviceId,
                                    Data = JsonConvert.SerializeObject(new { fromDate, toDate }),
                                    IsParallelRestricted = true,
                                    IsScheduled = false,
                                    OrderIndex = 1,

                                });

                            else
                            {
                                var virdidevices =
                                    _deviceService.GetAllDevicesBasicInfosByBrandId(DeviceBrands.VirdiCode);
                                foreach (var device in virdidevices)
                                {
                                    task.TaskItems.Add(new TaskItem
                                    {
                                        Status = TaskStatuses.Queued,
                                        TaskItemType = TaskItemTypes.GetServeLogsInPeriod,
                                        Priority = TaskPriorities.Medium,
                                        DueDate = DateTime.Today,
                                        DeviceId = device.DeviceId,
                                        Data = JsonConvert.SerializeObject(new { fromDate, toDate }),
                                        IsParallelRestricted = true,
                                        IsScheduled = false,
                                        OrderIndex = 1
                                    });
                                }
                            }

                            _taskService.InsertTask(task).Wait();
                            _taskManager.ProcessQueue();
                        }
                        else
                        {
                            var task = new TaskInfo
                            {
                                CreatedAt = DateTimeOffset.Now,
                                CreatedBy = creatorUser,
                                TaskType = TaskTypes.GetServeLogs,
                                Priority = TaskPriorities.Medium,
                                TaskItems = new List<TaskItem>(),
                                DeviceBrand = DeviceBrands.Virdi,
                            };

                            if (deviceId != default)
                                task.TaskItems.Add(new TaskItem
                                {
                                    Status = TaskStatuses.Queued,
                                    TaskItemType = TaskItemTypes.GetServeLogs,
                                    Priority = TaskPriorities.Medium,
                                    DueDate = DateTime.Today,
                                    DeviceId = deviceId,
                                    Data = JsonConvert.SerializeObject(deviceId),
                                    IsParallelRestricted = true,
                                    IsScheduled = false,
                                    OrderIndex = 1,

                                });

                            else
                            {
                                var virdidevices =
                                    _deviceService.GetAllDevicesBasicInfosByBrandId(DeviceBrands.VirdiCode);
                                foreach (var device in virdidevices)
                                {
                                    task.TaskItems.Add(new TaskItem
                                    {
                                        Status = TaskStatuses.Queued,
                                        TaskItemType = TaskItemTypes.GetServeLogs,
                                        Priority = TaskPriorities.Medium,
                                        DueDate = DateTime.Today,
                                        DeviceId = device.DeviceId,
                                        Data = JsonConvert.SerializeObject(deviceId),
                                        IsParallelRestricted = true,
                                        IsScheduled = false,
                                        OrderIndex = 1
                                    });
                                }
                            }

                            _taskService.InsertTask(task).Wait();
                            _taskManager.ProcessQueue();
                        }

                        return new ResultViewModel { Validate = 1, Message = "Retriving Log queued" };
                    }
                    catch (Exception exception)
                    {
                        return new ResultViewModel { Validate = 0, Id = code, Message = "Retriving Log not queued" };

                    }
                    //int deviceId = devices.FirstOrDefault(dev => dev.Code == code).DeviceId;
                }

                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Id = code, Message = "Retriving Log not queued" };
                }
            });
        }
        /*
        [HttpGet]
        public ResultViewModel ReadOfflineOfDevice(uint code, DateTime? fromDate, DateTime? toDate)
        {
            var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.VirdiCode);
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
        public Task<ResultViewModel> LockDevice([FromBody]uint code)
        {
            return Task.Run(() =>
            {
                try
                {
                    var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.VirdiCode);

                    var creatorUser = _userService.GetUser(123456789, false);

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.LockDevice,
                        Priority = TaskPriorities.Medium,
                        DeviceBrand = DeviceBrands.Virdi,
                        TaskItems = new List<TaskItem>()
                    };
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = TaskStatuses.Queued,
                        TaskItemType = TaskItemTypes.LockDevice,
                        Priority = TaskPriorities.Medium,
                        DueDate = DateTime.Today,
                        DeviceId = devices.DeviceId,

                        Data = JsonConvert.SerializeObject(devices.DeviceId),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1
                    });

                    _taskService.InsertTask(task).Wait();
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
        public Task<ResultViewModel> UnlockDevice([FromBody]uint code)
        {
            return Task.Run(() =>
            {
                try
                {
                    var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.VirdiCode);

                    var creatorUser = _userService.GetUser(123456789, false);

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.UnlockDevice,
                        Priority = TaskPriorities.Medium,
                        TaskItems = new List<TaskItem>()
                    };
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = TaskStatuses.Queued,
                        TaskItemType = TaskItemTypes.UnlockDevice,
                        Priority = TaskPriorities.Medium,
                        DueDate = DateTime.Today,
                        DeviceId = devices.DeviceId,
                        Data = JsonConvert.SerializeObject(devices.DeviceId),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1,

                    });
                    _taskService.InsertTask(task).Wait();
                    _taskManager.ProcessQueue();
                    return new ResultViewModel { Validate = 1, Message = "Unlocking Device queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });


        }

        [HttpPost]
        public Task<ResultViewModel> ModifyDevice([FromBody]DeviceBasicInfo device)
        {

            var devices = _deviceService.GetDeviceBasicInfoWithCode(device.Code, DeviceBrands.VirdiCode);
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
                            DeviceBrand = DeviceBrands.Virdi,
                            TaskType = TaskTypes.UnlockDevice,
                            Priority = TaskPriorities.Medium,
                            TaskItems = new List<TaskItem>()
                        };
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = TaskStatuses.Queued,
                            TaskItemType = TaskItemTypes.UnlockDevice,
                            Priority = TaskPriorities.Medium,
                            DueDate = DateTime.Today,
                            DeviceId = devices.DeviceId,
                            Data = JsonConvert.SerializeObject(devices.DeviceId),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1
                        });
                        _taskService.InsertTask(task).Wait();
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
                        TaskType = TaskTypes.LockDevice,
                        Priority = TaskPriorities.Medium,
                        TaskItems = new List<TaskItem>(),
                        DeviceBrand = DeviceBrands.Virdi,
                    };
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = TaskStatuses.Queued,
                        TaskItemType = TaskItemTypes.LockDevice,
                        Priority = TaskPriorities.Medium,
                        DueDate = DateTime.Today,
                        DeviceId = devices.DeviceId,
                        Data = JsonConvert.SerializeObject(devices.DeviceId),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1,

                    });
                    _taskService.InsertTask(task).Wait();
                    _taskManager.ProcessQueue();
                    return new ResultViewModel { Validate = 1, Message = "locking Device queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
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
        public Task<ResultViewModel> SendUsersOfDevice([FromBody]DeviceBasicInfo device)
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
                        DeviceBrand = DeviceBrands.Virdi,
                        TaskType = TaskTypes.SendUsers,
                        Priority = TaskPriorities.Medium,
                        TaskItems = new List<TaskItem>()
                    };
                    var accessGroups = _accessGroupService.GetAccessGroupsOfDevice((uint)device.DeviceId);

                    foreach (var accessGroup in accessGroups)
                    {
                        foreach (var userGroup in accessGroup.UserGroup)
                        {
                            foreach (var userGroupMember in userGroup.Users)
                            {
                                task.TaskItems.Add(new TaskItem
                                {
                                    Status = TaskStatuses.Queued,
                                    TaskItemType = TaskItemTypes.SendUser,
                                    Priority = TaskPriorities.Medium,
                                    DueDate = DateTime.Today,
                                    DeviceId = device.DeviceId,
                                    Data = JsonConvert.SerializeObject(new { userId = userGroupMember.UserId }),
                                    IsParallelRestricted = true,

                                    IsScheduled = false,
                                    OrderIndex = 1
                                });
                            }
                        }
                    }

                    _taskService.InsertTask(task).Wait();
                    _taskManager.ProcessQueue();

                    return new ResultViewModel { Validate = 1, Message = "Sending users queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
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
                        DeviceBrand = DeviceBrands.Virdi,
                        TaskType = TaskTypes.RetrieveUserFromTerminal,
                        Priority = TaskPriorities.Medium,
                        TaskItems = new List<TaskItem>()
                    };
                    var userIds = JsonConvert.DeserializeObject<int[]>(userId.ToString());
                    //int[] userIds =new[] {Convert.ToInt32(userId)};
                    var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.VirdiCode);
                    var deviceId = devices.DeviceId;
                    foreach (var id in userIds)
                    {

                        task.TaskItems.Add(new TaskItem
                        {
                            Status = TaskStatuses.Queued,
                            TaskItemType = TaskItemTypes.RetrieveUserFromTerminal,
                            Priority = TaskPriorities.Medium,
                            DueDate = DateTime.Today,
                            DeviceId = deviceId,
                            Data = JsonConvert.SerializeObject(new { userId = id }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,

                        });

                    }

                    _taskService.InsertTask(task).Wait();
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
        public ResultViewModel<List<User>> RetrieveUsersListFromDevice(uint code)
        {
            //this action return list of user so for task based this we need to syncron web and change return type for task manager statustask update

            try
            {
                var result = new ResultViewModel<List<User>>();
                var creatorUser = _userService.GetUser(123456789, false);
                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = TaskTypes.RetrieveAllUsersFromTerminal,
                    Priority = TaskPriorities.Medium,
                    DeviceBrand = DeviceBrands.Virdi,
                    TaskItems = new List<TaskItem>()
                };
                var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.VirdiCode);
                var deviceId = devices.DeviceId;
                task.TaskItems.Add(new TaskItem
                {
                    Status = TaskStatuses.Queued,
                    TaskItemType = TaskItemTypes.RetrieveAllUsersFromTerminal,
                    Priority = TaskPriorities.Medium,
                    DueDate = DateTime.Today,
                    DeviceId = deviceId,
                    Data = JsonConvert.SerializeObject(deviceId),
                    IsParallelRestricted = true,
                    IsScheduled = false,
                    OrderIndex = 1,

                });


                result = (ResultViewModel<List<User>>)_commandFactory.Factory(CommandType.RetrieveUsersListFromDevice,
                     new List<object> { task.TaskItems.FirstOrDefault().DeviceId, task.TaskItems.FirstOrDefault().Id }).Execute();


                return result;
            }

            catch (Exception exception)
            {

                return new ResultViewModel<List<User>> { Validate = 0, Message = exception.ToString() };
            }


        }


        [HttpPost]
        public Task<bool> OpenDoorTerminal(uint code)
        {

            return Task.Run(() =>
            {
                try
                {
                    var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.VirdiCode);

                    var creatorUser = _userService.GetUser(123456789, false);

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.OpenDoor,
                        Priority = TaskPriorities.Medium,
                        TaskItems = new List<TaskItem>(),
                        DeviceBrand = DeviceBrands.Virdi,
                    };
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = TaskStatuses.Queued,
                        TaskItemType = TaskItemTypes.OpenDoor,
                        Priority = TaskPriorities.Medium,
                        DueDate = DateTime.Today,
                        DeviceId = devices.DeviceId,
                        Data = JsonConvert.SerializeObject(devices.DeviceId),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1,

                    });
                    _taskService.InsertTask(task).Wait();
                    _taskManager.ProcessQueue();

                    var result = new ResultViewModel { Validate = 1, Message = "Unlocking Device queued" };
                    if (result.Validate == 1)
                    {
                        return true;
                    }
                    return false;

                }
                catch (Exception exception)
                {
                    return false;
                }
            });




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
        //        var devices = _deviceService.GetDeviceBasicInfoWithCode((uint)deviceCode, DeviceBrands.VirdiCode);
        //        var creatorUser = _userService.GetUser(123456789, false);
        //        if (!Request.Content.IsMimeMultipartContent())
        //            throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

        //        if (_virdiServer.GetOnlineDevices().Values.All(device => device.Code != deviceCode))
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

        //                        TaskType = TaskTypes.UpgradeDeviceFirmware,
        //                        Priority = TaskPriorities.Medium,
        //                        DeviceBrand = DeviceBrands.Virdi,
        //                        TaskItems = new List<TaskItem>()
        //                    };
        //                    task.TaskItems.Add(new TaskItem
        //                    {
        //                        Status = TaskStatuses.Queued,

        //                        TaskItemType = TaskItemTypes.UpgradeDeviceFirmware,
        //                        Priority = TaskPriorities.Medium,
        //                        DueDate = DateTime.Today,
        //                        DeviceId = devices.DeviceId,
        //                        Data = JsonConvert.SerializeObject(new { multipartFileData.LocalFileName }),
        //                        IsParallelRestricted = true,
        //                        IsScheduled = false,
        //                        OrderIndex = 1
        //                    });
        //                    _taskService.InsertTask(task).Wait();
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

                if (_virdiServer.GetOnlineDevices().Values.All(device => device.Code != deviceCode))
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
        public Task<ResultViewModel> DeleteUserFromDevice(uint code, [FromBody]JArray userId, bool updateServerSideIdentification = false)
        {
            return Task.Run(() =>
            {
                try
                {
                    var device = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.VirdiCode);

                    var creatorUser = _userService.GetUser(123456789, false);

                    /*var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.DeleteUserFromTerminal,
                        Priority = TaskPriorities.Medium,
                        TaskItems = new List<TaskItem>()
                    };*/
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,

                        TaskType = TaskTypes.DeleteUsers,
                        Priority = TaskPriorities.Medium,
                        DeviceBrand = DeviceBrands.Virdi,
                        TaskItems = new List<TaskItem>()
                    };


                    var userIds = JsonConvert.DeserializeObject<int[]>(userId.ToString());
                    foreach (var id in userIds)
                    {

                        task.TaskItems.Add(new TaskItem
                        {
                            Status = TaskStatuses.Queued,
                            TaskItemType = TaskItemTypes.DeleteUserFromTerminal,
                            Priority = TaskPriorities.Medium,
                            DueDate = DateTime.Today,
                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(new { userId = id }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,

                        });

                    }

                    _taskService.InsertTask(task).Wait();
                    _taskManager.ProcessQueue();


                    if (updateServerSideIdentification)
                    {
                        foreach (var id in userIds)
                        {
                            _callbacks.DeleteUserFromDeviceFastSearch(code, id);
                        }
                    }

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
