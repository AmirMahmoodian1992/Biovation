using Biovation.Brands.Virdi.Command;
using Biovation.Brands.Virdi.Manager;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceBrands = Biovation.Constants.DeviceBrands;

namespace Biovation.Brands.Virdi.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class VirdiAccessGroupController : ControllerBase
    {
        private readonly Callbacks _callbacks;
        private readonly TaskService _taskService;
        private readonly TaskManager _taskManager;
        private readonly DeviceBrands _deviceBrands;
        private readonly DeviceService _deviceService;
        private readonly CommandFactory _commandFactory;

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;

        public VirdiAccessGroupController(TaskService taskService, DeviceService deviceService, Callbacks callbacks, CommandFactory commandFactory, TaskManager taskManager, DeviceBrands deviceBrands, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities)
        {
            _taskService = taskService;
            _deviceService = deviceService;
            _callbacks = callbacks;
            _commandFactory = commandFactory;
            _taskManager = taskManager;
            _deviceBrands = deviceBrands;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> SendAccessGroupToAllDevices([FromBody] int accessGroupId)
        {
            return Task.Run(() =>
            {
                try
                {
                    var devices = _deviceService.GetDevices(brandId: DeviceBrands.VirdiCode);
                    //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                    var creatorUser = HttpContext.GetUser();
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.SendUsers,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = _deviceBrands.Virdi,
                        TaskItems = new List<TaskItem>(),
                        DueDate = DateTime.Today
                    };
                    foreach (var device in devices)
                    {
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.SendAccessGroupToTerminal,
                            Priority = _taskPriorities.Medium,
                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(new { accessGroupId }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,
                            CurrentIndex = 0,
                            TotalCount = 1
                        });
                    }

                    _taskService.InsertTask(task);
                    _taskManager.ProcessQueue();


                    return new ResultViewModel { Validate = 1, Message = "Sending users queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }
        /*   public ResultViewModel SendAccessGroupToAllDevices([FromBody]int accessGroupId)
           {
               var devices = _deviceService.GetAllDevicesBasicInfosByBrandId(DeviceBrands.VirdiCode);
   
               foreach (var device in devices)
               {
                   var sendAccessGroupCommand = CommandFactory.Factory(CommandType.SendAccessGroupToDevice,
                   new List<object> { device.DeviceId, accessGroupId });
   
                   sendAccessGroupCommand.Execute();
               }
   
               return new ResultViewModel { Validate = 0 };
           }*/

        [HttpGet]
        [Authorize]
        public ResultViewModel SendAccessGroupToDevice(int accessGroupId, uint code)
        {
            var sendAccessGroupCommand = _commandFactory.Factory(CommandType.SendAccessGroupToDevice,
                new List<object> { code, accessGroupId });

            var result = (bool)sendAccessGroupCommand.Execute();

            return new ResultViewModel { Validate = result ? 1 : 0, Message = code.ToString() };
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyAccessGroup(string accessGroup)
        {
            try
            {
                _callbacks.LoadFingerTemplates();
                return new ResultViewModel { Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }
    }
}
