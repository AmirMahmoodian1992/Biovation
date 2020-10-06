using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biovation.CommonClasses.Models;

namespace Biovation.Brands.Suprema.Commands
{
    class SupremaLockDevice : ICommand
    {
        private Dictionary<uint, Device> OnlineDevices { get; }
        private int DeviceId { get; }
        private uint Code { get; }

        public SupremaLockDevice(uint code, Dictionary<uint, Device> devices)
        {
            Code = code;
            DeviceId = devices.FirstOrDefault(dev => dev.Key == code).Value.GetDeviceInfo().DeviceId;
            OnlineDevices = devices;
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Console.WriteLine($"[Suprema] : The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId };
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.LockDevice();
                if (result)
                {
                    Console.WriteLine($"[Suprema] --> Terminal:{Code} Locked(Shutdown)");
                    Console.WriteLine("   +ErrorCode :" + result);
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("[Suprema]:");
                    Console.WriteLine($"-->Couldn't Terminal:{Code} Lock(Shutdown)");
                    Console.WriteLine("");
                }

                return new ResultViewModel { Validate = result ? 1 : 0, Id = DeviceId }; ;
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
            return "[Suprema]: Lock device";
        }

        public string GetDescription()
        {
            return "[Suprema]: Locking given device.";
        }
    }
}
