using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class FingerTemplateTypes
    {
        public const string V400Code = "18001";
        public const string SU384Code = "18003";
        public const string VX10Code = "18002";
        public const string EOSSupremaCode = "18004";

        public FingerTemplateTypes(Lookups lookups)
        {
            V400 = lookups.FingerTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, V400Code));
            SU384 = lookups.FingerTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, SU384Code));
            VX10 = lookups.FingerTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, VX10Code));
            EOSSuprema = lookups.FingerTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, EOSSupremaCode));
        }

        public Lookup V400;
        public Lookup SU384;
        public Lookup VX10;
        public Lookup EOSSuprema;

    }
}
