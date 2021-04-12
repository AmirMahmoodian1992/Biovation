﻿using Biovation.Constants;
using Biovation.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.Paliz.Manager
{
    public class PalizCodeMappings
    {
        private readonly MatchingTypes _matchingTypes;
        public PalizCodeMappings(GenericCodeMappings genericCodeMappings, MatchingTypes matchingTypes)
        {
            _matchingTypes = matchingTypes;

            _logSubEventMappings = genericCodeMappings.LogSubEventMappings.Where(
                 genericCode => genericCode.Brand.Code == DeviceBrands.PalizCode).ToList();

            _fingerTemplateTypeMappings = genericCodeMappings.FingerTemplateTypeMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.PalizCode).ToList();

            _matchingTypeMappings = genericCodeMappings.MatchingTypeMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.PalizCode).ToList();
        }

        private readonly List<GenericCodeMapping> _logSubEventMappings;

        private readonly List<GenericCodeMapping> _fingerTemplateTypeMappings;

        private readonly List<GenericCodeMapping> _matchingTypeMappings;


        public Lookup GetLogSubEventGenericLookup(int code)
        {
            return _logSubEventMappings.FirstOrDefault(subEvent => string.Equals(subEvent.ManufactureCode, code.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue ?? new Lookup { Code = code.ToString(), Category = _logSubEventMappings.FirstOrDefault()?.GenericValue.Category };
        }

        public Lookup GetFingerTemplateTypeLookup(int code)
        {
            return _fingerTemplateTypeMappings.FirstOrDefault(fingerTemplateType => string.Equals(fingerTemplateType.ManufactureCode, code.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }

        public Lookup GetMatchingTypeGenericLookup(int code)
        {
            return _matchingTypeMappings.FirstOrDefault(matchingType => string.Equals(matchingType.ManufactureCode, code.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue ?? _matchingTypes.Unknown;
        }
    }
}
