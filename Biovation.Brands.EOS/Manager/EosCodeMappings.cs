using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Constants;
using Biovation.Domain;

namespace Biovation.Brands.EOS.Manager
{
    public class EosCodeMappings
    {
        private readonly List<GenericCodeMapping> _eosLogSubEventMappings;

        private readonly List<GenericCodeMapping> _eosMatchingTypeMappings;
        public EosCodeMappings(GenericCodeMappings genericCodeMappings)
        {
            _eosLogSubEventMappings = genericCodeMappings.LogSubEventMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.EosCode).ToList();

            _eosMatchingTypeMappings = genericCodeMappings.MatchingTypeMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.EosCode).ToList();
        }

   


        public Lookup GetLogSubEventGenericLookup(string eosCode)
        {
            return _eosLogSubEventMappings.FirstOrDefault(subEvent => string.Equals(subEvent.ManufactureCode, eosCode, StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }
        public Lookup GetMatchingTypeGenericLookup(int eosCode)
        {
            return _eosMatchingTypeMappings.FirstOrDefault(matchingType => string.Equals(matchingType.ManufactureCode, eosCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }
    }
}