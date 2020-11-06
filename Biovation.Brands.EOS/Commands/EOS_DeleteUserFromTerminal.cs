using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.EOS.Commands
{
    public class EosDeleteUserFromTerminal : ICommand
    {
        //private uint _deviceId { get; }
        //private uint UserId { get; }
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;

        private TaskItem TaskItem { get; }
        private Dictionary<uint, Device> OnlineDevices { get; }

        public EosDeleteUserFromTerminal(TaskItem taskItem, Dictionary<uint, Device> onlineDevices, UserService userService, DeviceService deviceService)
        {
            //_deviceId = deviceId;
            //UserId = userId;

            TaskItem = taskItem;
            OnlineDevices = onlineDevices;
            _userService = userService;
            _deviceService = deviceService;
        }

        public object Execute()
        {
            if (TaskItem is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}.{Environment.NewLine}", Validate = 0 };

            var deviceId = TaskItem.DeviceId;
            var parseResult = uint.TryParse(JsonConvert.DeserializeObject<JObject>(TaskItem.Data)?["userId"].ToString() ?? "0", out var userId);

            if (!parseResult || userId == 0)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, zero or null user id is provided in data.{Environment.NewLine}", Validate = 0 };

            var device = _deviceService.GetDevice(deviceId)?.Data;
            if (device is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!OnlineDevices.ContainsKey((uint)device.DeviceId))
            {
                Logger.Log($"The device: {device.DeviceId} is not connected.");
                return new ResultViewModel { Validate = 0, Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            var user = _userService.GetUsers(userId: userId)?.Data?.Data.FirstOrDefault();

            if (user == null)
            {
                Logger.Log($"User {userId} does not exist.");
                return new ResultViewModel { Validate = 0, Id = device.DeviceId, Message = $"User {userId} does not exist.", Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            var successDeleteUser = OnlineDevices[(uint)device.DeviceId].DeleteUser((uint)user.Code);
            if (successDeleteUser) return new ResultViewModel { Id = TaskItem.Id, Message = "Successfully deleted", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
            return new ResultViewModel { Id = TaskItem.Id, Message = "UnSuccessfully deleted", Validate = 0, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "[EOS]: Delete user from terminal";
        }

        public string GetDescription()
        {
            //return $"[EOS]: Deleting user: {UserId} from device: { _deviceId}.";
            return "[EOS]: Delete user from terminal";
        }
    }
}
