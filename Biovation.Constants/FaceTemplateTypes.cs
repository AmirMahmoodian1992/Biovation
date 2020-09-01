using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class FaceTemplateTypes
    {
        public const string VFACECode = "18101";
        public const string ZKVX7Code = "18102";
        public const string SFACECode = "18103";

        public FaceTemplateTypes(Lookups lookups)
        {
            VFACE = lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, VFACECode));
            ZKVX7 = lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, ZKVX7Code));
            SFACE = lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, SFACECode));
        }

        public static Lookup VFACE { get; set; }
        public static Lookup ZKVX7 { get; set; }
        public static Lookup SFACE { get; set; }
    }
}
