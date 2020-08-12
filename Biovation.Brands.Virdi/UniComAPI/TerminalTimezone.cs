using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct TerminalTimezone
	{
		[MarshalAs(UnmanagedType.U1)]
		public bool IsUsed;

		public byte StartHour;

		public byte StartMin;

		public byte EndHour;

		public byte EndMin;
	}
}
