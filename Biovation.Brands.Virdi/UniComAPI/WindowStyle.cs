using System;

namespace Biovation.Brands.Virdi.UniComAPI
{
	[Flags]
	public enum WindowStyle : uint
	{
		Popup = 0u,
		Invisible = 1u,
		NoFingerprintImage = 65536u,
		NoWelcome = 131072u,
		NoTopmost = 262144u
	}
}
