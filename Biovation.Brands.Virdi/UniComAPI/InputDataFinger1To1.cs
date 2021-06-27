using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct InputDataFinger1To1
	{
		public uint UserID;

		public uint AuthMode;

		public uint SecurityLevel;

		[MarshalAs(UnmanagedType.Struct)]
		public UnmanagedData Finger;
	}
}
