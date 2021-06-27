using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct MatchOption
	{
		private byte _structureType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
		private byte[] _noMatchFinger;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		private uint[] _reserved;
	}
}
