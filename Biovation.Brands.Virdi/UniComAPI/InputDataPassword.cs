using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct InputDataPassword
	{
		public uint UserID;

		public uint AuthMode;

		[MarshalAs(UnmanagedType.Struct)]
		public UnmanagedData Password;
	}
}
