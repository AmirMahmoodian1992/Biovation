using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using Biovation.Constants;
using Biovation.Domain;

namespace Biovation.Brands.Suprema.Commands
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
        private readonly TaskStatuses _taskStatuses;



        private uint DeviceId { get; }

        public SupremaGetUsersOfDevice(uint deviceId, TaskStatuses taskStatuses, Dictionary<uint, Device> onlineDevices)
        {
            DeviceId = deviceId;
            _taskStatuses = taskStatuses;
            _onlineDevices = onlineDevices;
  
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            //var DeviceId = Convert.ToUInt32(items.First());

            if (!_onlineDevices.ContainsKey(Convert.ToUInt32(DeviceId)))
            {
                return null;
            }

            var usersOfDevice = _onlineDevices[DeviceId].GetAllUsers();

            /*for (int i = 0; i < 1000; i++)
            {
                userusOfDevice.Add(userusOfDevice[userusOfDevice.Count % 4]);
            }*/

            //return usersOfDevice;
            return new ResultViewModel<List<User>> { Data = usersOfDevice, Id = DeviceId, Message = "0", Validate = 1, Code = Convert.ToInt64(TaskStatuses.DoneCode) };
        }

        //public void Execute(List<object> items, Dictionary<uint, Device> devices, ClientConnection sender)
        //{
        //    var deviceId = Convert.ToUInt32(items.First());

        //    if (!devices.ContainsKey(Convert.ToUInt32(deviceId)))
        //    {
        //        sender.Send("Device is offline");

        //        sender.Disconnect();
        //        return;
        //    }

        //    var usersOfDevice = devices[deviceId].GetAllUsers();

        //    /*for (int i = 0; i < 1000; i++)
        //    {
        //        userusOfDevice.Add(userusOfDevice[userusOfDevice.Count % 4]);
        //    }*/

        //    var data = JsonConvert.SerializeObject(usersOfDevice);

        //    sender.Send(data);
        //    sender.Disconnect();
        //}

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