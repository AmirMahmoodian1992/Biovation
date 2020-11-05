using Biovation.Brands.EOS.Devices.SupremaBase;
using Biovation.Brands.EOS.Manager;
using Biovation.Brands.EOS.Service;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.EOS.Devices
{
    /// <summary>
    /// ساخت و بازگردانی یک نمونه ساعت، با توجه به نوع مورد نیاز
    /// </summary>
    public class DeviceFactory
    {
       // private readonly EosServer _eosServer;
        private readonly EosLogService _eosLogService;

        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private readonly EosCodeMappings _eosCodeMappings;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly UserCardService _userCardService;


        public const int SupremaBase = 2002;
        public const int HonvanBase = 2001;

        public DeviceFactory(/*EosServer eosServer,*/ EosLogService eosLogService, LogEvents logEvents, LogSubEvents logSubEvents, EosCodeMappings eosCodeMappings, FaceTemplateTypes faceTemplateTypes, FaceTemplateService faceTemplateService, UserCardService userCardService)
        {
           // _eosServer = eosServer;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _eosLogService = eosLogService;
            _eosCodeMappings = eosCodeMappings;
            _faceTemplateTypes = faceTemplateTypes;
            _faceTemplateService = faceTemplateService;
            _userCardService = userCardService;
        }

        /// <summary>
        /// <En>Creates a device instance by device type.</En>
        /// <Fa>با توجه به نوع دستگاه درحال پردازش، یک نمونه از آن ایجاد می کند.</Fa>
        /// </summary>
        /// <param name="device">اطلاعات کامل دستگاه</param>
        /// <param name="connectionType"></param>
        /// <returns>Device object</returns>
        public Device Factory(DeviceBasicInfo device, string connectionType)
        {

            switch (device.ModelId)
            {
                //case BSSDK.BS_DEVICE_BIOMINI_CLIENT:
                //    {
                //        return new BiominiClient(device, clientConnection);
                //    }
                case SupremaBase:
                    {
                        return new SupremaBaseDevice(device, _eosLogService, _logEvents, _logSubEvents, _eosCodeMappings);
                    }
                case HonvanBase:
                {
                    return new HonvanBase(device, _eosLogService, _logEvents, _logSubEvents, _eosCodeMappings, _faceTemplateTypes, _faceTemplateService, _userCardService);
                }



                default:
                    return new Device(device, _eosLogService, _logEvents, _logSubEvents, _eosCodeMappings);

            }
        }
    }
}
