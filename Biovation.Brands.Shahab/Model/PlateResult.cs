using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Shahab.Model
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode/*, Pack = 1*/)]
    public struct PlateResult
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string PlateData;
        public float DetectionPrecision;
        public RectangleCoordinates PlateCoordinates;
        public IntPtr PlateImage;
        public IntPtr CarImage;
        public byte Direction;//DIR_UNKNOWN = 0, DIR_COMMING = 1, DIR_DEPARTING = 2
        public byte CharactersCount;//تعداد کل نویسه ها (ارقام و حروف)
        public byte LettersCount;//تعداد حروف یافت شده در پلاک
        public byte PlateRepeatCount;//چند بار یک پلاک در فریمهای مختلف تکرار شده است
        public byte RegionOfInterest;//در کدام ناحیه مورد علاقه، این پلاک یافت شده است
    };
}
