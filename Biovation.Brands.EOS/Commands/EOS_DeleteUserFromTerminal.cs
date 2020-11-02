using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using Biovation.Constants;
using Biovation.Domain;

namespace Biovation.Brands.EOS.Commands
{
    class EOSDeleteUserFromTerminal : ICommand
    {
        // private Dictionary<uint, Device> _onlineDevices { get; }
        private readonly Dictionary<uint, Device> _onlineDevices;
        private uint DeviceId { get; }

        private uint UserId { get; }

        private readonly TaskStatuses _taskStatuses;
        public EOSDeleteUserFromTerminal(uint deviceId, Dictionary<uint, Device> onlineDevices, uint userId, TaskStatuses taskStatuses)
        {
            DeviceId = deviceId;
            UserId = userId;
            _taskStatuses = taskStatuses;
            _onlineDevices = onlineDevices;
        }

        public object Execute()
        {
            if (!_onlineDevices.ContainsKey(Convert.ToUInt32(DeviceId)))
            {
                return null;
            }

            var successDeleteUser = _onlineDevices[DeviceId].DeleteUser(UserId);
            if (successDeleteUser) return new ResultViewModel { Id = DeviceId, Message = "Successfully deleted", Validate = 1, Code = Convert.ToInt64(_taskStatuses.Done.Code) };
            return new ResultViewModel { Id = DeviceId, Message = "UnSuccessfully deleted", Validate = 0, Code = Convert.ToInt64(_taskStatuses.Done.Code) };
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
            return $"[EOS]: Deleting user: {UserId} from device: {DeviceId}.";
        }
    }
}
