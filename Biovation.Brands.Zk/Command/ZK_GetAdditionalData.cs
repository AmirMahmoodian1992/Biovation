using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.CommonClasses.Interface;

namespace Biovation.Brands.ZK.Command
{
    public class ZkGetAdditionalData : ICommand
    {
        private Dictionary<uint, Device> OnlineDevices { get; }
        private int Code { get; }

        public ZkGetAdditionalData(uint code, Dictionary<uint, Device> devices)
        {
            OnlineDevices = devices;
            Code = Convert.ToInt32(code);
        }

        public object Execute()
        {
            if (OnlineDevices.All(device => device.Key != Code))
            {
                Logger.Log($"The device: {Code} is not connected.");
                return new Dictionary<string, string>();
            }

            try
            {
                var device = OnlineDevices.FirstOrDefault(dev => dev.Key == Code).Value;

                var result = device.GetAdditionalData(Code);
                return result;
                
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new Dictionary<string, string>();
            }
        }

        public string GetDescription()
        {
            return $"GetAdditionalData from Device {Code}";
        }

        public string GetTitle()
        {
            return $"GetAdditionalData from Device {Code}";
        }

        public void Rollback()
        {
            
        }
    }
}
