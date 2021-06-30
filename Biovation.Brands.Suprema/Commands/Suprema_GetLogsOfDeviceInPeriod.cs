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
using System.Threading.Tasks;

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
            var parseResult = DateTime.TryParse(JsonConvert.DeserializeObject<JObject>(TaskItem.Data)?["fromDate"]?.ToString() ?? "1970/01/01", out var fromDate);
            if (!parseResult || fromDate == default)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, zero or null fromDate is provided in data.{Environment.NewLine}", Validate = 0 };
            parseResult = DateTime.TryParse(JsonConvert.DeserializeObject<JObject>(TaskItem.Data)?["toDate"]?.ToString() ?? "2050/12/29", out var toDate);
            if (!parseResult || toDate == default)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, zero or null toDate is provided in data.{Environment.NewLine}", Validate = 0 };
            StartDate = fromDate.Ticks;
            EndDate = toDate.Ticks;
            var device = _deviceService.GetDevice(DeviceId).GetAwaiter().GetResult()?.Data;
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
            var result = _onlineDevices[device.Code].ReadLogOfPeriod(startDateTicks, endDateTicks);
            if (!result.Success) return new ResultViewModel{Success = result.Success, Code = result.Code, Message = result.Message};
            
            _ = Task.Run(() =>
            {
                foreach (var log in result.Data)
                {
                    Logger.Log($@"nReaderIdn : {device.Code}
    EventId : {log.EventLog.Code}
    nDateTime : {log.DateTimeTicks}
    DateTime : {log.LogDateTime}
    TnaEvent : {log.TnaEvent}
    SubEvent : {log.SubEvent.Code}
    sUserID : {log.UserId}
    _matchingTypes:{log.MatchingType.Code}", logType: LogType.Information);
                }
            }).ConfigureAwait(false);

            return new ResultViewModel { Success = result.Success, Code = result.Code, Message = result.Message };
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
