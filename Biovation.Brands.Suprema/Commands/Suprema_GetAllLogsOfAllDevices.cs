using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;

namespace Biovation.Brands.Suprema.Commands
{
    public class SupremaGetLogsOfAllDevices : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }


        public SupremaGetLogsOfAllDevices(Dictionary<uint, Device> devices)
        {
            OnlineDevices = devices;
        }

        /// <summary>
        /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
        /// </summary>
        /// <seealso cref="ICommand" />
        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            //var deviceId = Convert.ToUInt32(itemsList[0]);
            //var startDate = itemsList[1].ToString();
            //var endDate = itemsList[2].ToString();

            foreach (var device in OnlineDevices)
            {
                var refDate = new DateTime(1970, 1, 1).Ticks / 10000000;
                var startDateTicks =
                    Convert.ToInt32(Convert.ToInt64(DateTime.Today.AddDays(-1).Ticks) / 10000000 - refDate);
                var endDateTicks = Convert.ToInt32(Convert.ToInt64(DateTime.Today.AddDays(1).Ticks) / 10000000 - refDate);


                OnlineDevices[device.Value.GetDeviceInfo().Code].ReadLogOfPeriod(startDateTicks, endDateTicks);
                Logger.Log($"Device: {device.Value.GetDeviceInfo().DeviceId} is discharged.");

                return true;
            }

            return null;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return " Get all logs of all devices command";
        }

        public string GetDescription()
        {
            return " Getting all logs of all devices command.";
        }
    }
}
