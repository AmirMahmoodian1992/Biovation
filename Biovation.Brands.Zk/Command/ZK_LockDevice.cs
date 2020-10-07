using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.ZK.Command
{
    public class ZKLockDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }
        private int DeviceId { get; }
        private uint Code { get; }

        private readonly DeviceService _deviceService;
        private readonly TaskService TaskService ;

        public ZKLockDevice(IReadOnlyList<object> items, Dictionary<uint, Device> devices, DeviceService deviceService, TaskService taskService)
        {
            DeviceId = Convert.ToInt32(items[0]);
            Code = (_deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);
            OnlineDevices = devices;
            _deviceService = deviceService;
            TaskService = taskService;
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
                var result = device.LockDevice();
                if (result)
                {
                    Logger.Log($" --> Terminal:{Code} Locked(Shutdown)");
                    Logger.Log("   +ErrorCode :" + result + "\n");
                    return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
                }
                else
                {
                    Logger.Log("");
                    Logger.Log($"-->Couldn't Terminal:{Code} Lock(Shutdown)\n");
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
