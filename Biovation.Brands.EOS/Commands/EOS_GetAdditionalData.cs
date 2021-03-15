using System;
using System.Collections.Generic;
using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;

namespace Biovation.Brands.EOS.Commands
{
    public class EOSGetAdditionalData : ICommand
    {
        private readonly DeviceService _deviceService;

        //private uint DeviceId { get; }
        private TaskItem TaskItem { get; }
        private Dictionary<uint, Device> OnlineDevices { get; }

        public EOSGetAdditionalData(Dictionary<uint, Device> onlineDevices, TaskItem taskItem, DeviceService deviceService)
        {
            OnlineDevices = onlineDevices;
            TaskItem = taskItem;
            _deviceService = deviceService;
        }

        public object Execute()
        {
            if (TaskItem is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}.{Environment.NewLine}", Validate = 0 };

            var deviceId = TaskItem.DeviceId;
            var device = _deviceService.GetDevice(deviceId)?.Data;
            if (device is null)
                return new Dictionary<string, string>();

            if (!OnlineDevices.ContainsKey(device.Code))
                return new Dictionary<string, string>();

            try
            {
                var result = OnlineDevices[device.Code].GetAdditionalData((int)device.Code);
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
            return $"GetAdditionalData from Device";
        }

        public string GetTitle()
        {
            return $"GetAdditionalData from Device";
        }

        public void Rollback()
        {
            
        }
    }
}
