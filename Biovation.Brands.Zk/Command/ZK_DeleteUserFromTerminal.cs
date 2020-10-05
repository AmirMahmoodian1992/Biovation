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
    public class ZKDeleteUserFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }
        private uint UserId { get; }
        private int TaskItemId { get; }


        private readonly DeviceService _deviceService = new DeviceService();
        private static readonly TaskService _taskService = new TaskService();
        private readonly LogService _logService = new LogService();

        public ZKDeleteUserFromTerminal(IReadOnlyList<object> items, Dictionary<uint, Device> devices)
        {
            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = _deviceService.GetDeviceBasicInfoByIdAndBrandId(DeviceId, DeviceBrands.ZkTecoCode)?.Code ?? 0;
            var taskItem = _taskService.GetTaskItem(TaskItemId).Result;
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            UserId = (uint)(data["userId"]);
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
                Logger.Log("-->Delete user from terminal");

                var device = OnlineDevices.FirstOrDefault(x => x.Key == Code).Value;
                var result = device.DeleteUser(UserId);
                if (result)
                {
                    Logger.Log($"  +User {UserId} successfuly deleted from device: {Code}.\n");
                    var log = new Log
                    {
                        DeviceId = DeviceId,
                        LogDateTime = DateTime.Now,
                        //EventLog = Event.USERREMOVEDFROMDEVICE,
                        EventLog = LogEvents.RemoveUserFromDevie,
                        UserId = UserId,
                        MatchingType = MatchingTypes.Finger,
                        SubEvent = LogSubEvents.Normal,
                        TnaEvent = 0,
                        SuccessTransfer = true
                    };

                    _logService.AddLog(log);

                    return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"+User {UserId} successfuly deleted from device: {Code}.\n", Code = Convert.ToInt64(TaskStatuses.Done.Code) };

                }
                Logger.Log($"  +Cannot delete user {UserId} from device: {Code} \n");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"  +Cannot delete user {UserId} from device: {Code} \n", Code = Convert.ToInt64(TaskStatuses.Failed.Code) };

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = exception.Message, Code = Convert.ToInt64(TaskStatuses.Failed.Code) };

            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Delete user from terminal";
        }

        public string GetDescription()
        {
            return $"Deleting user: {UserId} from device: {DeviceId}.";
        }
    }
}
