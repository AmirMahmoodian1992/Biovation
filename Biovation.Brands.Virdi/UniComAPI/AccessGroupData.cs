using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AccessGroupData
	{
		public uint AccessGroupNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public AccessGroup[] AccessGroup;
	}
}
