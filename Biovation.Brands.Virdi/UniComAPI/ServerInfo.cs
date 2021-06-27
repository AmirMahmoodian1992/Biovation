using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct ServerInfo
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] IP;

		public ushort Port;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		private byte[] _reserved;
	}
}
