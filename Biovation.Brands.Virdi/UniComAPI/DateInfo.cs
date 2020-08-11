using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct DateInfo
	{
		[MarshalAs(UnmanagedType.U2)]
		public ushort Year;

		[MarshalAs(UnmanagedType.U1)]
		public byte Month;

		[MarshalAs(UnmanagedType.U1)]
		public byte Day;
	}
}
