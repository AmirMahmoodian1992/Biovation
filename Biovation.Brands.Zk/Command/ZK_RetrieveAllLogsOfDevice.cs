using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Biovation.Brands.ZK.Command
{
    public class ZkRetrieveAllLogsOfDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }

        public ZkRetrieveAllLogsOfDevice(IReadOnlyList<object> items, Dictionary<uint, Device> devices, DeviceService deviceService)
        {
            OnlineDevices = devices;
            DeviceId = Convert.ToInt32(items[0]);
            Code = (deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);
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
                var result = device.ReadOfflineLogInPeriod(new CancellationTokenSource().Token, default, default);
                return result;

                //ZKTecoServer.LogReaderQueue.Enqueue(new Task(() => device.ReadOfflineLogInPeriod(new object(), new DateTime(1990, 1 , 1).ToString(CultureInfo.InvariantCulture), DateTime.Now.ToString(CultureInfo.InvariantCulture))));
                //ZKTecoServer.StartReadLogs();
                //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                //var task = new TaskInfo
                //{
                //    CreatedAt = DateTimeOffset.Now,
                //    CreatedBy = creatorUser,
                //    TaskType = TaskTypes.GetLogsInPeriod,
                //    Priority = _taskPriorities.Medium,
                //    TaskItems = new List<TaskItem>(),
                //    DeviceBrand = DeviceBrands.ZkTeco,

                //};
                //task.TaskItems.Add(new TaskItem
                //{
                //    Status = TaskStatuses.Queued,
                //    TaskItemType = TaskItemTypes.GetLogsInPeriod,
                //    Priority = _taskPriorities.Medium,
                //    DueDate = DateTimeOffset.Now,
                //    DeviceId = DeviceId,
                //    Data = JsonConvert.SerializeObject(new { fromDate = new DateTime(1990, 1, 1).ToString(CultureInfo.InvariantCulture), toDate = DateTime.Now.ToString(CultureInfo.InvariantCulture) }),
                //    IsParallelRestricted = true,
                //    IsScheduled = false,
                //    OrderIndex = 1,

                //});
                //_taskService.InsertTask(task);
                //ZKTecoServer.ProcessQueue();

                //return new ResultViewModel { Id = device.GetDeviceInfo().Code, Validate = 1, Message = $@"تخلیه دستگاه {device.GetDeviceInfo().Code} شروع شد", Code = Convert.ToInt64(TaskStatuses.Queued.Code) };
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
