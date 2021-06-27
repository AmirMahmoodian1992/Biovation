using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct SmartCardLayout
	{
		public SmartCardType CardType;

		public SmartCardReadType ReadType;

		public SmartCardSerialFormat SerialFormat;

		public byte SectorNumber;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		private byte[] _reserved;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public SmartCardSectorLayout[] Layouts;
	}
}
