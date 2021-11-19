using Biovation.Domain;
using System.Linq;

namespace Biovation.Constants
{
    public class relayTypes
    {
        public const string DefaultCode = "23000";
        public const string HumanGateCode = "23001";
        public const string CarGateCode = "23002";
        public const string MultiUseGateCode = "23003";
        public const string FixedLightCode = "23004";
        public const string FlashedLightCode = "23005";

        public relayTypes(Lookups lookups)
        {
            Default = lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, DefaultCode));
            HumanGate = lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, HumanGateCode));
            CarGate = lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, CarGateCode));
            MultiUseGate = lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, MultiUseGateCode));
            FixedLight = lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, FixedLightCode));
            FlashedLight = lookups.TaskPriorities.FirstOrDefault(lookup => string.Equals(lookup.Code, FlashedLightCode));
        }

        public Lookup Default;
        public Lookup HumanGate;
        public Lookup CarGate;
        public Lookup MultiUseGate;
        public Lookup FixedLight;
        public Lookup FlashedLight;
    }
}