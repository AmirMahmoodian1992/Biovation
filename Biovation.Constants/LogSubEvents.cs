using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class LogSubEvents
    {
        public const string NormalCode = "17001";
        public const string FirstFunctionCode = "17002";
        public const string SecondFunctionCode = "17003";
        public const string ThirdFunctionCode = "17004";
        public const string FourthFunctionCode = "17005";


        public static Lookup Normal = Lookups.LogSubEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, NormalCode));
        public static Lookup FirstFunction = Lookups.LogSubEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, FirstFunctionCode));
        public static Lookup SecondFunction = Lookups.LogSubEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, SecondFunctionCode));
        public static Lookup ThirdFunction = Lookups.LogSubEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, ThirdFunctionCode));
        public static Lookup FourthFunction = Lookups.LogSubEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, FourthFunctionCode));
    }
}
