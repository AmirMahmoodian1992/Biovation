using Biovation.Brands.ZK.Devices;
using Biovation.Brands.ZK.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Brands.ZK.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class ZkUserController : ControllerBase
    {
        private readonly AccessGroupService _accessGroupService;
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;
        private readonly Dictionary<uint, Device> _onlineDevices;

        public ZkUserController(AccessGroupService accessGroupService, TaskService taskService, DeviceService deviceService, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskManager taskManager, DeviceBrands deviceBrands, Dictionary<uint, Device> onlineDevices)
        {
            _accessGroupService = accessGroupService;
            _taskService = taskService;
            _deviceService = deviceService;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskManager = taskManager;
            _deviceBrands = deviceBrands;
            _onlineDevices = onlineDevices;
        }

        [HttpGet]
        [Authorize]
        public async Task<List<ResultViewModel>> SendUserToDevice(uint code, string userId)
        {
            return await Task.Run(() =>
            {
                var listResult = new List<ResultViewModel>();
                try
                {
                    try
                    {
                        var userIds = JsonConvert.DeserializeObject<long[]>(userId);

                        //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                        var creatorUser = HttpContext.GetUser();

                        var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.ZkTecoCode).FirstOrDefault();
                        if (devices != null)
                        {
                            var deviceId = devices.DeviceId;

                            var task = new TaskInfo
                            {
                                CreatedAt = DateTimeOffset.Now,
                                CreatedBy = creatorUser,
                                TaskType = _taskTypes.SendUsers,
                                Priority = _taskPriorities.Medium,
                                DeviceBrand = _deviceBrands.ZkTeco,
                                TaskItems = new List<TaskItem>()
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
                                    OrderIndex = 1
                                });

                                listResult.Add(new ResultViewModel { Message = "Sending user queued", Validate = 1 });
                            }
                            _taskService.InsertTask(task);
                        }

                        _taskService.ProcessQueue(_deviceBrands.ZkTeco).ConfigureAwait(false);
                        //_taskManager.ProcessQueue();
                    }
                    catch (Exception e)
                    {
                        Logger.Log($" --> SendUserToDevice Code: {code}  {e}");
                        listResult.Add(new ResultViewModel { Message = e.ToString(), Validate = 0 });

                    }
                    return listResult;
                }
                catch (Exception)
                {
                    listResult.Add(new ResultViewModel { Message = "Sending user queued", Validate = 1 });
                    return listResult;
                }
            });

        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> SendUserToAllDevices([FromBody] User user)
        {
            return await Task.Run(() =>
                {
                    var accessGroups = _accessGroupService.GetAccessGroups(user.Id);
                    var userId = user.Code;

                    //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                    var creatorUser = HttpContext.GetUser();

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.SendUsers,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = _deviceBrands.ZkTeco,
                        TaskItems = new List<TaskItem>()
                    };

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
                                task.TaskItems.Add(new TaskItem
                                {
                                    Status = _taskStatuses.Queued,
                                    TaskItemType = _taskItemTypes.SendUser,
                                    Priority = _taskPriorities.Medium,
                                    DeviceId = deviceGroupMember.DeviceId,
                                    Data = JsonConvert.SerializeObject(new { UserId = userId }),
                                    IsParallelRestricted = true,
                                    IsScheduled = false,
                                    OrderIndex = 1
                                });

                                return new ResultViewModel { Message = "Sending user queued", Validate = 1 };

                            }
                        }
                    }
                    _taskService.InsertTask(task);
                    _taskService.ProcessQueue(_deviceBrands.ZkTeco).ConfigureAwait(false);
                    //_taskManager.ProcessQueue();
                    return new ResultViewModel { Id = user.Id, Validate = 1 };
                });
        }

        [HttpPost]
        [Authorize]
        public async Task<List<ResultViewModel>> DeleteUserFromAllTerminal(int[] userCodes)
        {
            return await Task.Run(() =>
            {
                var onlineDevice = _onlineDevices;
                var result = new List<ResultViewModel>();

                //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                var creatorUser = HttpContext.GetUser();

                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = _taskTypes.DeleteUserFromTerminal,
                    Priority = _taskPriorities.Medium,
                    DeviceBrand = _deviceBrands.ZkTeco,
                    TaskItems = new List<TaskItem>()
                };
                foreach (var device in onlineDevice)
                {
                    var zkDevice = _deviceService.GetDevices(code: device.Key, brandId: DeviceBrands.ZkTecoCode)
                        .FirstOrDefault();

                    foreach (var userCode in userCodes)
                    {
                        if (zkDevice != null)
                            task.TaskItems.Add(new TaskItem
                            {
                                Status = _taskStatuses.Queued,
                                TaskItemType = _taskItemTypes.DeleteUserFromTerminal,
                                Priority = _taskPriorities.Medium,
                                DeviceId = zkDevice.DeviceId,
                                Data = JsonConvert.SerializeObject(new { userCode }),
                                IsParallelRestricted = true,
                                IsScheduled = false,
                                OrderIndex = 1
                            });

                        result.Add(new ResultViewModel { Id = userCode, Validate = 1 });
                    }
                    _taskService.InsertTask(task);
                    _taskService.ProcessQueue(_deviceBrands.ZkTeco).ConfigureAwait(false);
                    //_taskManager.ProcessQueue();
                }
                return result;
            });
        }
    }
}