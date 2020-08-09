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


        public static Lookup Virdi = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, VirdiCode));
        public static Lookup Eos = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, EosCode));
        public static Lookup Suprema = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, SupremaCode));
        public static Lookup ZkTeco = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, ZkTecoCode));
        public static Lookup ProcessingWorld = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, ProcessingWorldCode));
        public static Lookup Maxa = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, MaxaCode));
        public static Lookup Shahab = Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Code, ShahabCode));


        //public static DeviceBrand Virdi = Brands.FirstOrDefault
        /* public static DeviceBrand Eos = Brands.FirstOrDefault(brand => string.Equals(brand.Id, EosCode));
         public static DeviceBrand Suprema = Brands.FirstOrDefault(brand => string.Equals(brand.Id, SupremaCode));
         public static DeviceBrand ZkTeco = Brands.FirstOrDefault(brand => string.Equals(brand.Id, ZkTecoCode));
         public static DeviceBrand ProcessingWorld = Brands.FirstOrDefault(brand => string.Equals(brand.Id, ProcessingWorldCode));
         public static DeviceBrand Maxa = Brands.FirstOrDefault(brand => string.Equals(brand.Id, MaxaCode));
         */
    }
}