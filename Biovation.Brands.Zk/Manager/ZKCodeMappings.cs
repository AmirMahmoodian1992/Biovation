using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Constants;
using Biovation.Domain;

namespace Biovation.Brands.ZK.Manager
{
    public class ZkCodeMappings
    {
        public ZkCodeMappings(GenericCodeMappings genericCodeMappings)
        {

            _zkLogSubEventMappings = genericCodeMappings.LogSubEventMappings.Where(
                 genericCode => genericCode.Brand.Code == DeviceBrands.ZkTecoCode).ToList();

             _zkFingerTemplateTypeMappings = genericCodeMappings.FingerTemplateTypeMappings.Where(
                 genericCode => genericCode.Brand.Code == DeviceBrands.ZkTecoCode).ToList();

             _zkMatchingTypeMappings = genericCodeMappings.MatchingTypeMappings.Where(
                 genericCode => genericCode.Brand.Code == DeviceBrands.ZkTecoCode).ToList();
        }

        private readonly List<GenericCodeMapping> _zkLogSubEventMappings;

        private readonly List<GenericCodeMapping> _zkFingerTemplateTypeMappings;

        private readonly List<GenericCodeMapping> _zkMatchingTypeMappings;



        public Lookup GetLogSubEventGenericLookup(int zkCode)
        {
            return _zkLogSubEventMappings.FirstOrDefault(subEvent => string.Equals(subEvent.ManufactureCode, zkCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }

        public Lookup GetFingerTemplateTypeLookup(int zkCode)
        {
            return _zkFingerTemplateTypeMappings.FirstOrDefault(fingerTemplateType => string.Equals(fingerTemplateType.ManufactureCode, zkCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }
        public Lookup GetMatchingTypeGenericLookup(int zkCode)
        {
            return _zkMatchingTypeMappings.FirstOrDefault(matchingType => string.Equals(matchingType.ManufactureCode, zkCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }
    }
}

