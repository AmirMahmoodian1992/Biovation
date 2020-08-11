using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct FpInfo
	{
		[MarshalAs(UnmanagedType.U4)]
		public uint ID;

		[MarshalAs(UnmanagedType.U1)]
		public byte FingerID;

		[MarshalAs(UnmanagedType.U1)]
		public byte SampleNumber;

		[MarshalAs(UnmanagedType.U4)]
		public uint Reserved1;

		[MarshalAs(UnmanagedType.U4)]
		public uint Reserved2;
	}
}
