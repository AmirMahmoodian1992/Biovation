using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Brands.Eos.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    public class EosGetLogsOfDeviceInPeriod : ICommand
    {
        private readonly DeviceService _deviceService;

        //private uint DeviceId { get; }
        private TaskItem TaskItem { get; }
        private Dictionary<uint, Device> OnlineDevices { get; }
        private DateTime FromDate { get; }
        private DateTime ToDate { get; }


        public EosGetLogsOfDeviceInPeriod(TaskItem taskItem, Dictionary<uint, Device> onlineDevices, DeviceService deviceService)
        {
            //DeviceId = deviceId;
            TaskItem = taskItem;
            OnlineDevices = onlineDevices;
            _deviceService = deviceService;
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            FromDate = Convert.ToDateTime(data["fromDate"]);
            ToDate = Convert.ToDateTime(data["toDate"]);
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            if (TaskItem is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}.{Environment.NewLine}", Validate = 0 };

            var deviceId = TaskItem.DeviceId;
            var device = _deviceService.GetDevice(deviceId)?.Data;
            if (device is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!OnlineDevices.ContainsKey(device.Code))
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Message = $"  Enroll User face from device: {device.Code} failed. The device is disconnected.{Environment.NewLine}", Validate = 0 };


            var logs = OnlineDevices[device.Code].ReadLogOfPeriod(FromDate, ToDate);

            return logs == null ? new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.InProgressCode), Id = deviceId, Message = 0.ToString(), Validate = 1 } : new ResultViewModel { Id = deviceId, Message = 0.ToString(), Validate = 1, Code = Convert.ToInt64(TaskStatuses.InProgressCode) };
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Get users of a device command";
        }

        public string GetDescription()
        {
            //return "Getting users of device : " + DeviceId + " command";
            return "Get users of a device command";
        }
    }
}