using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;

namespace Biovation.Brands.Suprema.Commands
{
    public class SupremaGetOnlineDevices : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private readonly Dictionary<uint, Device> _onlineDevices;



        public SupremaGetOnlineDevices(Dictionary<uint, Device> onlineDevices)
        {
            _onlineDevices = onlineDevices;

        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {


            var deviceList = new List<int>();

            foreach (var device in _onlineDevices)
            {
                deviceList.Add(Convert.ToInt32(device.Value.GetDeviceInfo().DeviceId));
            }

            Newtonsoft.Json.JsonConvert.SerializeObject(deviceList);

            // Sender.Send(data);
            //Sender.Disconnect();

            return true;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "Get online devices command";
        }

        public string GetDescription()
        {
            return " Get suprema online devices command";
        }
    }
}
