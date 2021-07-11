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

namespace Biovation.Brands.Paliz.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class PalizBlackListController : ControllerBase
    {
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;

        public PalizBlackListController(TaskService taskService, DeviceService deviceService, DeviceBrands deviceBrands, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities)
        {
            _taskService = taskService;
            _deviceService = deviceService;
            _deviceBrands = deviceBrands;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
        }
        [HttpPost]
        [Authorize]
        public async Task<List<ResultViewModel>> SendBlackLisDevice(List<BlackList> blackLists)
        {
            var resultList = new List<ResultViewModel>();
            try
            {
                //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                var creatorUser = HttpContext.GetUser();
                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = _taskTypes.SendBlackList,
                    Priority = _taskPriorities.Medium,
                    DeviceBrand = _deviceBrands.Paliz,
                    TaskItems = new List<TaskItem>(),
                    DueDate = DateTime.Today
                };

                foreach (var blacklist in blackLists)
                {
                    var devices = _deviceService.GetDevices(code: blacklist.Device.Code, brandId: DeviceBrands.PalizCode).FirstOrDefault();
                    if (devices is null)
                        continue;

                    var deviceId = devices.DeviceId;
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.SendBlackList,
                        Priority = _taskPriorities.Medium,
                        DeviceId = deviceId,
                        Data = JsonConvert.SerializeObject(new { BlackListId = blacklist.Id, UserId = blacklist.User.Code }),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1
                    });

                    _taskService.InsertTask(task);
                    await _taskService.ProcessQueue(_deviceBrands.Paliz).ConfigureAwait(false);

                    resultList.Add(new ResultViewModel { Message = "Sending BlackList queued", Validate = 1 });

                }
                return resultList;
            }
            catch (Exception exception)
            {
                resultList.Add(new ResultViewModel { Message = exception.ToString(), Validate = 0 });
                return resultList;
            }
        }

    }
}
