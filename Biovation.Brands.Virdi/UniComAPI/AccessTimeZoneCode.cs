using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AccessTimeZoneCode
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Sun;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Mon;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Tue;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Wed;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Thu;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Fri;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Sat;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Hol;
	}
}
