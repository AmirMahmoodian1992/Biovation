using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct PictureData
	{
		public byte[] GetData()
		{
			var flag = _prvData == IntPtr.Zero;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				var array = new byte[_prvLength];
				Marshal.Copy(_prvData, array, 0, (int)_prvLength);
				result = array;
			}
			return result;
		}

		public void SetData(byte[] value, UnmanegedMemoryScope scope)
		{
			_prvFormat = "jpg";
			var flag = value == null || value.Length == 0;
			if (flag)
			{
				_prvData = IntPtr.Zero;
				_prvLength = 0u;
			}
			else
			{
				_prvLength = (uint)value.Length;
				_prvData = scope.GetIntPtr((int)_prvLength);
				Marshal.Copy(value, 0, _prvData, (int)_prvLength);
			}
		}

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
		private string _prvFormat;

		private uint _prvLength;

		private IntPtr _prvData;
	}
}
