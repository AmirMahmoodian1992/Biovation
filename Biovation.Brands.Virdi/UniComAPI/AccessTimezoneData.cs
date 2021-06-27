using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AccessTimezoneData
	{
		public uint TimezoneNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public AccessTimeZone[] Timezone;
	}
}
