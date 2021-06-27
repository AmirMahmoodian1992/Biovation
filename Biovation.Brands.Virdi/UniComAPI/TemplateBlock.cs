using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct TemplateBlock
	{
		public byte[] GetData()
		{
			var array = new byte[_prvLength];
			Marshal.Copy(_prvData, array, 0, (int)_prvLength);
			return array;
		}

		public void SetData(byte[] value, UnmanegedMemoryScope scope)
		{
			var flag = value == null || value.Length == 0;
			if (flag)
			{
				_prvData = IntPtr.Zero;
				_prvLength = 0u;
			}
			else
			{
				_prvLength = (uint)value.Length;
				_prvData = scope.GetIntPtr(value.Length);
				Marshal.Copy(value, 0, _prvData, value.Length);
			}
		}

		private uint _prvLength;

		private IntPtr _prvData;
	}
}
