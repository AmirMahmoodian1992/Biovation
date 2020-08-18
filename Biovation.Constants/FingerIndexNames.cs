using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class FingerIndexNames
    {
        public const string RightThumbCode = "14001";
        public const string RightIndexCode = "14002";
        public const string RightMiddleCode = "14003";
        public const string RightRingCode = "14004";
        public const string RightLittleCode = "14005";
        public const string LeftLittleCode = "14006";
        public const string LeftRingCode = "14007";
        public const string LeftMiddleCode = "14008";
        public const string LeftIndexCode = "14009";
        public const string LeftThumbCode = "140010";
        public const string UnknownCode = "140011";

        public static Lookup RightThumb = Lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, RightThumbCode));
        public static Lookup RightIndex = Lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, RightIndexCode));
        public static Lookup RightMiddle = Lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, RightMiddleCode));
        public static Lookup RightRing = Lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, RightRingCode));
        public static Lookup RightLittle = Lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, RightLittleCode));
        public static Lookup LeftLittle = Lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, LeftLittleCode));
        public static Lookup LeftRing = Lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, LeftRingCode));
        public static Lookup LeftMiddle = Lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, LeftMiddleCode));
        public static Lookup LeftIndex = Lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, LeftIndexCode));
        public static Lookup LeftThumb = Lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, LeftThumbCode));
        public static Lookup Unknown = Lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, UnknownCode));
    }
}