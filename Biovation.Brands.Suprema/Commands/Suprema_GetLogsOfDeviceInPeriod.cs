using System;
using System.Collections.Generic;
using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;

namespace Biovation.Brands.Suprema.Commands
{
    public class SupremaGetLogsOfDeviceInPeriod : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private uint DeviceId { get; }
        private long StartDate { get; }
        private long EndDate { get; }

        public SupremaGetLogsOfDeviceInPeriod(uint deviceId, long startDate, long endDate, Dictionary<uint, Device> devices)
        {
            DeviceId = deviceId;
            StartDate = startDate;
            EndDate = endDate;
            OnlineDevices = devices;
        }

        /// <summary>
        /// کنترل کننده برای تمامی اتفاقات بر روی تمامی و انواع مختلف ساعت ها
        /// </summary>
        /// <summary>
        /// <En>Handles the received event on devices.</En>
        /// <Fa>درخواست دریافت شده را کنترل میکند.</Fa>
        /// </summary>
        public object Execute()
        {
            var refDate = new DateTime(1970, 1, 1).Ticks / 10000000;
            //var startDateTicks = Convert.ToInt32((long)(StartDate / 10000000) - refDate);
            //var endDateTicks = Convert.ToInt32((long)(EndDate / 10000000) - refDate);
            var startDateTicks = Convert.ToInt32((StartDate / 10000000) - refDate);
            var endDateTicks = Convert.ToInt32((EndDate / 10000000) - refDate);

            if (OnlineDevices.ContainsKey(DeviceId))
            {
                return OnlineDevices[DeviceId].ReadLogOfPeriod(startDateTicks, endDateTicks);
            }

            Logger.Log($"Device: {DeviceId} is not connected.");
            return null;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return " Get all logs of a device in a period of time command";
        }

        public string GetDescription()
        {
            return $" Getting all logs of a device (id: {DeviceId} from: {new DateTime(1970, 1, 1).AddTicks(StartDate)} To: {new DateTime(1970, 1, 1).AddTicks(EndDate)}) command";
        }
    }
}
