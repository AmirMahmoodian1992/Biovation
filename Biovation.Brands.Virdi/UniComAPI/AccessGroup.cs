using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AccessGroup
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] Code;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] AccssTime1;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] AccssTime2;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] AccssTime3;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public sbyte[] AccssTime4;
	}
}
