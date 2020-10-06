using System;
using System.Collections.Generic;
using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.SocketHandler;

namespace Biovation.Brands.Suprema.Commands
{
    public class SupremaGetOnlineDevices : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private ClientConnection Sender { get; }

        public SupremaGetOnlineDevices(Dictionary<uint, Device> devices, ClientConnection sender)
        {
            OnlineDevices = devices;
            Sender = sender;
        }

        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            if (Sender == null)
            {
                Logger.Log("Get Online Devices Command:" +
                                  "Error: No connection has been set for this command!");

                return false;
            }

            var deviceList = new List<int>();

            foreach (var device in OnlineDevices)
            {
                deviceList.Add(Convert.ToInt32(device.Value.GetDeviceInfo().DeviceId));
            }

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(deviceList);

            Sender.Send(data);
            Sender.Disconnect();

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
