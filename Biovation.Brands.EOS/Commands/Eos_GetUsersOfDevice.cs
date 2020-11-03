using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using Biovation.Constants;
using Biovation.Domain;

namespace Biovation.Brands.Eos.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    public class SupremaGetUsersOfDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private readonly Dictionary<uint, Device> _onlineDevices;

        private uint DeviceId { get; }

        public SupremaGetUsersOfDevice(uint deviceId, Dictionary<uint, Device> onlineDevices)
        {
            DeviceId = deviceId;
            _onlineDevices = onlineDevices;
  
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
           
            if (!_onlineDevices.ContainsKey(Convert.ToUInt32(DeviceId)))
            {
                return null;
            }

            var usersOfDevice = _onlineDevices[DeviceId].GetAllUsers();


            return new ResultViewModel<List<User>> { Data = usersOfDevice, Id = DeviceId, Message = "0", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
        }

        

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Get users of a device command";
        }

        public string GetDescription()
        {
            return "Getting users of device : " + DeviceId + " command";
        }
    }
}