using Biovation.Constants;
using Biovation.Domain;

namespace Biovation.Brands.Suprema.Manager
{
    public class BiometricTemplateManager
    {
        private readonly FingerIndexNames _fingerIndexNames;

        public BiometricTemplateManager(FingerIndexNames fingerIndexNames)
        {
            _fingerIndexNames = fingerIndexNames;
        }

        public Lookup GetFingerIndex(int fingerIndex)
        {
            switch (fingerIndex)
            {
                case 1:
                    return _fingerIndexNames.RightThumb;

                case 2:
                    return _fingerIndexNames.RightIndex;

                case 3:
                    return _fingerIndexNames.RightMiddle;

                case 4:
                    return _fingerIndexNames.RightRing;

                case 5:
                    return _fingerIndexNames.RightLittle;

                case 6:
                    return _fingerIndexNames.LeftThumb;

                case 7:
                    return _fingerIndexNames.LeftIndex;

                case 8:
                    return _fingerIndexNames.LeftMiddle;

                case 9:
                    return _fingerIndexNames.LeftRing;

                case 10:
                    return _fingerIndexNames.LeftLittle;

                default:
                    return _fingerIndexNames.Unknown;
            }
        }
    }
}