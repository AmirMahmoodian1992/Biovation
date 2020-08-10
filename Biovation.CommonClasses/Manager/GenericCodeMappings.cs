using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.CommonClasses.Manager
{
    public class GenericCodeMappings
    {
        private readonly GenericCodeMappingService _genericCodeMappingService;

        public GenericCodeMappings(GenericCodeMappingService genericCodeMappingService)
        {
            _genericCodeMappingService = genericCodeMappingService;
        }

        public void FillValues()
        {
            Task.Run(() =>
            {
                var logEventMappingsQuery = _genericCodeMappingService.GetGenericCodeMappings(1);
                var logSubEventMappingsQuery = _genericCodeMappingService.GetGenericCodeMappings(2);
                var fingerTemplateTypeMappingsQuery = _genericCodeMappingService.GetGenericCodeMappings(9);
                var matchingTypeMappingsQuery = _genericCodeMappingService.GetGenericCodeMappings(15);

                LogEventMappings = logEventMappingsQuery.Result;
                LogSubEventMappings = logSubEventMappingsQuery.Result;
                FingerTemplateTypeMappings = fingerTemplateTypeMappingsQuery.Result;
                MatchingTypeMappings = matchingTypeMappingsQuery.Result;
            });
        }

        public List<GenericCodeMapping> LogSubEventMappings { get; set; }
        public List<GenericCodeMapping> LogEventMappings { get; set; }
        public List<GenericCodeMapping> FingerTemplateTypeMappings { get; set; }
        public List<GenericCodeMapping> MatchingTypeMappings { get; set; }
    }
}