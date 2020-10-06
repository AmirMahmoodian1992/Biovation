using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZone = Biovation.CommonClasses.Models.TimeZone;

namespace Biovation.Brands.Suprema.Commands
{
    class SupremaSendTimeZoneToTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }
        private int TimeZoneId { get; set; }
        private TimeZone TimeZoneObj { get; }

        private readonly TimeZoneService _timeZoneService = new TimeZoneService();

        public SupremaSendTimeZoneToTerminal(uint code, int timeZoneId, Dictionary<uint, Device> devices)
        {
            DeviceId = devices.FirstOrDefault(dev => dev.Key == code).Value.GetDeviceInfo().DeviceId;
            TimeZoneId = timeZoneId;
            TimeZoneObj = _timeZoneService.GetTimeZoneById(timeZoneId);
            OnlineDevices = devices;
            Code = code;
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Value.GetDeviceInfo().DeviceId != DeviceId))
            {
                Console.WriteLine($"[Suprema] : The device: {Code} is not connected.");
                return false;
            }

            if (TimeZoneObj == null)
            {
                Console.WriteLine($"[Suprema] : TimeZone {TimeZoneId} does not exist.");
                return false;
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.TransferTimeZone(TimeZoneId);
                return result;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return false;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public string GetTitle()
        {
            return "[Suprema]: Send timeZone to terminal";
        }

        public string GetDescription()
        {
            return $"[Suprema]: Sending timeZone: {TimeZoneId} to device: {Code}.";
        }
    }
}
