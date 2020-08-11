using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AccessTime
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Code;

		private AccessTimeZoneCode _timezone;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Holiday;
	}
}
