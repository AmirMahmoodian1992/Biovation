using System;

namespace Biovation.Brands.Virdi.UniComAPI
{
	[Flags]
	public enum UserProperty : byte
	{
		None = 0,
		Finger = 1,
		FpCard = 2,
		Password = 4,
		Card = 8,
		CardID = 16,
		Operation = 32,
		Fp1ToN = 64,
		Admin = 128
	}
}
