using System.Collections.Generic;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using RestSharp;

namespace Biovation.Brands.PFK.Devices
{
    public class DeviceFactory
    {
        private readonly LogEvents _logEvents;
        private readonly Dictionary<uint, Camera> _connectedCameras;
        private readonly PlateDetectionService _plateDetectionService;
        private readonly RestClient _logExternalSubmissionRestClient;

        public DeviceFactory(Dictionary<uint, Camera> connectedCameras, LogEvents logEvents, PlateDetectionService plateDetectionService, RestClient logExternalSubmissionRestClient)
        {
            _logEvents = logEvents;
            _connectedCameras = connectedCameras;
            _plateDetectionService = plateDetectionService;
            _logExternalSubmissionRestClient = logExternalSubmissionRestClient;
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

                    return new Camera(cameraInfo, _connectedCameras,_logExternalSubmissionRestClient, _plateDetectionService, _logEvents);
            }
        }
    }
}
