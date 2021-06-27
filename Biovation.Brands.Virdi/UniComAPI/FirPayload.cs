using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	internal struct FirPayload
	{
		public byte[] Data
		{
			get
			{
				var flag = _prvData != IntPtr.Zero;
				byte[] result;
				if (flag)
				{
					var array = new byte[_length];
					Marshal.Copy(_prvData, array, 0, (int)_length);
					result = array;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				_length = (uint)value.Length;
				_prvData = Marshal.AllocHGlobal(value.Length);
				Marshal.Copy(value, 0, _prvData, value.Length);
			}
		}

		private uint _length;

		private IntPtr _prvData;
	}
}
