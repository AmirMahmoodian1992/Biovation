using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class FaceTemplateTypes
    {
        public const string VFACECode = "18101";
        public const string ZKVX7Code = "18102";
        public const string SFACECode = "18103";
        public const string EOSHanvonCode = "18104";

        public FaceTemplateTypes(Lookups lookups)
        {
            VFACE = lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, VFACECode));
            ZKVX7 = lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, ZKVX7Code));
            SFACE = lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, SFACECode));
            EOSHanvon = lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, EOSHanvonCode));
        }

        public Lookup VFACE { get; set; }
        public Lookup ZKVX7 { get; set; }
        public Lookup SFACE { get; set; }
        public Lookup EOSHanvon { get; set; }
    }
}
