using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct TerminalInfo
	{
		public uint TerminalID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] TerminalIp;

		public byte TerminalStatus;

		public byte DoorStatus;

		public byte CoverStatus;

		public byte LockStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] ExtSignal;

		public UcsVersion FirmwareVersion;

		public UcsVersion ProtocolVersion;

		public UcsVersion CardReaderVersion;

		public ushort ModelNo;

		public byte TerminalType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		public byte[] MacAddr;
	}
}
