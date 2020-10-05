using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Models.ConstantValues;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Brands.ZK.Command
{
    public class ZKSendUserToDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }
        private readonly LogService _logService = new LogService();

        private int DeviceId { get; }
        private int TaskItemId { get; }

        private uint Code { get; }
        private int UserId { get; }
        private User UserObj { get; }

        private readonly UserService _userService = new UserService();
        private readonly DeviceService _deviceService = new DeviceService();
        private readonly TaskService _taskService = new TaskService();
        private readonly AdminDeviceService _adminDeviceService = new AdminDeviceService();

        public ZKSendUserToDevice(IReadOnlyList<object> items, Dictionary<uint, Device> devices)
        {

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = _deviceService.GetDeviceBasicInfoByIdAndBrandId(DeviceId, DeviceBrands.ZkTecoCode)?.Code ?? 0;
            var taskItem = _taskService.GetTaskItem(TaskItemId).Result;
            var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            UserId = (int)data["UserId"];
            UserObj = _userService.GetUser(UserId, false);
            OnlineDevices = devices;
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnected.Code) };
            }

            if (UserObj == null)
            {
                Logger.Log($"User {UserId} does not exist.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"User {UserId} does not exist.", Code = Convert.ToInt64(TaskStatuses.DeviceDisconnected.Code) };
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var adminDevices = _adminDeviceService.GetAdminDevicesByUserId(UserId);
                UserObj.IsAdmin = adminDevices.Any(x => x.DeviceId == DeviceId);
                var result = device.TransferUser(UserObj);
                var log = new Log
                {
                    DeviceId = DeviceId,
                    LogDateTime = DateTime.Now,
                    EventLog = LogEvents.AddUserToDevice,
                    UserId = UserId,
                    MatchingType = MatchingTypes.Finger,
                    SubEvent = LogSubEvents.Normal,
                    TnaEvent = 0
                };

                _logService.AddLog(log);
                return new ResultViewModel { Validate = result ? 1 : 0, Id = DeviceId, Message = $"User {UserId} Send", Code = Convert.ToInt64(TaskStatuses.Done.Code) };

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
            return "Send user to device";
        }

        public string GetDescription()
        {
            return $"Sending user: {UserId} to device: {Code}.";
        }
    }
}
