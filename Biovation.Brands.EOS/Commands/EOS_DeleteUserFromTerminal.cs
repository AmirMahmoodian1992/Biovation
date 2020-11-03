using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.CommonClasses;

namespace Biovation.Brands.EOS.Commands
{
    class EosDeleteUserFromTerminal : ICommand
    {
        // private Dictionary<uint, Device> _onlineDevices { get; }
        private readonly Dictionary<uint, Device> _onlineDevices;
        private uint _deviceId { get; }

        private uint UserId { get; }

       
        public EosDeleteUserFromTerminal(uint deviceId, Dictionary<uint, Device> onlineDevices, uint userId)
        {
            _deviceId = deviceId;
            UserId = userId;
            _onlineDevices = onlineDevices;
        }

        public object Execute()
        {
            if (!_onlineDevices.ContainsKey(Convert.ToUInt32(_deviceId)))
            {
                Logger.Log($"The device: {_deviceId} is not connected.");
                return new ResultViewModel { Validate = 0, Id = _deviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };

            }

            var successDeleteUser = _onlineDevices[_deviceId].DeleteUser(UserId);
            if (successDeleteUser) return new ResultViewModel { Id = _deviceId, Message = "Successfully deleted", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
            return new ResultViewModel { Id = _deviceId, Message = "UnSuccessfully deleted", Validate = 0, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
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
            return $"[EOS]: Deleting user: {UserId} from device: { _deviceId}.";
        }
    }
}
