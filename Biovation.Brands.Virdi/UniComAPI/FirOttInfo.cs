using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct FirOttInfo
	{
		public uint Index;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] UUID;
	}
}
