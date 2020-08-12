using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AccessTimeData
	{
		public uint AccessTimeNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public AccessTime[] AccessTime;
	}
}
