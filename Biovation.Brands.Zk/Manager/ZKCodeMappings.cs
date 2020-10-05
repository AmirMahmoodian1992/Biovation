using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;

namespace Biovation.Brands.ZK.Manager
{
    public static class ZKCodeMappings
    {
      
        private static readonly List<GenericCodeMapping> ZkMatchingTypeMappings = GenericCodeMappings.MatchingTypeMappings.Where(
            genericCode => genericCode.Brand.Code == CommonClasses.Models.ConstantValues.DeviceBrands.ZkTecoCode).ToList();

        
        public static Lookup GetMatchingTypeGenericLookup(int zkCode)
        {
            return ZkMatchingTypeMappings.FirstOrDefault(matchingType => string.Equals(matchingType.ManufactureCode, zkCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }
    }
}

