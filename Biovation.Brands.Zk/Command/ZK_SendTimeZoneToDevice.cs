using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimeZone = Biovation.CommonClasses.Models.TimeZone;

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
        private readonly TaskService _taskService = new TaskService();
        private readonly DeviceService _deviceService = new DeviceService();

        private readonly TimeZoneService _timeZoneService = new TimeZoneService();

        public ZKSendTimeZoneToDevice(IReadOnlyList<object> items, Dictionary<uint, Device> devices)
        {

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = _deviceService.GetDeviceBasicInfoByIdAndBrandId(DeviceId, DeviceBrands.ZkTecoCode)?.Code ?? 0;
            var taskItem = _taskService.GetTaskItem(TaskItemId).Result;
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            TimeZoneId = (int)data["timeZoneId"];
            TimeZoneObj = _timeZoneService.GetTimeZoneById(TimeZoneId);
            OnlineDevices = devices;
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Value.GetDeviceInfo().DeviceId != DeviceId))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"The device: {Code} is not connected.", Code = Convert.ToInt64(TaskStatuses.DeviceDisconnected.Code) };
            }

            if (TimeZoneObj == null)
            {
                Logger.Log($"TimeZone {TimeZoneId} does not exist.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"TimeZone {TimeZoneId} does not exist.", Code = Convert.ToInt64(TaskStatuses.Failed.Code) };
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.TransferTimeZone(TimeZoneObj);
                return new ResultViewModel { Validate = result ? 1 : 0, Id = DeviceId, Message = $"Send TimeZone {TimeZoneId} .", Code = Convert.ToInt64(TaskStatuses.Done.Code) };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $" Send TimeZone {TimeZoneId} Failed.", Code = Convert.ToInt64(TaskStatuses.Failed.Code) };

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
