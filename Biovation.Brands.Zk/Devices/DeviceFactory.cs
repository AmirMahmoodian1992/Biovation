using Biovation.Brands.ZK.Model;
using Biovation.CommonClasses.Models;
using Biovation.Domain;

namespace Biovation.Brands.ZK.Devices
{
    public static class DeviceFactory
    {
        /// <summary>
        /// <En>Creates a device instance by device type.</En>
        /// <Fa>با توجه به نوع دستگاه درحال پردازش، یک نمونه از آن ایجاد می کند.</Fa>
        /// </summary>
        /// <param name="device">اطلاعات کامل دستگاه</param>
        /// <returns>Device object</returns>
        public static Device Factory(DeviceBasicInfo device)
        {
            switch (device.Model.Id)
            {
                //case DeviceModels.IFace202:
                //    {
                //        return new IFace202(device);
                //    }

                default:

                    return new Device(device);
            }
        }
    }
}
