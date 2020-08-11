using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct TerminalOption
	{
		public SecurityLevel VerifySecurityLevel
		{
			get
			{
				return (SecurityLevel)(prvSecurityLevel & 15);
			}
			set
			{
				prvSecurityLevel = (byte) ((prvSecurityLevel & 240) | (byte)value);
			}
		}

		public SecurityLevel IdentifySecurityLevel
		{
			get
			{
				return (SecurityLevel)(prvSecurityLevel >> 4);
			}
			set
			{
				prvSecurityLevel = (byte) ((prvSecurityLevel & 15) | (byte)((byte)value << 4));
			}
		}

		public AntiPassbackLevel Antipassback
		{
			get
			{
				return (AntiPassbackLevel)prvAntipassback;
			}
			set
			{
				prvAntipassback = (byte)value;
			}
		}

		public AccessLevel AccessLevel
		{
			get
			{
				return (AccessLevel)prvAccessLevel;
			}
			set
			{
				prvAccessLevel = (byte)value;
			}
		}

		public TerminalOptionFlag Flags;

		private byte prvSecurityLevel;

		public byte InputIDLength;

		[MarshalAs(UnmanagedType.U1)]
		public bool AutoEnterKey;

		public byte Sound;

		public TerminalAuthenticationMode Authentication;

		public ApplicationType Applicaction;

		private byte prvAntipassback;

		public NetworkInfo Network;

		public ServerInfo Server;

		public InputIDType InputIDType;

		private byte prvAccessLevel;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string PrintName;

		public TerminalSchedule Schedule;
	}
}
