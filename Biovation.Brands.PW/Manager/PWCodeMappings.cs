using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.Constants;
using Biovation.Domain;

namespace Biovation.Brands.PW.Manager
{
    public class PwCodeMappings
    {
        public PwCodeMappings(GenericCodeMappings genericCodeMappings)
        {
            _pwMatchingTypeMappings = genericCodeMappings.MatchingTypeMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.ProcessingWorldCode).ToList();
        }

        private readonly List<GenericCodeMapping> _pwMatchingTypeMappings;

        public Lookup GetMatchingTypeGenericLookup(int pwCode)
        {
            return _pwMatchingTypeMappings.FirstOrDefault(matchingType => string.Equals(matchingType.ManufactureCode, pwCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }
    }
}