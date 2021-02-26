using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Brands.Suprema.Commands
{
    class SupremaDeleteUserFromTerminal : ICommand
    {
        private TaskItem TaskItem { get; }
        private readonly DeviceService _deviceService;
        private readonly Dictionary<uint, Device> _onlineDevices;

        public SupremaDeleteUserFromTerminal(TaskItem taskItem, Dictionary<uint, Device> onlineDevices, DeviceService deviceService)
        {
            TaskItem = taskItem;
            _onlineDevices = onlineDevices;
            _deviceService = deviceService;
        }

        public object Execute()
        {
            if (TaskItem is null)
                return new ResultViewModel { Id = 0, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item.{Environment.NewLine}", Validate = 0 };

            var deviceId = TaskItem.DeviceId;

            var parseResult = uint.TryParse(JsonConvert.DeserializeObject<JObject>(TaskItem.Data)?["userCode"].ToString() ?? "0", out var userCode);

            if (!parseResult || userCode == 0)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, zero or null user id is provided in data.{Environment.NewLine}", Validate = 0 };

            var device = _deviceService.GetDevice(deviceId)?.Data;
            if (device is null)
                return new ResultViewModel { Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.FailedCode), Message = $"Error in processing task item {TaskItem.Id}, wrong or zero device id is provided.{Environment.NewLine}", Validate = 0 };

            if (!_onlineDevices.ContainsKey(device.Code))
            {
                Logger.Log($"The device: {device.DeviceId} is not connected.");
                return new ResultViewModel { Validate = 0, Id = TaskItem.Id, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            var successDeleteUser = _onlineDevices[device.Code].DeleteUser(userCode);
            if (successDeleteUser) return new ResultViewModel { Id = device.Code, Message = "Successfully deleted", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
            return new ResultViewModel { Id = device.Code, Message = "UnSuccessfully deleted", Validate = 0, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "[Suprema]: Delete user from terminal";
        }

        public string GetDescription()
        {
            return "[Suprema]: Delete user from terminal";
            //return $"[Suprema]: Deleting user: {UserId} from device: {DeviceId}.";
        }
    }
}
