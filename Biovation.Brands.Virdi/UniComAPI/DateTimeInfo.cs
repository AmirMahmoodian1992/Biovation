using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct DateTimeInfo
	{
		public DateTime ToDateTime()
		{
			return new DateTime((int)Year, (int)Month, (int)Day, (int)Hour, (int)Min, 0);
		}

		[MarshalAs(UnmanagedType.U2)]
		public ushort Year;

		[MarshalAs(UnmanagedType.U1)]
		public byte Month;

		[MarshalAs(UnmanagedType.U1)]
		public byte Day;

		[MarshalAs(UnmanagedType.U1)]
		public byte Hour;

		[MarshalAs(UnmanagedType.U1)]
		public byte Min;

		[MarshalAs(UnmanagedType.U1)]
		public byte Sec;

		[MarshalAs(UnmanagedType.U1)]
		private byte _reserved;
	}
}
