using Biovation.Domain;
using System.Collections.Generic;

namespace Biovation.Brands.Paliz.Manager
{
    public class PalizDeviceMappings
    {
        private readonly Dictionary<string, DeviceBasicInfo> _deviceMappings;

        public PalizDeviceMappings()
        {
            _deviceMappings = new Dictionary<string, DeviceBasicInfo>();
        }

        public DeviceBasicInfo GetDeviceBasicInfo(string deviceName)
        {
            return _deviceMappings.ContainsKey(deviceName) ? _deviceMappings[deviceName] : null;
        }

        public void InsertDeviceMapping(string deviceName, DeviceBasicInfo device)
        {
            if (!_deviceMappings.ContainsKey(deviceName))
            {
                _deviceMappings.Add(deviceName, device);
            }
        }
    }
}
