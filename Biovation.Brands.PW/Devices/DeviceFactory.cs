using Biovation.Brands.PW.Manager;
using Biovation.Brands.PW.Service;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;

namespace Biovation.Brands.PW.Devices
{
    public class DeviceFactory
    {
        private readonly PwLogService _pwLogService;

        private readonly LogEvents _logEvents;
        private readonly LogSubEvents _logSubEvents;
        private readonly PwCodeMappings _pwCodeMappings;
        
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public DeviceFactory(LogEvents logEvents, LogSubEvents logSubEvents, PwCodeMappings pwCodeMappings, BiovationConfigurationManager biovationConfigurationManager, PwLogService pwLogService)
        {
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
            _pwLogService = pwLogService;
            _pwCodeMappings = pwCodeMappings;
            _biovationConfigurationManager = biovationConfigurationManager;
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
                default:
                    return new Device(device, _biovationConfigurationManager, _logEvents, _logSubEvents, _pwCodeMappings, _pwLogService);
            }
        }
    }
}
