using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct PublicMessage
	{
		private ushort _length;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		private byte[] _reserved;

		public DateInfo StartDate;

		public DateInfo EndDate;

		public TimeInfo StartTime;

		public TimeInfo EndTime;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Message;
	}
}
