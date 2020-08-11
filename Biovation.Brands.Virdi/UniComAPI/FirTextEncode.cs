using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct FirTextEncode
	{
		public string TextFir
		{
			get
			{
				var flag = _prvTextFir != IntPtr.Zero;
				string result;
				if (flag)
				{
					var isWideChar = IsWideChar;
					if (isWideChar)
					{
						result = Marshal.PtrToStringUni(_prvTextFir);
					}
					else
					{
						result = Marshal.PtrToStringAnsi(_prvTextFir);
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				var flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new InvalidOperationException("value can not be null or empty");
				}
				var isWideChar = IsWideChar;
				if (isWideChar)
				{
					_prvTextFir = Marshal.StringToHGlobalUni(value);
				}
				else
				{
					_prvTextFir = Marshal.StringToHGlobalAnsi(value);
				}
			}
		}

		[MarshalAs(UnmanagedType.Bool)]
		public bool IsWideChar;

		private IntPtr _prvTextFir;
	}
}
