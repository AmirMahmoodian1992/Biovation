using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.Suprema.Manager
{
    public class SupremaCodeMappings
    {
        private readonly DeviceService _commonDeviceService;

        public SupremaCodeMappings(DeviceService commonDeviceService, GenericCodeMappings genericCodeMapping)
        {
            _commonDeviceService = commonDeviceService;
            _supremaLogSubEventMappings = genericCodeMapping.LogSubEventMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.SupremaCode).ToList();

            _supremaLogEventMappings = genericCodeMapping.LogEventMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.SupremaCode).ToList();
        }

        private readonly List<GenericCodeMapping> _supremaLogSubEventMappings;
        private readonly List<GenericCodeMapping> _supremaLogEventMappings;

        public Lookup GetLogSubEventGenericLookup(byte supremaCode)
        {
            return _supremaLogSubEventMappings.FirstOrDefault(subEvent => string.Equals(subEvent.ManufactureCode, ((int)supremaCode).ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }

        public Lookup GetLogEventGenericLookup(byte supremaCode)
        {
            return _supremaLogEventMappings.FirstOrDefault(logEvent => string.Equals(logEvent.ManufactureCode, ((int)supremaCode).ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }

        public DeviceModel GetGenericDeviceModel(int supremaDeviceTypeId)
        {
            var deviceModels = _commonDeviceService.GetDeviceModels(brandId: Convert.ToInt32(DeviceBrands.SupremaCode));
            return deviceModels.FirstOrDefault(deviceModel => deviceModel.Brand.Code == DeviceBrands.SupremaCode && deviceModel.ManufactureCode == supremaDeviceTypeId);
        }
    }
}