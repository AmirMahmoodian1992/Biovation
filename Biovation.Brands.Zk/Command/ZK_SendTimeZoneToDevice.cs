using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TimeZone = Biovation.Domain.TimeZone;

namespace Biovation.Brands.ZK.Command
{
    public class ZKSendTimeZoneToDevice : ICommand
    {/// <summary>
     /// All connected devices
     /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }
        private int TimeZoneId { get; set; }
        private TimeZone TimeZoneObj { get; }
        private int TaskItemId { get; }
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;

        private readonly TimeZoneService _timeZoneService;

        public ZKSendTimeZoneToDevice(IReadOnlyList<object> items, Dictionary<uint, Device> devices, TimeZoneService timeZoneService, DeviceService deviceService, TaskService taskService)
        {

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = (_deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);
            var taskItem = _taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            TimeZoneId = (int)data["timeZoneId"];
            TimeZoneObj = _timeZoneService.TimeZones(TimeZoneId);
            OnlineDevices = devices;
            _timeZoneService = timeZoneService;
            _deviceService = deviceService;
            _taskService = taskService;
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Value.GetDeviceInfo().DeviceId != DeviceId))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"The device: {Code} is not connected.", Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            if (TimeZoneObj == null)
            {
                Logger.Log($"TimeZone {TimeZoneId} does not exist.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"TimeZone {TimeZoneId} does not exist.", Code = Convert.ToInt64(TaskStatuses.FailedCode) };
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.TransferTimeZone(TimeZoneObj);
                return new ResultViewModel { Validate = result ? 1 : 0, Id = DeviceId, Message = $"Send TimeZone {TimeZoneId} .", Code = Convert.ToInt64(TaskStatuses.DoneCode) };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $" Send TimeZone {TimeZoneId} Failed.", Code = Convert.ToInt64(TaskStatuses.FailedCode) };

            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Send timeZone to terminal";
        }

        public string GetDescription()
        {
            return $"Sending timeZone: {TimeZoneId} to device: {Code}.";
        }
    }
}
