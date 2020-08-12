using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AccessHoliday
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Code;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public DateInfo[] Date;
	}
}
