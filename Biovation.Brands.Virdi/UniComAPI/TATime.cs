using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct TaTime
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Start1;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Start2;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Start3;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Start4;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] End1;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] End2;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] End3;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] End4;
	}
}
