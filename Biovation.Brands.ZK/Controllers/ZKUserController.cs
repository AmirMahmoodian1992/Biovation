using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using Newtonsoft.Json;

namespace Biovation.Brands.ZK.ApiControllers
{
    public class ZKUserController : ApiController
    {
        private readonly AccessGroupService _accessGroupService = new AccessGroupService();
        private readonly TaskService _taskService = new TaskService();
        private readonly UserService _userService = new UserService();
        private readonly DeviceService _deviceService = new DeviceService();
        [HttpGet]
        public Task<List<ResultViewModel>> SendUserToDevice(uint code, string userId)
        {
            return Task.Run(() =>
            {
                var listResult = new List<ResultViewModel>();
                try
                {
                    try
                    {
                        var userIds = JsonConvert.DeserializeObject<long[]>(userId);
                        var creatorUser = _userService.GetUser(123456789, false);
                        var devices = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.ZkTecoCode);
                        var deviceId = devices.DeviceId;

                        var task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = TaskTypes.SendUsers,
                            Priority = TaskPriorities.Medium,
                            DeviceBrand = DeviceBrands.ZkTeco,
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

                            listResult.Add(new ResultViewModel { Message = "Sending user queued", Validate = 1 });
                        }
                        _taskService.InsertTask(task).Wait();
                        ZKTecoServer.ProcessQueue();
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
        public Task<ResultViewModel> SendUserToAllDevices([FromBody]User user)
        {
            return Task.Run(() =>
                {
                    var accessGroups = _accessGroupService.GetAccessGroupsOfUser(user.Id);
                    var userId = user.Code;
                    var creatorUser = _userService.GetUser(123456789, false);
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.SendUsers,
                        Priority = TaskPriorities.Medium,
                        DeviceBrand = DeviceBrands.ZkTeco,
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
                                    Status = TaskStatuses.Queued,
                                    TaskItemType = TaskItemTypes.SendUser,
                                    Priority = TaskPriorities.Medium,
                                    DueDate = DateTime.Today,
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
                    _taskService.InsertTask(task).Wait();
                    ZKTecoServer.ProcessQueue();
                    return new ResultViewModel { Id = user.Id, Validate = 1 };
                });

        }

        [HttpPost]
        public Task<List<ResultViewModel>> DeleteUserFromAllTerminal(int[] ids)
        {
            return Task.Run(() =>
            {
                var onlineDevice = ZKTecoServer.GetOnlineDevices();
                var result = new List<ResultViewModel>();
                var creatorUser = _userService.GetUser(123456789, false);

                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = TaskTypes.DeleteUserFromTerminal,
                    Priority = TaskPriorities.Medium,
                    DeviceBrand = DeviceBrands.ZkTeco,
                    TaskItems = new List<TaskItem>()
                };
                foreach (var device in onlineDevice)
                {
                    var zkdevice = _deviceService.GetDeviceBasicInfoWithCode(device.Key, DeviceBrands.ZkTecoCode);

                    foreach (var id in ids)
                    {
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = TaskStatuses.Queued,
                            TaskItemType = TaskItemTypes.DeleteUserFromTerminal,
                            Priority = TaskPriorities.Medium,
                            DueDate = DateTime.Today,
                            DeviceId = zkdevice.DeviceId,
                            Data = JsonConvert.SerializeObject(new { UserId = id }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1
                        });

                        result.Add(new ResultViewModel { Id = id, Validate = 1 });
                    }
                    _taskService.InsertTask(task).Wait();
                    ZKTecoServer.ProcessQueue();
                }
                return result;
            });

        }
    }
}