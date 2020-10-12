using Biovation.Brands.Suprema.Devices;
using Biovation.CommonClasses.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Domain;

namespace Biovation.Brands.Suprema.Commands
{
    class SupremaLockDevice : ICommand
    {
        private readonly Dictionary<uint, Device> _onlineDevices;

        private int DeviceId { get; }
        private uint Code { get; }

        public SupremaLockDevice(uint code, Dictionary<uint, Device> devices, Dictionary<uint, Device> onlineDevices)
        {
            Code = code;
            _onlineDevices = onlineDevices;
            DeviceId = devices.FirstOrDefault(dev => dev.Key == code).Value.GetDeviceInfo().DeviceId;
            
        }

        public object Execute()
        {
            if (_onlineDevices.All(device => device.Key != Code))
            {
                Console.WriteLine($"[Suprema] : The device: {Code} is not connected.");
                return new ResultViewModel { Validate = 0, Id = DeviceId };
            }

            try
            {
                var device = _onlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;
                var result = device.LockDevice();
                if (result)
                {
                    Console.WriteLine($"[Suprema] --> Terminal:{Code} Locked(Shutdown)");
                    Console.WriteLine("   +ErrorCode :" + true);
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("[Suprema]:");
                    Console.WriteLine($"-->Couldn't Terminal:{Code} Lock(Shutdown)");
                    Console.WriteLine("");
                }

                return new ResultViewModel { Validate = result ? 1 : 0, Id = DeviceId };
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
