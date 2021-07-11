using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class IrisTemplateTypes
    {
        public const string VIrisCode = "20001";
        

        public IrisTemplateTypes(Lookups lookups)
        {
            VIris = lookups.IrisTemplateType.FirstOrDefault(lookup => string.Equals(lookup.Code, VIrisCode));
        }

        public Lookup VIris { get; set; }
    }
}
