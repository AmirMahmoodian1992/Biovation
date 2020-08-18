using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class MatchingTypes
    {
        public const string FaceCode = "19001";
        public const string FingerCode = "19002";
        public const string CarCode = "19003";
        public const string CardCode = "19004";
        public const string UnknownCode = "19000";
        public const string UnIdentifyCode = "0";

        public static Lookup Face = Lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, FaceCode));
        public static Lookup Finger = Lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, FingerCode));
        public static Lookup Car = Lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, CarCode));
        public static Lookup Card = Lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, CardCode));
        public static Lookup Unknown = Lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UnknownCode));
        public static Lookup UnIdentify = Lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UnIdentifyCode));

    }
}