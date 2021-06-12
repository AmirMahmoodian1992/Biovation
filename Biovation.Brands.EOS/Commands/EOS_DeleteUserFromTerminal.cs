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

namespace Biovation.Brands.EOS.Commands
{
    public class EosDeleteUserFromTerminal : ICommand
    {
        //private uint _deviceId { get; }
        //private uint UserId { get; }
        private readonly DeviceService _deviceService;

        private TaskItem TaskItem { get; }
        private Dictionary<uint, Device> OnlineDevices { get; }

        public EosDeleteUserFromTerminal(TaskItem taskItem, Dictionary<uint, Device> onlineDevices, DeviceService deviceService)
        {
            //_deviceId = deviceId;
            //UserId = userId;

            TaskItem = taskItem;
            OnlineDevices = onlineDevices;
            _deviceService = deviceService;
        }

        public object Execute()
        {
            if (TaskItem is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}.{Environment.NewLine}", Validate = 0 };

            var deviceId = TaskItem.DeviceId;
            var parseResult = uint.TryParse(JsonConvert.DeserializeObject<JObject>(TaskItem.Data)?["userCode"].ToString() ?? "0", out var userCode);

            if (!parseResult || userCode == 0)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, zero or null user id is provided in data.{Environment.NewLine}", Validate = 0 };

            var device = _deviceService.GetDevice(deviceId).Result?.Data;
            if (device is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!OnlineDevices.ContainsKey(device.Code))
            {
                Logger.Log($"The device: {device.DeviceId} is not connected.");
                return new ResultViewModel { Validate = 0, Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            var successDeleteUser = OnlineDevices[device.Code].DeleteUser(userCode);
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
