using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Eos.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    public class EosGetUsersOfDevice : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private readonly Dictionary<uint, Device> _onlineDevices;
        private readonly DeviceService _deviceService;

        private uint DeviceId { get; }

        public EosGetUsersOfDevice(uint deviceId, Dictionary<uint, Device> onlineDevices)
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
            var device = _deviceService.GetDevice(DeviceId);

            if (!_onlineDevices.ContainsKey(device.Code))
                return new ResultViewModel { Id = DeviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Message = $"  Enroll User face from device: {device.Code} failed. The device is disconnected.{Environment.NewLine}", Validate = 0 };


            var usersOfDevice = _onlineDevices[device.Code].GetAllUsers();


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