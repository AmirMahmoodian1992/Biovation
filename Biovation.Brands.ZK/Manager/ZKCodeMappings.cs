using Biovation.Constants;
using Biovation.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.ZK.Manager
{
    public class ZkCodeMappings
    {
        private readonly MatchingTypes _matchingTypes;
        public ZkCodeMappings(GenericCodeMappings genericCodeMappings, MatchingTypes matchingTypes)
        {
            _matchingTypes = matchingTypes;

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
            return _zkLogSubEventMappings.FirstOrDefault(subEvent => string.Equals(subEvent.ManufactureCode, zkCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue ?? new Lookup { Code = zkCode.ToString(), Category = _zkLogSubEventMappings.FirstOrDefault()?.GenericValue.Category };
        }

        public Lookup GetFingerTemplateTypeLookup(int zkCode)
        {
            return _zkFingerTemplateTypeMappings.FirstOrDefault(fingerTemplateType => string.Equals(fingerTemplateType.ManufactureCode, zkCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }

        public Lookup GetMatchingTypeGenericLookup(int zkCode)
        {
            return _zkMatchingTypeMappings.FirstOrDefault(matchingType => string.Equals(matchingType.ManufactureCode, zkCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue ?? _matchingTypes.Unknown;
        }
    }
}

