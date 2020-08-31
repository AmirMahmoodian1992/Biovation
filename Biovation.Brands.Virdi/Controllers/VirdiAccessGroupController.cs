using Biovation.Brands.Virdi.Command;
using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Constants;
using Biovation.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Brands.Virdi.Manager;
using DeviceBrands = Biovation.Constants.DeviceBrands;

namespace Biovation.Brands.Virdi.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class VirdiAccessGroupController : Controller
    {
        private readonly Callbacks _callbacks;
        private readonly TaskService _taskService;
        private readonly TaskManager _taskManager;
        private readonly UserService _userService;
        private readonly DeviceBrands _deviceBrands;
        private readonly DeviceService _deviceService;
        private readonly CommandFactory _commandFactory;

        public VirdiAccessGroupController(TaskService taskService, UserService userService, DeviceService deviceService, VirdiServer virdiServer, Callbacks callbacks, CommandFactory commandFactory, TaskManager taskManager, DeviceBrands deviceBrands)
        {
            _taskService = taskService;
            _userService = userService;
            _deviceService = deviceService;
            _callbacks = callbacks;
            _commandFactory = commandFactory;
            _taskManager = taskManager;
            _deviceBrands = deviceBrands;
        }

        [HttpPost]
        public Task<ResultViewModel> SendAccessGroupToAllDevices([FromBody] int accessGroupId)
        {
            return Task.Run(() =>
            {
                try
                {
                    var devices = _deviceService.GetAllDevicesBasicInfosByBrandId(DeviceBrands.VirdiCode);
                    var creatorUser = _userService.GetUser(123456789, false);
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.SendUsers,
                        Priority = TaskPriorities.Medium,
                        DeviceBrand = _deviceBrands.Virdi,
                        TaskItems = new List<TaskItem>()
                    };
                    foreach (var device in devices)
                    {
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = TaskStatuses.Queued,
                            TaskItemType = TaskItemTypes.SendAccessGroupToTerminal,
                            Priority = TaskPriorities.Medium,
                            DueDate = DateTime.Today,
                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(new { accessGroupId }),
                            IsParallelRestricted = true,
                            IsScheduled = false,

                            OrderIndex = 1
                        });
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
        public ResultViewModel SendAccessGroupToDevice(int accessGroupId, uint code)
        {
            var sendAccessGroupCommand = _commandFactory.Factory(CommandType.SendAccessGroupToDevice,
                new List<object> { code, accessGroupId });

            var result = (bool)sendAccessGroupCommand.Execute();

            return new ResultViewModel { Validate = result ? 1 : 0, Message = code.ToString() };
        }

        [HttpPost]
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
