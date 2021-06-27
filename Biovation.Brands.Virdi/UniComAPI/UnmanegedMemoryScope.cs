using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public class UnmanegedMemoryScope : IDisposable
	{
		public static UnmanegedMemoryScope Begin()
		{
			return new UnmanegedMemoryScope();
		}

		private UnmanegedMemoryScope()
		{
			_prvPointers = new List<IntPtr>();
		}

		internal IntPtr GetIntPtr(int length)
		{
			var flag = _prvIsDisposed;
			if (flag)
			{
				throw new ObjectDisposedException("Already diposed");
			}
			var flag2 = length > 0;
			IntPtr result;
			if (flag2)
			{
				var intPtr = Marshal.AllocHGlobal(length);
				_prvPointers.Add(intPtr);
				result = intPtr;
			}
			else
			{
				result = IntPtr.Zero;
			}
			return result;
		}

		internal IntPtr StringToHGlobalAnsi(string data)
		{
			var flag = _prvIsDisposed;
			if (flag)
			{
				throw new ObjectDisposedException("Already diposed");
			}
			var intPtr = Marshal.StringToHGlobalAnsi(data);
			_prvPointers.Add(intPtr);
			return intPtr;
		}

		internal IntPtr StringToHGlobalUni(string data)
		{
			var flag = _prvIsDisposed;
			if (flag)
			{
				throw new ObjectDisposedException("Already diposed");
			}
			var intPtr = Marshal.StringToHGlobalUni(data);
			_prvPointers.Add(intPtr);
			return intPtr;
		}

		public void Dispose()
		{
			var flag = !_prvIsDisposed;
			if (flag)
			{
				_prvIsDisposed = true;
				foreach (var current in _prvPointers)
				{
					try
					{
						Marshal.FreeHGlobal(current);
					}
					catch
					{
					}
				}
				_prvPointers.Clear();
			}
		}

		~UnmanegedMemoryScope()
		{
			Dispose();
		}

		private List<IntPtr> _prvPointers;

		private bool _prvIsDisposed;
	}
}
