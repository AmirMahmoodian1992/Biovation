using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses.Interface;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.Suprema.Commands
{
    class SupremaSendAccessGroupToTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private readonly Dictionary<uint, Device> _onlineDevices;


       // private int DeviceId { get; }
        private uint Code { get; }
        private int AccessGroupId { get; set; }
        private AccessGroup AccessGroupObj { get; }

        public SupremaSendAccessGroupToTerminal(uint code, int accessGroupId, Dictionary<uint, Device> onlineDevices, AccessGroupService accessGroupService)
        {
            Code = code;
            AccessGroupId = accessGroupId;
            _onlineDevices = onlineDevices;
            AccessGroupObj = accessGroupService.GetAccessGroup(accessGroupId);
            _onlineDevices = onlineDevices;
           // DeviceId = onlineDevices.FirstOrDefault(dev => dev.Key == code).Value.GetDeviceInfo().DeviceId;
        }

        public object Execute()
        {
            if (_onlineDevices.All(device => device.Key != Code))
            {
                Console.WriteLine($"[Suprema] : The device: {Code} is not connected.");
                return false;
            }

            if (AccessGroupObj == null)
            {
                Console.WriteLine($"[Suprema] : Access Group {AccessGroupId} does not exist.");
                return false;
            }

            try
            {
                var device = _onlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.TransferAccessGroup(AccessGroupObj.Id);
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
            return "[Suprema]: Send access group to terminal";
        }

        public string GetDescription()
        {
            return $"[Suprema]: Sending access group: {AccessGroupId} to device: {Code}.";
        }
    }
}
