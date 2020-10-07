using System.Collections.Generic;
using Biovation.Domain;

namespace Biovation.Brands.Shahab.Devices
{
    public class DeviceFactory
    {
        private readonly Dictionary<uint, Device> _connectedCameras;

        public DeviceFactory(Dictionary<uint, Device> connectedCameras)
        {
            _connectedCameras = connectedCameras;
        }

        /// <summary>
        /// <En>Creates a device instance by device type.</En>
        /// <Fa>با توجه به نوع دستگاه درحال پردازش، یک نمونه از آن ایجاد می کند.</Fa>
        /// </summary>
        /// <param name="device">اطلاعات کامل دستگاه</param>
        /// <returns>Device object</returns>
        public Device Factory(DeviceBasicInfo device)
        {
            switch (device.Model.Id)
            {
                //case DeviceModels.IFace202:
                //    {
                //        return new IFace202(device);
                //    }

                default:

                    return new Device(device, _connectedCameras);
            }
        }
    }
}
