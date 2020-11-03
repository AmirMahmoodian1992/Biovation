using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Brands.EOS.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Eos.Commands
{
    /// <summary>
    /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
    /// </summary>
    public class EosSendUserToDevice : ICommand
    {
        private User User { get; }
        private uint _deviceId { get; }
        private uint _userId { get; }
        private readonly UserService _userService;
        private readonly TaskStatuses _taskStatuses;
        private readonly AdminDeviceService _adminDeviceService;

        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        public EosSendUserToDevice(uint deviceId, uint userId, Dictionary<uint, Device> onlineDevices, UserService userService, TaskStatuses taskStatuses, AdminDeviceService adminDeviceService)
        {

            _deviceId =deviceId;
            _userId = userId;
            _userService = userService;
            _taskStatuses = taskStatuses;
            _adminDeviceService = adminDeviceService;
            OnlineDevices = onlineDevices;
            User = _userService.GetUsers(userId).FirstOrDefault();

        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {

            if (OnlineDevices.All(device => device.Key != _deviceId))
            {
                Logger.Log($"The device: {_deviceId} is not connected.");
                return new ResultViewModel { Validate = 0, Id = _deviceId, Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            if (User == null)
            {
                Logger.Log($"User {_userId} does not exist.");
                return new ResultViewModel { Validate = 0, Id = _deviceId, Message = $"User {_userId} does not exist.", Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode) };
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == _deviceId).Value;
                var adminDevices = _adminDeviceService.GetAdminDevicesByUserId((int)_userId);
                User.IsAdmin = adminDevices.Any(x => x.DeviceId == _deviceId);
                var result = device.TransferUser(User);


                return new ResultViewModel { Validate = result ? 1 : 0, Id = _deviceId, Message = $"User {_userId} Send", Code = Convert.ToInt64(TaskStatuses.DoneCode) };

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Id = _deviceId, Message = exception.Message, Code = Convert.ToInt64(TaskStatuses.FailedCode) };

            }


            return true;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Sync a user with all devices command";
        }

        public string GetDescription()
        {
            return "Syncing User (id: " + User.Id + " name: " + User.UserName + ") command";
        }
    }
}