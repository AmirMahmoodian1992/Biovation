using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct SampleInfo
	{
		public uint ID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
		public byte[] SampleCount;
	}
}
