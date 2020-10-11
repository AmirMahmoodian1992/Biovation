using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.ZK.Command
{
    public class ZkUnlockDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        //private int TaskItemId { get; }
        private uint Code { get; }

        public ZkUnlockDevice(IReadOnlyList<object> items, Dictionary<uint, Device> devices, DeviceService deviceService/*, TaskService taskService*/)
        {
            OnlineDevices = devices;

            DeviceId = Convert.ToInt32(items[0]);
            //TaskItemId = Convert.ToInt32(items[1]);
            //TaskItemId = Convert.ToInt32(items[1]);
            Code = (deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);
            //var taskItem = taskService.GetTaskItem(TaskItemId);
            //var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.UnLockDevice();
                if (result)
                {
                    Logger.Log($" --> Terminal:{Code} UnLocked(Shutdown)");
                    Logger.Log("   +ErrorCode :" + true + "\n");
                    return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
                }
                else
                {
                    Logger.Log("");
                    Logger.Log($"--> Couldn't Terminal:{Code} UnLock(Shutdown)\n");
                    return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.FailedCode) };
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
