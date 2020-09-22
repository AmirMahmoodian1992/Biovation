﻿using Biovation.Brands.Virdi.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Virdi.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class VirdiBlackListController : Controller
    {
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskService _taskService;
        private readonly TaskManager _taskManager;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;

        public VirdiBlackListController(TaskService taskService, UserService userService, DeviceService deviceService, DeviceBrands deviceBrands, TaskManager taskManager, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities)
        {
            _taskService = taskService;
            _userService = userService;
            _deviceService = deviceService;
            _deviceBrands = deviceBrands;
            _taskManager = taskManager;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
        }

        [HttpPost]
        public Task<List<ResultViewModel>> SendBlackLisDevice(List<BlackList> blackLists)
        {

            return Task.Run(() =>
            {
                var resultList = new List<ResultViewModel>();
                try
                {
                    var creatorUser = _userService.GetUsers(userId:123456789, withPicture:false)[0];
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.SendBlackList,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = _deviceBrands.Virdi,
                        TaskItems = new List<TaskItem>()
                    };


                    foreach (var blacklist in blackLists)
                    {

                        var devices = _deviceService.GetDevices(code:blacklist.Device.Code, brandId:int.Parse(DeviceBrands.VirdiCode))[0];
                        var deviceId = devices.DeviceId;
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.SendBlackList,
                            Priority = _taskPriorities.Medium,
                            DueDate = DateTime.Today,
                            DeviceId = deviceId,
                            Data = JsonConvert.SerializeObject(new { BlackListId = blacklist.Id, UserId = blacklist.User.Id }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1
                        });

                        _taskService.InsertTask(task);
                        _taskManager.ProcessQueue();

                        resultList.Add(new ResultViewModel { Message = "Sending BlackList queued", Validate = 1 });

                    }
                    return resultList;
                }
                catch (Exception exception)
                {
                    resultList.Add(new ResultViewModel { Message = exception.ToString(), Validate = 0 });
                    return resultList;
                }
            });
        }
    }
}
