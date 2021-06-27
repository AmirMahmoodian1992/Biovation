using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct InputDataCard
	{
		public uint AuthMode;

		[MarshalAs(UnmanagedType.Struct)]
		public UnmanagedData RFID;
	}
}
