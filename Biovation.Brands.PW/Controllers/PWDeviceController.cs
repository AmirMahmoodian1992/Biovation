using Biovation.Brands.PW.Devices;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Serilog;
using Newtonsoft.Json;

namespace Biovation.Brands.PW.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly PwServer _pwServer;
        private readonly DeviceService _deviceService;
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly TaskTypes _taskTypes;
        private readonly DeviceBrands _deviceBrands;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;


        public DeviceController(DeviceService deviceService, Dictionary<uint, Device> onlineDevices, PwServer pwServer, ILogger logger, TaskTypes taskTypes, DeviceBrands deviceBrands, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities)
        {
            _deviceService = deviceService;
            _onlineDevices = onlineDevices;
            _pwServer = pwServer;
            _logger = logger.ForContext<DeviceController>();
            _taskTypes = taskTypes;
            _deviceBrands = deviceBrands;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
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
                    try
                    {
                        if (string.IsNullOrEmpty(onlineDevice.Value.GetDeviceInfo().Name))
                            onlineDevice.Value.GetDeviceInfo().Name = _deviceService.GetDevices(code: onlineDevice.Key, brandId: DeviceBrands.ProcessingWorldCode)?.FirstOrDefault()?.Name;
                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, exception.Message);
                    }

                    try
                    {
                        onlineDevices.Add(onlineDevice.Value.GetDeviceInfo());
                    }
                    catch (Exception exception)
                    {
                        _logger.Warning(exception, exception.Message);
                    }
                }
            }

            return onlineDevices;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> ModifyDevice([FromBody] DeviceBasicInfo device)
        {
            return await Task.Run(() =>
            {
                if (device.Active)
                    _pwServer.ConnectToDevice(device);

                else
                    _pwServer.DisconnectFromDevice(device);

                return new ResultViewModel { Validate = 0, Id = device.DeviceId };
            });
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel> ReadOfflineOfDevice(uint code, DateTime? fromDate, DateTime? toDate)
        {
            return Task.Run(() =>
            {
                var result = new ResultViewModel {Validate = 1, Message = $"Reading logs of device {code} queued"};
                return result;
            });
        }

        [HttpPost]
        [Authorize]
        public Dictionary<uint, bool> DeleteDevices([FromBody] List<uint> deviceIds)
        {
            var resultList = new Dictionary<uint, bool>();

            foreach (var deviceId in deviceIds)
            {
                var device = _deviceService.GetDevice(deviceId);
                lock (_onlineDevices)
                {
                    if (_onlineDevices.ContainsKey(device.Code))
                    {
                        _onlineDevices[device.Code].Disconnect();
                        if (_onlineDevices.ContainsKey(device.Code))
                            _onlineDevices.Remove(device.Code);
                    }
                }

                resultList.Add(deviceId, true);
            }

            return resultList;
        }

        //[HttpGet]
        //[Authorize]
        //public async Task<Dictionary<string, string>> GetAdditionalData(uint code)
        //{
        //    var creatorUser = HttpContext.GetUser();

        //    var task = new TaskInfo
        //    {
        //        CreatedAt = DateTimeOffset.Now,
        //        CreatedBy = creatorUser,
        //        TaskType = _taskTypes.GetLogsInPeriod,
        //        Priority = _taskPriorities.Immediate,
        //        DeviceBrand = _deviceBrands.Eos,
        //        TaskItems = new List<TaskItem>(),
        //        DueDate = DateTime.Today
        //    };
        //    var device = ( _deviceService.GetDevices(code: code, brandId: DeviceBrands.EosCode))
        //        ?.FirstOrDefault();

        //    if (device is null)
        //    {
        //        return null;
        //    }

        //    var deviceId = device.DeviceId;
        //    task.TaskItems.Add(new TaskItem
        //    {
        //        Status = _taskStatuses.Done,
        //        TaskItemType = _taskItemTypes.GetLogsInPeriod,
        //        Priority = _taskPriorities.Immediate,
        //        DeviceId = deviceId,
        //        Data = JsonConvert.SerializeObject(new { deviceId }),
        //        IsParallelRestricted = true,
        //        IsScheduled = false,
        //        OrderIndex = 1,
        //        CurrentIndex = 0
        //    });

        //    var getAdditionalData = _commandFactory.Factory(CommandType.GetDeviceAdditionalData,
        //        new List<object> { task.TaskItems.FirstOrDefault() });

        //    var result = getAdditionalData.Execute();

        //    return (Dictionary<string, string>)result;
        //}
    }
}
