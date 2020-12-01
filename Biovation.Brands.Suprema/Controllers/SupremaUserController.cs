using Biovation.Brands.Suprema.Commands;
using Biovation.Brands.Suprema.Services;
using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Brands.Suprema.Manager;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;

namespace Biovation.Brands.Suprema.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class SupremaUserController : ControllerBase
    {
        private readonly CommandFactory _commandFactory;
        private readonly FastSearchService _fastSearchService;

        private readonly UserService _userService;
        private readonly AccessGroupService _accessGroupService;
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;

        public SupremaUserController(UserService userService, AccessGroupService accessGroupService, FastSearchService fastSearchService, CommandFactory commandFactory, TaskService taskService, DeviceService deviceService, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskPriorities taskPriorities, TaskItemTypes taskItemTypes, TaskManager taskManager, DeviceBrands deviceBrands)
        {
            _userService = userService;
            _accessGroupService = accessGroupService;
            _fastSearchService = fastSearchService;
            _commandFactory = commandFactory;
            _taskService = taskService;
            _deviceService = deviceService;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskPriorities = taskPriorities;
            _taskItemTypes = taskItemTypes;
            _taskManager = taskManager;
            _deviceBrands = deviceBrands;
        }

        [HttpGet]
        public Task<List<User>> Users()
        {
            return Task.Run(() =>
            {

                var user = _userService.GetUsers();
                return user;
            }
        );
        }


        [HttpGet]
        [Authorize]
        public User Users(int id)
        {
            var user = _userService.GetUsers(userId: id)?.FirstOrDefault();
            return user;
        }

        [HttpGet]
        [Authorize]
        public List<AccessGroup> GetUserAccessGroups(int userId)
        {
            var accessGroups = _accessGroupService.GetAccessGroups(userId: userId);
            return accessGroups.ToList();
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyUser([FromBody]User user)
        {
            try
            {
                _fastSearchService.Initial();
                return new ResultViewModel { Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel> SendUserToDevice(uint code, string userId)
        {
            return Task.Run(() =>
            {

                try
                {
                    var userIds = JsonConvert.DeserializeObject<long[]>(userId);
                    var creatorUser = HttpContext.GetUser();

                    var devices = _deviceService.GetDevices(code: code, brandId: DeviceBrands.SupremaCode).FirstOrDefault();
                    if (devices != null)
                    {
                        var deviceId = devices.DeviceId;

                        var task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = _taskTypes.SendUsers,
                            Priority = _taskPriorities.Medium,
                            DeviceBrand = _deviceBrands.Suprema,
                            TaskItems = new List<TaskItem>()
                        };

                        foreach (var receivedUserId in userIds)
                        {
                            task.TaskItems.Add(new TaskItem
                            {
                                Status = _taskStatuses.Queued,
                                TaskItemType = _taskItemTypes.SendUser,
                                Priority = _taskPriorities.Medium,
                                DeviceId = deviceId,
                                Data = JsonConvert.SerializeObject(new { UserId = receivedUserId }),
                                IsParallelRestricted = true,
                                IsScheduled = false,
                                OrderIndex = 1
                            });

                            //listResult.Add(new ResultViewModel { Message = "Sending user queued", Validate = 1 });
                        }
                        _taskService.InsertTask(task);
                    }
                    _taskManager.ProcessQueue();
                    return new ResultViewModel { Validate = 1, Message = "Sending user queued" };
                }
                catch (Exception e)
                {
                    Logger.Log($" --> SendUserToDevice Code: {code}  {e}");
                    return new ResultViewModel { Validate = 0, Message = e.Message };
                }
            });

        }

        [HttpPost]
        [Authorize]
        public ResultViewModel SendUserToAllDevices([FromBody]User user)
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
                DeviceBrand = _deviceBrands.Suprema,
                TaskItems = new List<TaskItem>()
            };

            if (!accessGroups.Any())
            {
                return new ResultViewModel { Id = user.Id, Validate = 0 };
            }

            foreach (var deviceGroupMember in accessGroups.SelectMany(accessGroup => accessGroup.DeviceGroup.SelectMany(deviceGroup => deviceGroup.Devices)))
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
            _taskService.InsertTask(task);
            _taskManager.ProcessQueue();
            return new ResultViewModel { Id = user.Id, Validate = 1 };
        }
    }
}
