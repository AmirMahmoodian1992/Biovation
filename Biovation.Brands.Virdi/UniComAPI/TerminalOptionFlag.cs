using System;

namespace Biovation.Brands.Virdi.UniComAPI
{
	[Flags]
	public enum TerminalOptionFlag : uint
	{
		None = 0u,
		SecurityLevel = 1u,
		InputIDLength = 2u,
		AutoEnterKey = 4u,
		Sound = 8u,
		Authentication = 16u,
		Applicaction = 32u,
		Antipassback = 64u,
		Network = 128u,
		Server = 256u,
		InputIDType = 512u,
		AccessLevel = 1024u,
		PrintText = 2048u,
		Schedule = 4096u,
		Na14 = 8192u,
		Na15 = 16384u,
		Na16 = 32768u,
		Na17 = 65536u,
		Na18 = 131072u,
		Na19 = 262144u,
		Na20 = 524288u,
		Na21 = 1048576u,
		Na22 = 2097152u,
		Na23 = 4194304u,
		Na24 = 8388608u,
		Na25 = 16777216u,
		Na26 = 33554432u,
		Na27 = 67108864u,
		Na28 = 134217728u,
		Na29 = 268435456u,
		Na30 = 536870912u,
		Na31 = 1073741824u,
		Na32 = 2147483648u
	}
}
