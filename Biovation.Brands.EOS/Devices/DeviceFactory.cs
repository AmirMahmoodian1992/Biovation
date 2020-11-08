using Biovation.Brands.EOS.Manager;
using Biovation.Brands.EOS.Service;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using RestSharp;

namespace Biovation.Brands.EOS.Devices
{
    /// <summary>
    /// ساخت و بازگردانی یک نمونه ساعت، با توجه به نوع مورد نیاز
    /// </summary>
    public class DeviceFactory
    {
        private readonly EosLogService _eosLogService;

        private readonly RestClient _restClient;
        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private readonly EosCodeMappings _eosCodeMappings;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly TaskManager _taskManager;
        private readonly UserCardService _userCardService;
        private readonly BiometricTemplateManager _biometricTemplateManager;

        public const int StPro = 2001;
        public const int StProPlus = 2002;
        public const int StShineL = 2003;
        public const int StShineM = 2004;
        public const int StFace110 = 2005;
        public const int StFace120 = 2006;
        public const int StFace130 = 2007;
        public const int StFace160 = 2008;
        public const int StFace710 = 2009;
        public const int StEco210 = 2010;
        public const int StP220 = 2011;

        public DeviceFactory(EosLogService eosLogService, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings, FaceTemplateTypes faceTemplateTypes, UserCardService userCardService, BiometricTemplateManager biometricTemplateManager, FingerTemplateTypes fingerTemplateTypes, RestClient restClient, TaskManager taskManager)
        {
            _logEvents = logEvents;
            _restClient = restClient;
            _taskManager = taskManager;
            _logSubEvents = logSubEvents;
            _eosLogService = eosLogService;
            _eosCodeMappings = eosCodeMappings;
            _fingerTemplateTypes = fingerTemplateTypes;
            _userCardService = userCardService;
            _faceTemplateTypes = faceTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;
        }

        /// <summary>
        /// <En>Creates a device instance by device type.</En>
        /// <Fa>با توجه به نوع دستگاه درحال پردازش، یک نمونه از آن ایجاد می کند.</Fa>
        /// </summary>
        /// <param name="device">اطلاعات کامل دستگاه</param>
        /// <returns>Device object</returns>
        public Device Factory(DeviceBasicInfo device)
        {
            switch (device.ModelId)
            {
                case StPro:
                case StProPlus:
                    {
                        return new SupremaBaseDevice(device, _eosLogService, _logEvents, _logSubEvents, _eosCodeMappings, _biometricTemplateManager, _fingerTemplateTypes, _taskManager, _restClient);
                    }

                case StFace110:
                case StFace710:
                    {
                        return new HanvonBase(device, _eosLogService, _logEvents, _logSubEvents, _eosCodeMappings, _faceTemplateTypes, _userCardService, _taskManager, _restClient);
                    }

                case StShineL:
                    {
                        return null;
                    }

                case StFace120:
                case StFace130:
                case StFace160:
                case StEco210:
                case StP220:
                case StShineM:
                    {
                        return null;
                    }

                default:
                    return new Device(device, _eosLogService, _logEvents, _logSubEvents, _eosCodeMappings);
            }
        }
    }
}
