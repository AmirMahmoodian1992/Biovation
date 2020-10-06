using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using Biovation.Constants;
using Biovation.Domain;

namespace Biovation.Brands.Suprema.Commands
{
    class SupremaDeleteUserFromTerminal : ICommand
    {
        private Dictionary<uint, Device> OnlineDevices { get; }

        private uint DeviceId { get; }

        private uint UserId { get; }

        private readonly TaskStatuses _taskStatuses;
        public SupremaDeleteUserFromTerminal(uint deviceId, Dictionary<uint, Device> devices, uint userId, TaskStatuses taskStatuses)
        {
            DeviceId = deviceId;
            OnlineDevices = devices;
            UserId = userId;
            _taskStatuses = taskStatuses;
        }

        public object Execute()
        {
            if (!OnlineDevices.ContainsKey(Convert.ToUInt32(DeviceId)))
            {
                return null;
            }

            var successDeleteUser = OnlineDevices[DeviceId].DeleteUser(UserId);
            if (successDeleteUser) return new ResultViewModel { Id = DeviceId, Message = "Successfully deleted", Validate = 1, Code = Convert.ToInt64(_taskStatuses.Done.Code) };
            return new ResultViewModel { Id = DeviceId, Message = "UnSuccessfully deleted", Validate = 0, Code = Convert.ToInt64(_taskStatuses.Done.Code) };
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
            return $"[Suprema]: Deleting user: {UserId} from device: {DeviceId}.";
        }
    }
}
