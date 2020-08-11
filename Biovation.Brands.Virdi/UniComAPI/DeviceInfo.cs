using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct DeviceInfo
	{
		public ushort DeviceID;

		public ushort Instance;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string Name;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string Description;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string Dll;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string Sys;

		public int Brightness;

		public int Contrast;

		public int Gain;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public int[] Reserved;
	}
}
