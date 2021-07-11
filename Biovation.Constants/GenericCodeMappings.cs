using System.Collections.Generic;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class GenericCodeMappings
    {
        
        public List<GenericCodeMapping> LogSubEventMappings { get; set; }
        public List<GenericCodeMapping> LogEventMappings { get; set; }
        public List<GenericCodeMapping> FingerTemplateTypeMappings { get; set; }
        public List<GenericCodeMapping> FaceTemplateTypeMappings { get; set; }
        public List<GenericCodeMapping> MatchingTypeMappings { get; set; }
        public List<GenericCodeMapping> FingerIndexMappings { get; set; }
    }
}