using Biovation.CommonClasses.Manager;
using System.Linq;

namespace Biovation.CommonClasses.Models.ConstantValues
{
    public class FaceTemplateTypes
    {
        public const string VFACECode = "18101";
        public const string ZKVX7Code = "18102";
        public const string SFACECode = "18103";

        public FaceTemplateTypes()
        {
            VFACE = Lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, VFACECode));
            ZKVX7 = Lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, ZKVX7Code));
            SFACE = Lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, SFACECode));
        }

        public static Lookup VFACE { get; set; }
        public static Lookup ZKVX7 { get; set; }
        public static Lookup SFACE { get; set; }
    }
}
