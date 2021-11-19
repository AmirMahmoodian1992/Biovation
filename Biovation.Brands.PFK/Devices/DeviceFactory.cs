using Biovation.Constants;
using Biovation.Service.Api.v2;
using System.Collections.Generic;

namespace Biovation.Brands.PFK.Devices
{
    public class DeviceFactory
    {
        private readonly LogEvents _logEvents;
        private readonly Dictionary<uint, Camera> _connectedCameras;
        private readonly PlateDetectionService _plateDetectionService;

        public DeviceFactory(Dictionary<uint, Camera> connectedCameras, LogEvents logEvents, PlateDetectionService plateDetectionService)
        {
            _logEvents = logEvents;
            _connectedCameras = connectedCameras;
            _plateDetectionService = plateDetectionService;
        }

        /// <summary>
        /// <En>Creates a device instance by device type.</En>
        /// <Fa>با توجه به نوع دستگاه درحال پردازش، یک نمونه از آن ایجاد می کند.</Fa>
        /// </summary>
        /// <param name="cameraInfo">اطلاعات کامل دستگاه</param>
        /// <returns>Device object</returns>
        public Camera Factory(Domain.Camera cameraInfo)
        {
            switch (cameraInfo.Model.Id)
            {
                //case DeviceModels.IFace202:
                //    {
                //        return new IFace202(device);
                //    }

                default:

                    return new Camera(cameraInfo, _connectedCameras, _plateDetectionService, _logEvents);
            }
        }
    }
}
