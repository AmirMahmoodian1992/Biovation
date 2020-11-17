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
    public class ZkSendUserToDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }
        
        private readonly LogService _logService;

        private int DeviceId { get; }
        private int TaskItemId { get; }

        private uint Code { get; }
        private uint UserId { get; set; }
        private User UserObj { get; set; }

        private readonly LogEvents _logEvents;
        private readonly UserService _userService;
        private readonly TaskService _taskService;
        private readonly LogSubEvents _logSubEvents;
        private readonly DeviceService _deviceService;
        private readonly MatchingTypes _matchingTypes;
        private readonly AdminDeviceService _adminDeviceService;

        public ZkSendUserToDevice(IReadOnlyList<object> items, Dictionary<uint, Device> devices, LogService logService, UserService userService, DeviceService deviceService, TaskService taskService, AdminDeviceService adminDeviceService, LogEvents logEvents, LogSubEvents logSubEvents, MatchingTypes matchingTypes)
        {

            DeviceId = Convert.ToInt32(items[0]);
            TaskItemId = Convert.ToInt32(items[1]);
            Code = (_deviceService.GetDevices(brandId: DeviceBrands.ZkTecoCode).FirstOrDefault(d => d.DeviceId == DeviceId)?.Code ?? 0);

            OnlineDevices = devices;
            _logService = logService;
            _userService = userService;
            _deviceService = deviceService;
            _taskService = taskService;
            _adminDeviceService = adminDeviceService;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _matchingTypes = matchingTypes;
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            var taskItem = _taskService.GetTaskItem(TaskItemId);
            var taskData = JsonConvert.DeserializeObject<JObject>(taskItem.Data);
            if (taskData != null && taskData.ContainsKey("userId"))
            {
                var parseResult = uint.TryParse(taskData["userId"].ToString() ?? "0", out var userId);

                if (!parseResult || userId == 0)
                    return new ResultViewModel
                    {
                        Id = taskItem.Id,
                        Code = Convert.ToInt64(TaskStatuses.FailedCode),
                        Message =
                            $"Error in processing task item {taskItem.Id}, zero or null user id is provided in data.{Environment.NewLine}",
                        Validate = 0
                    };

                UserId = userId;
                UserObj = _userService.GetUsers(UserId, getTemplatesData: true)?.FirstOrDefault();

                if (UserObj == null)
                {
                    Logger.Log($"User {UserId} does not exist.");
                    return new ResultViewModel { Validate = 0, Id = taskItem.Id, Message = $"User {UserId} does not exist.", Code = Convert.ToInt64(TaskStatuses.FailedCode) };
                }
            }
            else
            {
                try
                {
                    UserObj = JsonConvert.DeserializeObject<User>(taskItem.Data);
                }
                catch (Exception)
                {
                    Logger.Log($"Bad user data in task item {taskItem.Id}.");
                    return new ResultViewModel { Validate = 0, Id = taskItem.Id, Message = $"Bad user data in task item {taskItem.Id}.", Code = Convert.ToInt64(TaskStatuses.FailedCode) };
                }
            }

            if (UserObj == null)
            {
                Logger.Log($"User {UserId} does not exist.");
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = $"User {UserId} does not exist.", Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var adminDevices = _adminDeviceService.GetAdminDevicesByUserId((int)UserId);
                UserObj.IsAdmin = adminDevices.Any(x => x.DeviceId == DeviceId);
                var result = device.TransferUser(UserObj);
                var log = new Log
                {
                    DeviceId = DeviceId,
                    LogDateTime = DateTime.Now,
                    EventLog = _logEvents.AddUserToDevice,
                    UserId = UserId,
                    MatchingType = _matchingTypes.Finger,
                    SubEvent = _logSubEvents.Normal,
                    TnaEvent = 0
                };

                _logService.AddLog(log);
                return new ResultViewModel { Validate = result ? 1 : 0, Id = DeviceId, Message = $"User {UserId} Send", Code = Convert.ToInt64(TaskStatuses.DoneCode) };

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = DeviceId, Message = exception.Message, Code = Convert.ToInt64(TaskStatuses.FailedCode) };

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
