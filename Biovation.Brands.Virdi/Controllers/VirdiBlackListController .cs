using Biovation.Brands.Virdi.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public VirdiBlackListController(TaskService taskService, UserService userService, DeviceService deviceService, DeviceBrands deviceBrands, TaskManager taskManager)
        {
            _taskService = taskService;
            _userService = userService;
            _deviceService = deviceService;
            _deviceBrands = deviceBrands;
            _taskManager = taskManager;
        }

        [HttpPost]
        public Task<List<ResultViewModel>> SendBlackLisDevice(List<BlackList> blackLists)
        {

            return Task.Run(() =>
            {
                var resultList = new List<ResultViewModel>();
                try
                {
                    var creatorUser = _userService.GetUser(123456789, false);
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.SendBlackList,
                        Priority = TaskPriorities.Medium,
                        DeviceBrand = _deviceBrands.Virdi,
                        TaskItems = new List<TaskItem>()
                    };


                    foreach (var blacklist in blackLists)
                    {

                        var devices = _deviceService.GetDeviceBasicInfoWithCode(blacklist.Device.Code, DeviceBrands.VirdiCode);
                        var deviceId = devices.DeviceId;
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = TaskStatuses.Queued,
                            TaskItemType = TaskItemTypes.SendBlackList,
                            Priority = TaskPriorities.Medium,
                            DueDate = DateTime.Today,
                            DeviceId = deviceId,
                            Data = JsonConvert.SerializeObject(new { BlackListId = blacklist.Id, UserId = blacklist.User.Id }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1
                        });

                        _taskService.InsertTask(task).Wait();
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
