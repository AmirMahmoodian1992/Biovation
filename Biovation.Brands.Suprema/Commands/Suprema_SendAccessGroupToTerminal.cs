using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biovation.Brands.Suprema.Commands
{
    class SupremaSendAccessGroupToTerminal : ICommand
    {
        /// <summary>
        /// All connected devices
        /// </summary>
        private Dictionary<uint, Device> OnlineDevices { get; }

        private int DeviceId { get; }
        private uint Code { get; }
        private int AccessGroupId { get; set; }
        private AccessGroup AccessGroupObj { get; }

        private readonly AccessGroupService _accessGroupService = new AccessGroupService();

        public SupremaSendAccessGroupToTerminal(uint code, int accessGroupId, Dictionary<uint, Device> devices)
        {
            Code = code;
            AccessGroupId = accessGroupId;
            AccessGroupObj = _accessGroupService.GetAccessGroupById(accessGroupId);
            OnlineDevices = devices;
            DeviceId = devices.FirstOrDefault(dev => dev.Key == code).Value.GetDeviceInfo().DeviceId;
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
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
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
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
