using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.CommonClasses.Manager
{
    public static class GenericCodeMappings
    {
        private static readonly GenericCodeMappingService GenericCodeMappingService = new GenericCodeMappingService();

        public static void FillValues()
        {
            Task.Run(() =>
            {
                var logEventMappingsQuery = GenericCodeMappingService.GetGenericCodeMappings(1);
                var logSubEventMappingsQuery = GenericCodeMappingService.GetGenericCodeMappings(2);
                var fingerTemplateTypeMappingsQuery = GenericCodeMappingService.GetGenericCodeMappings(9);
                var matchingTypeMappingsQuery = GenericCodeMappingService.GetGenericCodeMappings(15);

                LogEventMappings = logEventMappingsQuery.Result;
                LogSubEventMappings = logSubEventMappingsQuery.Result;
                FingerTemplateTypeMappings = fingerTemplateTypeMappingsQuery.Result;
                MatchingTypeMappings = matchingTypeMappingsQuery.Result;
            });
        }

        public static List<GenericCodeMapping> LogSubEventMappings { get; set; }
        public static List<GenericCodeMapping> LogEventMappings { get; set; }
        public static List<GenericCodeMapping> FingerTemplateTypeMappings { get; set; }
        public static List<GenericCodeMapping> MatchingTypeMappings { get; set; }
    }
}