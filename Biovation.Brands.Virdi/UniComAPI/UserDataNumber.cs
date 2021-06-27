using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct UserDataNumber
	{
		public int MaxUser
		{
			get;
			set;
		}

		public int RegFinger
		{
			get;
			set;
		}

		public int MaxFinger
		{
			get;
			set;
		}

		public int RegFace
		{
			get;
			set;
		}

		public int MaxFace
		{
			get;
			set;
		}

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
		private byte[] _reserved;
	}
}
