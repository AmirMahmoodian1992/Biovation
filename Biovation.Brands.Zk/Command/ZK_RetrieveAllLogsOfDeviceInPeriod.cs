using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.ZK.Command
{
    public class ZKRetrieveAllLogsOfDeviceInPeriod : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }
        private string StartDate { get; }
        private string EndDate { get; }
        private readonly DeviceService _deviceService = new DeviceService();
        private readonly TaskService _taskService = new TaskService();
        //private readonly UserService _userService = new UserService();
        private int TaskItemId { get; }
        public ZKRetrieveAllLogsOfDeviceInPeriod(IReadOnlyList<object> items, Dictionary<uint, Device> devices)
        {
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = _deviceService.GetDeviceBasicInfoByIdAndBrandId(DeviceId, DeviceBrands.ZkTecoCode)?.Code ?? 0;
            var taskItem = _taskService.GetTaskItem(TaskItemId).Result;
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);

            var startDate = (DateTime)data["fromDate"];
            var endDate = (DateTime)data["toDate"];

            StartDate = startDate == default ? new DateTime(1990, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") : startDate.ToString("yyyy-MM-dd HH:mm:ss");
            EndDate = endDate == default ? new DateTime(1990, 0, 0).ToString("yyyy-MM-dd HH:mm:ss") : endDate.ToString("yyyy-MM-dd HH:mm:ss");

            OnlineDevices = devices;
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

                //ZKTecoServer.LogReaderQueue.Enqueue(new Task(() => device.ReadOfflineLogInPeriod(new object(), StartDate, EndDate)));

                //ZKTecoServer.StartReadLogs();

                //var creatorUser = _userService.GetUser(123456789, false);
                //var task = new TaskInfo
                //{
                //    CreatedAt = DateTimeOffset.Now,
                //    CreatedBy = creatorUser,
                //    TaskType = TaskTypes.GetLogsInPeriod,
                //    Priority = TaskPriorities.Medium,
                //    TaskItems = new List<TaskItem>(),
                //    DeviceBrand = DeviceBrands.ZkTeco,

                //};
                //task.TaskItems.Add(new TaskItem
                //{
                //    Status = TaskStatuses.Queued,
                //    TaskItemType = TaskItemTypes.GetLogsInPeriod,
                //    Priority = TaskPriorities.Medium,
                //    DueDate = DateTimeOffset.Now,
                //    DeviceId = DeviceId,
                //    Data = JsonConvert.SerializeObject(new { fromDate = StartDate, toDate = EndDate }),
                //    IsParallelRestricted = true,
                //    IsScheduled = false,
                //    OrderIndex = 1,

                //});
                //_taskService.InsertTask(task).Wait();
                //ZKTecoServer.ProcessQueue();
                //return new ResultViewModel { Id = device.GetDeviceInfo().Code, Message = $@"تخلیه دستگاه {device.GetDeviceInfo().Code} شروع شد", Code=Convert.ToInt64(TaskStatuses.Queued.Code) };
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
