using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using Newtonsoft.Json;

namespace Biovation.Brands.ZK.ApiControllers
{
    public class ZKTimeZoneController : ApiController
    {
        private readonly DeviceService _deviceService = new DeviceService();
        private readonly TaskService _taskService = new TaskService();
        private readonly UserService _userService = new UserService();

        [HttpPost]
        public Task<ResultViewModel> SendTimeZoneToAllDevices([FromBody]int timeZoneId)
        {
            return Task.Run(() =>
            {
                try
                {
                    var devices = _deviceService.GetAllDevicesBasicInfosByBrandId(DeviceBrands.ZkTecoCode);
                    var creatorUser = _userService.GetUser(123456789, false);
                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = TaskTypes.SendTimeZoneToTerminal,
                        Priority = TaskPriorities.Medium,
                        DeviceBrand = DeviceBrands.ZkTeco,
                        TaskItems = new List<TaskItem>()
                    };

                    foreach (var device in devices)
                    {
                        task.TaskItems.Add(new TaskItem
                        {
                            Status = TaskStatuses.Queued,
                            TaskItemType = TaskItemTypes.SendTimeZoneToTerminal,
                            Priority = TaskPriorities.Medium,
                            DueDate = DateTime.Today,
                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(new { timeZoneId }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1
                        });
                    }
                    _taskService.InsertTask(task).Wait();
                    ZKTecoServer.ProcessQueue();


                    return new ResultViewModel { Validate = 1, Message = "Sending TimeZoneToTerminal queued" };
                }
                catch (Exception exception)
                {
                    return new ResultViewModel
                    {
                        Validate = 0,
                        Message = exception.ToString()
                    };
                }
            });

        }

        [HttpGet]
        public Task<ResultViewModel> SendTimeZoneToDevice(int timeZoneId, uint code)
        {
            return Task.Run(() =>
                {
                    try
                    {
                        var device = _deviceService.GetDeviceBasicInfoWithCode(code, DeviceBrands.ZkTecoCode);

                        var creatorUser = _userService.GetUser(123456789, false);
                        var task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = TaskTypes.SendTimeZoneToTerminal,
                            Priority = TaskPriorities.Medium,
                            DeviceBrand = DeviceBrands.ZkTeco,
                            TaskItems = new List<TaskItem>()
                        };

                            task.TaskItems.Add(new TaskItem
                            {
                                Status = TaskStatuses.Queued,
                                TaskItemType = TaskItemTypes.SendTimeZoneToTerminal,
                                Priority = TaskPriorities.Medium,
                                DueDate = DateTime.Today,
                                DeviceId = device.DeviceId,
                                Data = JsonConvert.SerializeObject(new { timeZoneId }),
                                IsParallelRestricted = true,
                                IsScheduled = false,

                                OrderIndex = 1
                            });
                            _taskService.InsertTask(task).Wait();
                            ZKTecoServer.ProcessQueue();
                            return new ResultViewModel { Validate = 1, Message = "Sending TimeZoneToTerminal queued" };
                    }
                    catch (Exception exception)
                    {
                        return new ResultViewModel
                        {
                            Validate = 0,
                            Message = exception.ToString()
                        };
                    }
                });
        }
    }
}
