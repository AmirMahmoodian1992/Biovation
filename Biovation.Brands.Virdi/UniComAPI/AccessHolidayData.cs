using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AccessHolidayData
	{
		public uint HolidayNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public AccessHoliday Holiday;
	}
}
