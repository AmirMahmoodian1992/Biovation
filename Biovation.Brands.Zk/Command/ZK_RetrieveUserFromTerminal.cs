using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Brands.ZK.Command
{
    public class ZKRetrieveUserFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private int UserId { get; }
        private uint Code { get; }
        private int TaskItemId { get; }
        private readonly TaskService _taskService = new TaskService();
        private readonly DeviceService _deviceService = new DeviceService();

        public ZKRetrieveUserFromTerminal(IReadOnlyList<object> items, Dictionary<uint, Device> devices)
        {
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = _deviceService.GetDeviceBasicInfoByIdAndBrandId(DeviceId, DeviceBrands.ZkTecoCode)?.Code ?? 0;
            var taskItem = _taskService.GetTaskItem(TaskItemId).Result;
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            UserId = (int)(data["userId"]);
            DeviceId = devices.FirstOrDefault(dev => dev.Key == Code).Value.GetDeviceInfo().DeviceId;
            OnlineDevices = devices;
        }
        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"The device: {Code} is not connected.", Code = Convert.ToInt64(TaskStatuses.DeviceDisconnected.Code) };
            }


            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.GetAndSaveUser(UserId);
                return new ResultViewModel { Validate = result ? 1 : 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.Done.Code) };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate =  0, Id = DeviceId,Message = exception.Message,Code = Convert.ToInt64(TaskStatuses.Failed.Code) };
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Retrieve user from terminal";
        }

        public string GetDescription()
        {
            return $"Retrieving user: {UserId} from device: {Code}.";
        }
    }
}
