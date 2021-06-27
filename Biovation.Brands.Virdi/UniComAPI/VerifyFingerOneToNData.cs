using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct VerifyFingerOneToNData
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
				return (VerifyAuthenticationMode)_prvAuthMode;
			}
			set
			{
				_prvAuthMode = (uint)value;
			}
		}

		private uint _prvAuthMode;

		[MarshalAs(UnmanagedType.U4)]
		public uint PrvSecurityLevel;

		public uint InputIDLength;

		public UnmanagedData Finger;
	}
}
