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

        public FingerIndexNames(Lookups lookups)
        {
            RightThumb = lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, RightThumbCode));
            RightIndex = lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, RightIndexCode));
            RightMiddle = lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, RightMiddleCode));
            RightRing = lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, RightRingCode));
            RightLittle = lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, RightLittleCode));
            LeftLittle = lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, LeftLittleCode));
            LeftRing = lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, LeftRingCode));
            LeftMiddle = lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, LeftMiddleCode));
            LeftIndex = lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, LeftIndexCode));
            LeftThumb = lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, LeftThumbCode));
            Unknown = lookups.FingerIndexNames.FirstOrDefault(lookup => string.Equals(lookup.Code, UnknownCode));
        }

        public static Lookup RightThumb;
        public static Lookup RightIndex;
        public static Lookup RightMiddle;
        public static Lookup RightRing;
        public static Lookup RightLittle;
        public static Lookup LeftLittle;
        public static Lookup LeftRing;
        public static Lookup LeftMiddle;
        public static Lookup LeftIndex;
        public static Lookup LeftThumb;
        public static Lookup Unknown;
    }
}