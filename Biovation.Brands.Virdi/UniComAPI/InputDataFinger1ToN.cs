using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct InputDataFinger1ToN
	{
		public uint AuthMode;

		public uint SecurityLevel;

		public uint InputIDLength;

		[MarshalAs(UnmanagedType.Struct)]
		public UnmanagedData Finger;
	}
}
