using System.Linq;
using Biovation.CommonClasses.Manager;

namespace Biovation.CommonClasses.Models.ConstantValues
{
    public class FingerTemplateTypes
    {
        public const string V400Code ="18001";
        public const string SU384Code ="18003";
        public const string VX10Code = "18002";
 
        public static Lookup V400 = Lookups.FingerTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, V400Code));
        public static Lookup SU384 = Lookups.FingerTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, SU384Code));
        public static Lookup VX10 = Lookups.FingerTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, VX10Code));

    }
}
