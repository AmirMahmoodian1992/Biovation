using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct ImageData
	{
		public byte[] GetImage(int length)
		{
			var array = new byte[length];
			Marshal.Copy(_prvImagePtr, array, 0, length);
			return array;
		}

		private uint _prvLength;

		private IntPtr _prvImagePtr;
	}
}
