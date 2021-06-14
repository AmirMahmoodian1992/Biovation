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

namespace Biovation.Brands.ZK.Command
{
    public class ZkRetrieveAllLogsOfDeviceInPeriod : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }
        private string StartDate { get; }
        private string EndDate { get; }
        private int TaskItemId { get; }

        public ZkRetrieveAllLogsOfDeviceInPeriod(IReadOnlyList<object> items, Dictionary<uint, Device> devices, DeviceService deviceService, TaskService taskService)
        {
            OnlineDevices = devices;

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = (deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);
            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);

            DateTime startDate;
            try
            {
                startDate = (DateTime)data["fromDate"];
            }
            catch (Exception)
            {
                startDate = new DateTime(1970, 1, 1);
            }

            DateTime endDate;
            try
            {
                endDate = (DateTime)data["toDate"];
            }
            catch (Exception)
            {
                endDate = DateTime.Now.AddYears(5);
            }

            //StartDate = startDate == default ? new DateTime(1990, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") : startDate.ToString("yyyy-MM-dd HH:mm:ss");
            //EndDate = endDate == default ? new DateTime(1990, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") : endDate.ToString("yyyy-MM-dd HH:mm:ss");

            StartDate = startDate < new DateTime(1970,1,1) ? new DateTime(1970, 1, 1).ToString("yyyy-MM-dd HH:mm:ss") : startDate.ToString("yyyy-MM-dd HH:mm:ss");
            EndDate = endDate > DateTime.Now.AddYears(5) ? DateTime.Now.AddYears(5).ToString("yyyy-MM-dd HH:mm:ss") : endDate.ToString("yyyy-MM-dd HH:mm:ss");

        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $@"ارتباط با دستگاه {Code} برقرار نیست", Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.ReadOfflineLogInPeriod(new object(), StartDate, EndDate);
                return result;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $@"تخلیه دستگاه {Code} انجام نشد ", Code = Convert.ToInt64(TaskStatuses.FailedCode) };
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
            return $"Read All Log of Device: {Code}.";
        }
    }
}
