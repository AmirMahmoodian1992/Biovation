using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class DeviceBrands
    {
        public const string VirdiCode = "15001";
        public const string EosCode = "15002";
        public const string SupremaCode = "15003";
        public const string ZkTecoCode = "15004";
        public const string ProcessingWorldCode = "15005";
        public const string MaxaCode = "15006";
        public const string ShahabCode = "15007";

        public const string PalizCode = "15009";

        public DeviceBrands(Lookups lookups)
        {
            Virdi = lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, VirdiCode));
            Eos = lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, EosCode));
            Suprema = lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, SupremaCode));
            ZkTeco = lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, ZkTecoCode));
            ProcessingWorld = lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, ProcessingWorldCode));
            Maxa = lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, MaxaCode));
            Shahab = lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, ShahabCode));
            Paliz = lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, PalizCode));
        }

        public Lookup Virdi { get; set; }
        public Lookup Eos { get; set; }
        public Lookup Suprema { get; set; }
        public Lookup ZkTeco { get; set; }
        public Lookup ProcessingWorld { get; set; }
        public Lookup Maxa { get; set; }
        public Lookup Shahab { get; set; }
        public Lookup Paliz { get; set; }
    }
}