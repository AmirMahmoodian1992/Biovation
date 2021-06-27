using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct SmartCardSectorLayout
	{
		public byte SectorNumber;

		[MarshalAs(UnmanagedType.U1)]
		public SmartCardKeyTypes KeyType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		public byte[] KeyData;

		public byte BlockNumber;

		public byte Start;

		public byte Length;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[] AID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		private byte[] _reserved;
	}
}
