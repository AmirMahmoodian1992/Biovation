using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Biovation.Brands.Suprema.Commands
{
    public class SupremaGetLogsOfDeviceInPeriod : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly DeviceService _deviceService;

        private TaskItem TaskItem { get; }
        private uint DeviceId { get; set; }
        private long StartDate { get; set; }
        private long EndDate { get; set; }

        public SupremaGetLogsOfDeviceInPeriod(TaskItem taskItem, Dictionary<uint, Device> onlineDevices, DeviceService deviceService)
        {
            TaskItem = taskItem;
            _onlineDevices = onlineDevices;
            _deviceService = deviceService;
        }

        /// <summary>
        /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
        /// </summary>
        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            if (TaskItem is null)
                return new ResultViewModel { Id = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item.{Environment.NewLine}", Validate = 0 };

            DeviceId = (uint)TaskItem.DeviceId;
            var parseResult = uint.TryParse(JsonConvert.DeserializeObject<JObject>(TaskItem.Data)?["fromDate"]?.ToString() ?? "0", out var fromDate);
            if (!parseResult || fromDate == 0)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, zero or null fromDate is provided in data.{Environment.NewLine}", Validate = 0 };
            parseResult = uint.TryParse(JsonConvert.DeserializeObject<JObject>(TaskItem.Data)?["toDate"]?.ToString() ?? "0", out var toDate);
            if (!parseResult || toDate == 0)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, zero or null toDate is provided in data.{Environment.NewLine}", Validate = 0 };
            StartDate = fromDate;
            EndDate = toDate;
            var device = _deviceService.GetDevice(DeviceId).Result?.Data;
            if (device is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!_onlineDevices.ContainsKey(device.Code))
            {
                Logger.Log($"The device: {device.DeviceId} is not connected.");
                return new ResultViewModel { Validate = 0, Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }



            var refDate = new DateTime(1970, 1, 1).Ticks / 10000000;
            //var startDateTicks = Convert.ToInt32((long)(StartDate / 10000000) - refDate);
            //var endDateTicks = Convert.ToInt32((long)(EndDate / 10000000) - refDate);
            var startDateTicks = Convert.ToInt32((StartDate / 10000000) - refDate);
            var endDateTicks = Convert.ToInt32((EndDate / 10000000) - refDate);
            return _onlineDevices[DeviceId].ReadLogOfPeriod(startDateTicks, endDateTicks);

        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return " Get all logs of a device in a period of time command";
        }

        public string GetDescription()
        {
            return $" Getting all logs of a device (id: {DeviceId} from: {new DateTime(1970, 1, 1).AddTicks(StartDate)} To: {new DateTime(1970, 1, 1).AddTicks(EndDate)}) command";
        }
    }
}
