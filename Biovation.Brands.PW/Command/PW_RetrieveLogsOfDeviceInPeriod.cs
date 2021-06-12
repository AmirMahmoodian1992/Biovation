using Biovation.Brands.PW.Devices;
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

namespace Biovation.Brands.PW.Command
{
    public class RetrieveLogsOfDeviceInPeriod : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }
        private int DeviceId { get; }

        private TaskItem TaskItem { get; }
        private readonly DeviceService _deviceService;

        public RetrieveLogsOfDeviceInPeriod(TaskItem taskItem, Dictionary<uint, Device> devices, DeviceService deviceService)
        {
            TaskItem = taskItem;
            OnlineDevices = devices;
            DeviceId = taskItem.DeviceId;
            _deviceService = deviceService;

            //if (items.Count == 1)
            //{ DeviceId = Convert.ToInt32(items[0]); }
            //else
            //{
            //    DeviceId = Convert.ToInt32(items[0]);
            //    TaskItemId = Convert.ToInt32(items[1]);
            //}

            //Code = (int)(deviceService.GetDevices(code: (uint)DeviceId, brandId: DeviceBrands.ProcessingWorldCode)?.FirstOrDefault()?.Code ?? 0);
        }

        public object Execute()
        {
            if (TaskItem is null)
                return new ResultViewModel { Id = TaskItem?.Id ?? 0, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem?.Id ?? 0}.{Environment.NewLine}", Validate = 0 };

            var deviceInfo = _deviceService.GetDevice(DeviceId);
            if (deviceInfo is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (OnlineDevices.All(onlineDevice => onlineDevice.Key != deviceInfo.Code))
            {
                Logger.Log($"The device: {deviceInfo.Code} is not connected.");
                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = DeviceId, Message = $"Device {deviceInfo.Code} is disconnected", Validate = 0 };
            }

            DateTime fromDate;
            DateTime toDate;

            try
            {
                fromDate = (DateTime)JsonConvert.DeserializeObject<JObject>(TaskItem.Data)["fromDate"];
                toDate = (DateTime)JsonConvert.DeserializeObject<JObject>(TaskItem.Data)["toDate"];
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                fromDate = new DateTime(1970, 1, 1);
                toDate = DateTime.Now.AddDays(1);
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == deviceInfo.Code).Value;
                device.ReadOfflineLogInPeriod(new object(), fromDate, toDate);

                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DoneCode), Id = DeviceId, Message = $@"تخلیه دستگاه {device.GetDeviceInfo().Code} شروع شد", Validate = 1 };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, {exception}.{Environment.NewLine}", Validate = 0 };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Read All Log of Device";
        }

        public string GetDescription()
        {
            return "Read All Log of Device";
            //return $"Read All Log of Device: {Code}.";
        }
    }
}
