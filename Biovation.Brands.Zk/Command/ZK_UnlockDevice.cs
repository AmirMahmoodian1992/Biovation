using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Brands.ZK.Command
{
    public class ZKUnlockDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private int TaskItemId { get; }
        private uint Code { get; }

        private readonly DeviceService _deviceService = new DeviceService();
        private static readonly TaskService _taskService = new TaskService();
        public ZKUnlockDevice(IReadOnlyList<object> items, Dictionary<uint, Device> devices)
        {
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = _deviceService.GetDeviceBasicInfoByIdAndBrandId(DeviceId, DeviceBrands.ZkTecoCode)?.Code ?? 0; ;
            var taskItem = _taskService.GetTaskItem(TaskItemId).Result;
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            OnlineDevices = devices;
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnected.Code) };
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.UnLockDevice();
                if (result)
                {
                    Logger.Log($" --> Terminal:{Code} UnLocked(Shutdown)");
                    Logger.Log("   +ErrorCode :" + true + "\n");
                    return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.Done.Code) };
                }
                else
                {
                    Logger.Log("");
                    Logger.Log($"--> Couldn't Terminal:{Code} UnLock(Shutdown)\n");
                    return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.Failed.Code) };
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.Failed.Code) };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Lock device";
        }

        public string GetDescription()
        {
            return "Locking given device.";
        }
    }
}
