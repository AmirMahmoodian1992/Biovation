﻿using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;

namespace Biovation.Brands.ZK.Manager
{
    public class BiometricTemplateManager
    {
        public static Lookup GetFingerIndex(int fingerIndex)
        {
            switch (fingerIndex)
            {
                case 1:
                    return FingerIndexNames.RightThumb;

                case 2:
                    return FingerIndexNames.RightIndex;

                case 3:
                    return FingerIndexNames.RightMiddle;

                case 4:
                    return FingerIndexNames.RightRing;

                case 5:
                    return FingerIndexNames.RightLittle;

                case 6:
                    return FingerIndexNames.LeftLittle;

                case 7:
                    return FingerIndexNames.LeftRing;

                case 8:
                    return FingerIndexNames.LeftMiddle;

                case 9:
                    return FingerIndexNames.LeftIndex;

                case 10:
                    return FingerIndexNames.LeftThumb;

                default:
                    return FingerIndexNames.Unknown;
            }
        }
    }
}
