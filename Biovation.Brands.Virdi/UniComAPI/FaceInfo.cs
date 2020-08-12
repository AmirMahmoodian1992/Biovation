using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct FaceInfo
	{
		public void SetBytes(byte[] value, UnmanegedMemoryScope scope)
		{
			var flag = value == null || value.Length == 0;
			if (flag)
			{
				_prvLength = 0u;
				_prvData = IntPtr.Zero;
			}
			else
			{
				_prvLength = (uint)value.Length;
				_prvData = scope.GetIntPtr(value.Length);
				Marshal.Copy(value, 0, _prvData, value.Length);
			}
		}

		public byte[] GetBytes()
		{
			var flag = _prvData != IntPtr.Zero;
			byte[] result;
			if (flag)
			{
				var array = new byte[_prvLength];
				var flag2 = _prvLength > 0u;
				if (flag2)
				{
					Marshal.Copy(_prvData, array, 0, (int)_prvLength);
				}
				result = array;
			}
			else
			{
				result = null;
			}
			return result;
		}

		private uint _prvLength;

		private IntPtr _prvData;
	}
}
