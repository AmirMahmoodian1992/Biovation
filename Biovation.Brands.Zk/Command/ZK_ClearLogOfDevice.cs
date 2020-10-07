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
    public class ZkClearLogOfDevice : ICommand
    {
        private Dictionary<uint, Device> OnlineDevices { get; }
        private int Code { get; }
        private string StartDate { get; }
        private string EndDate { get; }
        private int DeviceId { get; }
        private int TaskItemId { get; }

        public ZkClearLogOfDevice(IReadOnlyList<object> items, Dictionary<uint, Device> devices, TaskService taskService, DeviceService deviceService)
        {
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = (int)(deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);

            var taskItem = taskService.GetTaskItem(TaskItemId);
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            StartDate = (string)data["fromDate"];
            EndDate = (string)data[" toDate"];

            OnlineDevices = devices;
        }
        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.", logType: LogType.Warning);
                return new ResultViewModel { Validate = 0, Id = Code, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                device.ReadOfflineLog(new object(), true);
                //ZKTecoServer.LogReaderQueue.Enqueue(new Task(() => device.ReadOfflineLog(new CancellationToken(), true)));
                //ZKTecoServer.StartReadLogs();

                var result = device.ClearLog(Code, StartDate, EndDate);
                if (result)
                {
                    Logger.Log($" -->  logs deleted Terminal:{Code}");
                    return new ResultViewModel { Validate = result ? 1 : 0, Id = Code, Message = $" -->  logs deleted Terminal:{Code}", Code = Convert.ToInt64(TaskStatuses.DoneCode) };

                }
                else
                {
                    Logger.Log($"--> Couldn't logs deleted Terminal:{Code}", logType: LogType.Warning);
                    return new ResultViewModel { Validate = result ? 1 : 0, Id = Code, Message = $"--> Couldn't logs deleted Terminal:{Code}", Code = Convert.ToInt64(TaskStatuses.FailedCode) };

                }

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = Code, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = exception.Message };

            }
        }

        public string GetDescription()
        {
            return $"Clear Log of Device : {Code}";
        }

        public string GetTitle()
        {
            return $"Clear Log of Device : {Code}";
        }

        public void Rollback()
        {

        }
    }
}
