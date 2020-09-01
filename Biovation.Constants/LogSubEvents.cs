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

        public LogSubEvents(Lookups lookups)
        {
            Normal = lookups.LogSubEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, NormalCode));
            FirstFunction = lookups.LogSubEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, FirstFunctionCode));
            SecondFunction = lookups.LogSubEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, SecondFunctionCode));
            ThirdFunction = lookups.LogSubEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, ThirdFunctionCode));
            FourthFunction = lookups.LogSubEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, FourthFunctionCode));
        }

        public static Lookup Normal;
        public static Lookup FirstFunction;
        public static Lookup SecondFunction;
        public static Lookup ThirdFunction;
        public static Lookup FourthFunction;
    }
}
