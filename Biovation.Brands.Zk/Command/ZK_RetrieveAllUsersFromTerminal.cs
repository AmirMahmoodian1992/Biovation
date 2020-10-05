using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;

namespace Biovation.Brands.ZK.Command
{
    public class ZKRetrieveUsersListFromTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }

        private readonly DeviceService _deviceService = new DeviceService();
        public ZKRetrieveUsersListFromTerminal(IReadOnlyList<object> items, Dictionary<uint, Device> devices)
        {
            DeviceId = Convert.ToInt32(items[0]);
            Code = _deviceService.GetDeviceBasicInfoByIdAndBrandId(DeviceId, DeviceBrands.ZkTecoCode)?.Code ?? 0;
            DeviceId = devices.FirstOrDefault(dev => dev.Key == Code).Value.GetDeviceInfo().DeviceId;
            OnlineDevices = devices;
        }
        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new List<User>();
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.GetAllUserInfo();
                return new ResultViewModel<List<User>> { Data = result, Id = DeviceId, Message = "0", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new List<User>();
            }
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Retrieve all users from terminal";
        }

        public string GetDescription()
        {
            return $"Retrieving all users from device: {Code}.";
        }
    }
}
