using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AccessTimeZone
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Code;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
		public TimeZone Zone;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		private byte[] _reserved;
	}
}
