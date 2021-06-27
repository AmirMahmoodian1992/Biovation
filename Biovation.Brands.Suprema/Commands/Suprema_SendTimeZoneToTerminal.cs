using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses.Interface;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using TimeZone = Biovation.Domain.TimeZone;


namespace Biovation.Brands.Suprema.Commands
{
    class SupremaSendTimeZoneToTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private readonly Dictionary<uint, Device> _onlineDevices;


        private int DeviceId { get; }
        private uint Code { get; }
        private int TimeZoneId { get; set; }
        private TimeZone TimeZoneObj { get; }

        public SupremaSendTimeZoneToTerminal(uint code, int timeZoneId, TimeZoneService timeZoneService, Dictionary<uint, Device> onlineDevices)
        {
            DeviceId = onlineDevices.FirstOrDefault(dev => dev.Key == code).Value.GetDeviceInfo().DeviceId;
            TimeZoneId = timeZoneId;
            _onlineDevices = onlineDevices;
            TimeZoneObj = timeZoneService.TimeZones(timeZoneId);
            _onlineDevices = onlineDevices;
            Code = code;
        }

        public object Execute()
        {
            if (_onlineDevices.All(device => device.Value.GetDeviceInfo().DeviceId != DeviceId))
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
                var device = _onlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
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
