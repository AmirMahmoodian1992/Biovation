using Biovation.Brands.Suprema.Devices.Suprema_Version_1;
using Biovation.Brands.Suprema.Manager;
using Biovation.Brands.Suprema.Model;
using Biovation.Constants;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Suprema.Devices
{
    /// <summary>
    /// ساخت و بازگردانی یک نمونه ساعت، با توجه به نوع مورد نیاز
    /// </summary>
    public class DeviceFactory
    {
        private readonly DeviceService _deviceService;
        private readonly AccessGroupService _accessGroupService;
        private readonly UserCardService _userCardService;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly BiometricTemplateManager _biometricTemplateManager;
        private readonly LogService _logService;

        private readonly TimeZoneService _timeZoneService;
        private readonly SupremaCodeMappings _supremaCodeMappings;

        private readonly DeviceBrands _deviceBrands;
        private readonly MatchingTypes _matchingTypes;
        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;

        public DeviceFactory(DeviceService deviceService, AccessGroupService accessGroupService, UserCardService userCardService, FingerTemplateService fingerTemplateService, FingerTemplateTypes fingerTemplateTypes, FaceTemplateService faceTemplateService, FaceTemplateTypes faceTemplateTypes, BiometricTemplateManager biometricTemplateManager, LogService logService, TimeZoneService timeZoneService, SupremaCodeMappings supremaCodeMappings, DeviceBrands deviceBrands, MatchingTypes matchingTypes, LogEvents logEvents, LogSubEvents logSubEvents)
        {
            _deviceService = deviceService;
            _accessGroupService = accessGroupService;
            _userCardService = userCardService;
            _fingerTemplateService = fingerTemplateService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _faceTemplateService = faceTemplateService;
            _faceTemplateTypes = faceTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;
            _logService = logService;
            _timeZoneService = timeZoneService;
            _supremaCodeMappings = supremaCodeMappings;
            _deviceBrands = deviceBrands;
            _matchingTypes = matchingTypes;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
        }

        /// <summary>
        /// <En>Creates a device instance by device type.</En>
        /// <Fa>با توجه به نوع دستگاه درحال پردازش، یک نمونه از آن ایجاد می کند.</Fa>
        /// </summary>

        /// <returns>Device object</returns>
        public Device Factory(SupremaDeviceModel device/*, ClientConnection clientConnection = null*/)
        {
            switch (device.DeviceTypeId)
            {
                //case BSSDK.BS_DEVICE_BIOMINI_CLIENT:
                //    {
                //        return new BiominiClient(device, clientConnection);
                //    }
                case BSSDK.BS_DEVICE_FSTATION:
                    {
                        return new FaceStation(device, _deviceService, _userCardService
                        , _accessGroupService, _faceTemplateService, _faceTemplateTypes, _matchingTypes, _logEvents, _logService, _timeZoneService, _supremaCodeMappings, _deviceBrands, _logSubEvents);
                    }

                case BSSDK.BS_DEVICE_BIOSTATION:
                    {
                        return new BioStation(device, _logService, _deviceService, _timeZoneService, _accessGroupService, _fingerTemplateService, _fingerTemplateTypes, _userCardService, _supremaCodeMappings, _matchingTypes, _deviceBrands, _biometricTemplateManager, _logEvents, _logSubEvents);
                    }

                case BSSDK.BS_DEVICE_BIOSTATION2:
                    {
                        return new BioStationT2(device, _deviceService, _logService, _accessGroupService, _userCardService, _fingerTemplateService, _fingerTemplateTypes, _timeZoneService, _supremaCodeMappings, _biometricTemplateManager, _deviceBrands);
                    }

                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    {
                        return new OtherDevices(device, _deviceService, _logService, _accessGroupService, _userCardService, _fingerTemplateService, _supremaCodeMappings, _fingerTemplateTypes, _deviceBrands, _timeZoneService, _biometricTemplateManager, _matchingTypes, _logEvents, _logSubEvents);
                    }
                default:
                    return null;
            }
        }
    }
}
