using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct Init0
	{
		public InitFlag General
		{
			get
			{
				return _prvGeneral;
			}
			set
			{
				_prvGeneral = value;
			}
		}

		private InitFlag _prvGeneral;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 127)]
		private byte[] _reserved;
	}
}
