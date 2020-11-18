using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Brands.PW.Devices;
using Biovation.Brands.PW.Manager;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Biovation.Brands.PW.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class PwDeviceController : ControllerBase
    {
        private readonly PwServer _pwServer;
        private readonly TaskService _taskService;
        private readonly TaskManager _taskManager;
        private readonly DeviceService _deviceService;

        private readonly TaskTypes _taskTypes;
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;

        private readonly Dictionary<uint, Device> _onlineDevices;

        public PwDeviceController(TaskService taskService, DeviceService deviceService, Dictionary<uint, Device> onlineDevices, PwServer pwServer, TaskTypes taskTypes, DeviceBrands deviceBrands, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, TaskManager taskManager)
        {
            _taskService = taskService;
            _deviceService = deviceService;
            _onlineDevices = onlineDevices;
            _pwServer = pwServer;
            _taskTypes = taskTypes;
            _deviceBrands = deviceBrands;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _taskManager = taskManager;
        }


        [HttpGet]
        [AllowAnonymous]
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            var onlineDevices = new List<DeviceBasicInfo>();

            lock (_onlineDevices)
            {
                foreach (var onlineDevice in _onlineDevices)
                {
                    if (string.IsNullOrEmpty(onlineDevice.Value.GetDeviceInfo().Name))
                        onlineDevice.Value.GetDeviceInfo().Name = _deviceService.GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.ProcessingWorldCode)?.FirstOrDefault()?.Name;

                    onlineDevices.Add(onlineDevice.Value.GetDeviceInfo());
                }
            }

            return onlineDevices;
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            if (device.Active)
            {
                _pwServer.ConnectToDevice(device);
            }

            else
            {
                _pwServer.DisconnectFromDevice(device);
            }

            return new ResultViewModel { Validate = 0, Id = device.DeviceId };
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel> ReadOfflineOfDevice(uint code)
        {

            return Task.Run(() =>
            {
                try
                {


                    var device = _deviceService.GetDevices(code: code, brandId: DeviceBrands.ProcessingWorldCode)?.FirstOrDefault();
                    if (device != null)
                    {
                        var deviceId = device.DeviceId;
                        //var creatorUser = _userService.GetUsers(123456789)?.FirstOrDefault();
                        var creatorUser = HttpContext.GetUser();

                        var task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = _taskTypes.GetServeLogs,
                            Priority = _taskPriorities.Medium,
                            DeviceBrand = _deviceBrands.ProcessingWorld,
                            TaskItems = new List<TaskItem>(),
                            DueDate = DateTime.Today
                        };
                        task.TaskItems.Add(new TaskItem
                        {

                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.GetServeLogs,
                            Priority = _taskPriorities.Medium,
                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(new { deviceId }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1
                        });

                        _taskService.InsertTask(task);
                    }

                    _taskManager.ProcessQueue();

                    var result = new ResultViewModel { Validate = 1, Message = $"Reading logs of device {code} queued" };
                    return result;

                }
                catch (Exception exception)
                {
                    return new ResultViewModel { Validate = 1, Message = $"Error ,Reading logs of device {code} queued!{exception}" };
                }
            });
        }


        [HttpPost]
        [Authorize]
        public Dictionary<uint, bool> DeleteDevices([FromBody] List<uint> deviceIds)
        {
            var resultList = new Dictionary<uint, bool>();

            foreach (var deviceId in deviceIds)
            {
                lock (_onlineDevices)
                {
                    if (_onlineDevices.ContainsKey(deviceId))
                    {
                        _onlineDevices[deviceId].Disconnect();
                        _onlineDevices.Remove(deviceId);
                    }
                }

                resultList.Add(deviceId, true);
            }

            return resultList;
        }
    }
}
