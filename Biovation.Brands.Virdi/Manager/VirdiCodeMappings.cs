using Biovation.Domain;
using Biovation.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.Virdi.Manager
{
    public class VirdiCodeMappings
    {
        public VirdiCodeMappings(GenericCodeMappings genericCodeMappings)
        {
            _virdiLogSubEventMappings = genericCodeMappings.LogSubEventMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.VirdiCode).ToList();

            _virdiFingerTemplateTypeMappings = genericCodeMappings.FingerTemplateTypeMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.VirdiCode).ToList();

            _virdiMatchingTypeMappings = genericCodeMappings.MatchingTypeMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.VirdiCode).ToList();
        }


        private static List<GenericCodeMapping> _virdiLogSubEventMappings;

        private static List<GenericCodeMapping> _virdiFingerTemplateTypeMappings;

        private static List<GenericCodeMapping> _virdiMatchingTypeMappings;

        public static Lookup GetLogSubEventGenericLookup(int virdiCode)
        {
            return _virdiLogSubEventMappings.FirstOrDefault(subEvent => string.Equals(subEvent.ManufactureCode, virdiCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }

        public static Lookup GetFingerTemplateTypeLookup(int virdiCode)
        {
            return _virdiFingerTemplateTypeMappings.FirstOrDefault(fingerTemplateType => string.Equals(fingerTemplateType.ManufactureCode, virdiCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }
        public static Lookup GetMatchingTypeGenericLookup(int virdiCode)
        {
            return _virdiMatchingTypeMappings.FirstOrDefault(matchingType => string.Equals(matchingType.ManufactureCode, virdiCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }
    }
}
