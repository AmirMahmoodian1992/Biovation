using Biovation.Domain;
using System.Linq;

namespace Biovation.Constants
{
    public class MatchingTypes
    {
        public const string FaceCode = "19001";
        public const string FingerCode = "19002";
        public const string CarCode = "19003";
        public const string CardCode = "19004";
        public const string UnknownCode = "19000";
        public const string UnIdentifyCode = "19099";
        public const string IrisCode = "19005";

        public MatchingTypes(Lookups lookups)
        {
            Face = lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, FaceCode));
            Finger = lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, FingerCode));
            Car = lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, CarCode));
            Card = lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, CardCode));
            Unknown = lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UnknownCode));
            UnIdentify = lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UnIdentifyCode));
            Iris = lookups.MatchingTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, IrisCode));
        }

        public Lookup Face;
        public Lookup Finger;
        public Lookup Car;
        public Lookup Card;
        public Lookup Unknown;
        public Lookup UnIdentify;
        public Lookup Iris;
    }
}