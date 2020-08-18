using System.Collections.Generic;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class GenericCodeMappings
    {
        //Todo fill values in startup
        //public GenericCodeMappings(GenericCodeMappingService genericCodeMappingService)
        //{
        //    var logEventMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(1);
        //    var logSubEventMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(2);
        //    var fingerTemplateTypeMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(9);
        //    var matchingTypeMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(15);

        //    LogEventMappings = logEventMappingsQuery.Result;
        //    LogSubEventMappings = logSubEventMappingsQuery.Result;
        //    FingerTemplateTypeMappings = fingerTemplateTypeMappingsQuery.Result;
        //    MatchingTypeMappings = matchingTypeMappingsQuery.Result;
        //}

        public List<GenericCodeMapping> LogSubEventMappings { get; set; }
        public List<GenericCodeMapping> LogEventMappings { get; set; }
        public List<GenericCodeMapping> FingerTemplateTypeMappings { get; set; }
        public List<GenericCodeMapping> MatchingTypeMappings { get; set; }
    }
}