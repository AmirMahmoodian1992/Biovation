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
        private readonly Lookups _lookups;
        private readonly DeviceService _deviceService;

        public SupremaCodeMappings(DeviceService deviceService, GenericCodeMappings genericCodeMapping, Lookups lookups)
        {
            _deviceService = deviceService;
            _lookups = lookups;

            _supremaLogSubEventMappings = genericCodeMapping.LogSubEventMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.SupremaCode).ToList();

            _supremaLogEventMappings = genericCodeMapping.LogEventMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.SupremaCode).ToList();

            _supremaMatchingTypeMappings = genericCodeMapping.MatchingTypeMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.SupremaCode).ToList();
        }

        private readonly List<GenericCodeMapping> _supremaLogSubEventMappings;
        private readonly List<GenericCodeMapping> _supremaLogEventMappings;
        private readonly List<GenericCodeMapping> _supremaMatchingTypeMappings;

        public Lookup GetLogSubEventGenericLookup(byte supremaCode)
        {
            return _supremaLogSubEventMappings.FirstOrDefault(subEvent => string.Equals(subEvent.ManufactureCode, ((int)supremaCode).ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue ?? new Lookup { Code = Convert.ToInt32(supremaCode).ToString(), Category = _supremaLogSubEventMappings.FirstOrDefault()?.GenericValue.Category };
        }

        public Lookup GetLogEventGenericLookup(byte supremaCode)
        {
            return _supremaLogEventMappings.FirstOrDefault(logEvent => string.Equals(logEvent.ManufactureCode, ((int)supremaCode).ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }

        public DeviceModel GetGenericDeviceModel(int supremaDeviceTypeId)
        {
            var deviceModels = _deviceService.GetDeviceModels(brandId: DeviceBrands.SupremaCode);
            return deviceModels.FirstOrDefault(deviceModel => deviceModel.Brand.Code == DeviceBrands.SupremaCode && deviceModel.ManufactureCode == supremaDeviceTypeId);
        }

        public Lookup GetMatchingTypeGenericLookup(int supremaCode)
        {
            return _supremaMatchingTypeMappings.FirstOrDefault(matchingType => string.Equals(matchingType.ManufactureCode, supremaCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue ?? _lookups.MatchingTypes.FirstOrDefault(matchingType => matchingType.Code == MatchingTypes.UnknownCode);
        }
    }
}