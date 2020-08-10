using Biovation.CommonClasses.Manager;
using System.Linq;

namespace Biovation.CommonClasses.Models.ConstantValues
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

        public DeviceBrands()
        {
            Virdi = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, VirdiCode));
            Eos = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, EosCode));
            Suprema = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, SupremaCode));
            ZkTeco = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, ZkTecoCode));
            ProcessingWorld = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, ProcessingWorldCode));
            Maxa = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, MaxaCode));
            Shahab = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, ShahabCode));
        }

        public static Lookup Virdi { get; set; }
        public static Lookup Eos { get; set; }
        public static Lookup Suprema { get; set; }
        public static Lookup ZkTeco { get; set; }
        public static Lookup ProcessingWorld { get; set; }
        public static Lookup Maxa { get; set; }
        public static Lookup Shahab { get; set; }



        //public static DeviceBrand Virdi = Brands.FirstOrDefault
        /* public static DeviceBrand Eos = Brands.FirstOrDefault(brand => string.Equals(brand.Id, EosCode));
         public static DeviceBrand Suprema = Brands.FirstOrDefault(brand => string.Equals(brand.Id, SupremaCode));
         public static DeviceBrand ZkTeco = Brands.FirstOrDefault(brand => string.Equals(brand.Id, ZkTecoCode));
         public static DeviceBrand ProcessingWorld = Brands.FirstOrDefault(brand => string.Equals(brand.Id, ProcessingWorldCode));
         public static DeviceBrand Maxa = Brands.FirstOrDefault(brand => string.Equals(brand.Id, MaxaCode));
         */
    }
}