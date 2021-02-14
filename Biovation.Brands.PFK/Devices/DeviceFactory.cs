using System.Collections.Generic;
using Biovation.Domain;

namespace Biovation.Brands.PFK.Devices
{
    public class DeviceFactory
    {
        private readonly Dictionary<uint, Camera> _connectedCameras;

        public DeviceFactory(Dictionary<uint, Camera> connectedCameras)
        {
            _connectedCameras = connectedCameras;
        }

        /// <summary>
        /// <En>Creates a device instance by device type.</En>
        /// <Fa>با توجه به نوع دستگاه درحال پردازش، یک نمونه از آن ایجاد می کند.</Fa>
        /// </summary>
        /// <param name="cameraInfo">اطلاعات کامل دستگاه</param>
        /// <returns>Device object</returns>
        public Camera Factory(DeviceBasicInfo cameraInfo)
        {
            switch (cameraInfo.Model.Id)
            {
                //case DeviceModels.IFace202:
                //    {
                //        return new IFace202(device);
                //    }

                default:

                    return new Camera(cameraInfo, _connectedCameras);
            }
        }
    }
}
