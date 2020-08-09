using System.Linq;
using Biovation.CommonClasses.Manager;

namespace Biovation.CommonClasses.Models.ConstantValues
{
    public class FaceTemplateTypes
    {
        public const string VFACECode = "18101";
        public const string ZKVX7Code = "18102";
        public const string SFACECode = "18103";

        public static Lookup VFACE = Lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, VFACECode));
        public static Lookup ZKVX7 = Lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, ZKVX7Code));
        public static Lookup SFACE = Lookups.FaceTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, SFACECode));
    }
}
