﻿using Biovation.Brands.Virdi.Command;
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

namespace Biovation.Brands.Virdi.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class VirdiUserController : ControllerBase
    {
        private readonly VirdiServer _virdiServer;
        private readonly TaskService _taskService;
        private readonly DeviceBrands _deviceBrands;
        private readonly DeviceService _deviceService;
        private readonly CommandFactory _commandFactory;
        private readonly AccessGroupService _accessGroupService;

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;

        public VirdiUserController(TaskService taskService, DeviceService deviceService, VirdiServer virdiServer, AccessGroupService accessGroupService, CommandFactory commandFactory, DeviceBrands deviceBrands, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities)
        {
            _taskService = taskService;
            _deviceService = deviceService;
            _virdiServer = virdiServer;
            _accessGroupService = accessGroupService;
            _commandFactory = commandFactory;
            _deviceBrands = deviceBrands;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> EnrollFromTerminal([FromBody] uint deviceId)
        {
            try
            {
                var creatorUser = HttpContext.GetUser();
                var devices = _deviceService.GetDevice(deviceId);

                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = _taskTypes.EnrollFromTerminal,
                    Priority = _taskPriorities.Medium,
                    DeviceBrand = _deviceBrands.Virdi,
                    TaskItems = new List<TaskItem>(),
                    DueDate = DateTime.Today
                };

                task.TaskItems.Add(new TaskItem
                {
                    Status = _taskStatuses.Queued,
                    TaskItemType = _taskItemTypes.EnrollFromTerminal,
                    Priority = _taskPriorities.Medium,
                    DeviceId = devices.DeviceId,
                    Data = JsonConvert.SerializeObject(new { deviceId }),
                    IsParallelRestricted = true,
                    IsScheduled = false,
                    OrderIndex = 1,
                    CurrentIndex = 0,
                    TotalCount = 1
                });

                _taskService.InsertTask(task);
                await _taskService.ProcessQueue(_deviceBrands.Virdi, devices.DeviceId).ConfigureAwait(false);

                var result = new ResultViewModel { Validate = 1, Message = "Enrolling User queued" };
                return result;

            }
            catch (Exception exception)
            {
                return new ResultViewModel { Validate = 1, Message = $"Error ,Enrolling User not queued!{exception}" };
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> ModifyUser([FromBody] User user)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // ReSharper disable once AssignmentIsFullyDiscarded
                    _virdiServer.LoadFingerTemplates().ConfigureAwait(false);
                    return new ResultViewModel { Validate = 1 };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = exception.Message };
                }
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<List<ResultViewModel>> SendUserToDevice(uint code, string userId, bool updateServerSideIdentification = false)
        {
            var resultList = new List<ResultViewModel>();
            try
            {
                var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.VirdiCode).FirstOrDefault();
                if (device is null)
                    return new List<ResultViewModel> { new ResultViewModel { Success = false, Message = $"Device {code} does not exists" } };

                var userIds = JsonConvert.DeserializeObject<long[]>(userId);


                foreach (var id in userIds)
                {

                    if (!updateServerSideIdentification) continue;
                    // ReSharper disable once AssignmentIsFullyDiscarded
                    _ =_virdiServer.AddUserToDeviceFastSearch(code, (int)id).ConfigureAwait(false);
                }

                resultList.Add(new ResultViewModel { Message = "Sending user queued", Validate = 1 });
                return resultList;
            }
            catch (Exception exception)
            {
                resultList.Add(new ResultViewModel { Message = exception.ToString(), Validate = 0 });
                return resultList;
            }
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel SendUserToAllDevices([FromBody] User user)
        {
            return new ResultViewModel { Id = user.Id, Validate = 1 };
        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel> DeleteUserFromTerminal(uint code, int userId)
        {
            return await Task.Run(() =>
            {
                var deleteUserFromTerminalCommand = _commandFactory.Factory(CommandType.DeleteUserFromTerminal,
                    new List<object> { code, userId });

                var result = deleteUserFromTerminalCommand.Execute();

                return new ResultViewModel { Id = userId, Validate = Convert.ToInt32(result) };
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<List<ResultViewModel>> DeleteUserFromAllTerminal(int[] ids)
        {
            return await Task.Run(() =>
            {
                var onlineDevice = _virdiServer.GetOnlineDevices();
                var result = new List<ResultViewModel>();
                foreach (var device in onlineDevice)
                {
                    foreach (var userId in ids)
                    {
                        var deleteUserFromTerminalCommand = _commandFactory.Factory(CommandType.DeleteUserFromTerminal,
                            new List<object> { device.Key, userId });
                        var deleteResult = (bool)deleteUserFromTerminalCommand.Execute();
                        result.Add(new ResultViewModel { Id = userId, Validate = Convert.ToInt32(deleteResult) });
                    }
                }

                return result;
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> EnrollFaceTemplate(int userId, int deviceId)
        {
            try
            {
                //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();

                return new ResultViewModel { Id = userId, Validate = 1, Message = $"Enrolling face from device {deviceId} started successfully." };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Id = userId, Validate = 0, Message = $"Enrolling face from device {deviceId} encountered an error: {exception}." };
            }
        }
    }
}
