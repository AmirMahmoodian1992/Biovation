using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct PrivateMessage
	{
		private ushort _messageSize;

		private ushort _displayTime;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Message;
	}
}
