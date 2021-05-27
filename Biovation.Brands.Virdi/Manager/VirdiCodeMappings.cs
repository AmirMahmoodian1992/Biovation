using Biovation.Domain;
using Biovation.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biovation.Brands.Virdi.Manager
{
    public class VirdiCodeMappings
    {
        private readonly MatchingTypes _matchingTypes;

        public VirdiCodeMappings(GenericCodeMappings genericCodeMappings, MatchingTypes matchingTypes)
        {
            _matchingTypes = matchingTypes;
            _virdiLogSubEventMappings = genericCodeMappings.LogSubEventMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.VirdiCode).ToList();

            _virdiFingerTemplateTypeMappings = genericCodeMappings.FingerTemplateTypeMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.VirdiCode).ToList();

            _virdiFaceTemplateTypeMappings = genericCodeMappings.FaceTemplateTypeMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.VirdiCode).ToList();

            _virdiMatchingTypeMappings = genericCodeMappings.MatchingTypeMappings.Where(
                genericCode => genericCode.Brand.Code == DeviceBrands.VirdiCode).ToList();
        }

        private readonly List<GenericCodeMapping> _virdiLogSubEventMappings;

        private readonly List<GenericCodeMapping> _virdiFingerTemplateTypeMappings;
        private readonly List<GenericCodeMapping> _virdiFaceTemplateTypeMappings;

        private readonly List<GenericCodeMapping> _virdiMatchingTypeMappings;

        public Lookup GetLogSubEventGenericLookup(int virdiCode)
        {
            return _virdiLogSubEventMappings.FirstOrDefault(subEvent => string.Equals(subEvent.ManufactureCode, virdiCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue ?? new Lookup { Code = virdiCode.ToString(), Category = _virdiLogSubEventMappings.FirstOrDefault()?.GenericValue.Category };
        }

        public Lookup GetFingerTemplateTypeLookup(int virdiCode)
        {
            return _virdiFingerTemplateTypeMappings.FirstOrDefault(fingerTemplateType => string.Equals(fingerTemplateType.ManufactureCode, virdiCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }
        public Lookup GetMatchingTypeGenericLookup(int virdiCode)
        {
            return _virdiMatchingTypeMappings.FirstOrDefault(matchingType => string.Equals(matchingType.ManufactureCode, virdiCode.ToString(), StringComparison.InvariantCultureIgnoreCase))?.GenericValue ?? _matchingTypes.Unknown;
        }
        public Lookup GetFaceTemplateTypeLookup(string faceCode)
        {
            return _virdiFaceTemplateTypeMappings.FirstOrDefault(faceTemplateType => string.Equals(faceTemplateType.ManufactureCode, faceCode, StringComparison.InvariantCultureIgnoreCase))?.GenericValue;
        }

        public string GetFaceTemplateManufactureCode(string faceCode)
        {
            return _virdiFaceTemplateTypeMappings.FirstOrDefault(faceTemplateType => string.Equals(faceTemplateType.GenericValue.Code, faceCode, StringComparison.InvariantCultureIgnoreCase))?.ManufactureCode;
        }
    }
}
