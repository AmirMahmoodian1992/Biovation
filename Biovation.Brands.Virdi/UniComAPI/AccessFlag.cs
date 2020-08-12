using System;

namespace Biovation.Brands.Virdi.UniComAPI
{
	[Flags]
	public enum AccessFlag : byte
	{
		None = 0,
		BlackList = 1,
		Face1ToN = 2,
		ExceptPassback = 128
	}
}
