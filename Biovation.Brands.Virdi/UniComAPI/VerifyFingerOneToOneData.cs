using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct VerifyFingerOneToOneData
	{
		public SecurityLevel SecurityLevel
		{
			get
			{
				return (SecurityLevel)PrvSecurityLevel;
			}
			set
			{
				PrvSecurityLevel = (uint)value;
			}
		}

		public VerifyAuthenticationMode AuthMode
		{
			get
			{
				return (VerifyAuthenticationMode)PrvAuthMode;
			}
			set
			{
				PrvAuthMode = (uint)value;
			}
		}

		public uint UserID;

		public uint PrvAuthMode;

		[MarshalAs(UnmanagedType.U4)]
		public uint PrvSecurityLevel;

		public UnmanagedData Finger;
	}
}
