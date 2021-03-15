using Biovation.CommonClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;

namespace Biovation.Brands.ZK.Command
{
    public class ZkGetAdditionalData : ICommand
    {
        private readonly DeviceService _deviceService;

        //private uint DeviceId { get; }
        private TaskItem TaskItem { get; }
        private Dictionary<uint, Device> OnlineDevices { get; }
        private int Code { get; }

        public ZkGetAdditionalData(uint code, Dictionary<uint, Device> onlineDevices, TaskItem taskItem, DeviceService deviceService)
        {
            OnlineDevices = onlineDevices;
            TaskItem = taskItem;
            _deviceService = deviceService;

            Code = Convert.ToInt32(code);
        }

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

            try
            {
                var result = OnlineDevices[device.Code].GetAdditionalData(Code);
                return result;
                
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new Dictionary<string, string>();
            }
        }

        public string GetDescription()
        {
            return $"GetAdditionalData from Device {Code}";
        }

        public string GetTitle()
        {
            return $"GetAdditionalData from Device {Code}";
        }

        public void Rollback()
        {
            
        }
    }
}
