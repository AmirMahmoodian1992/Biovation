using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct NetworkInfo
	{
		public IpType NetworkType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] IP;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Subnet;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Gateway;
	}
}
