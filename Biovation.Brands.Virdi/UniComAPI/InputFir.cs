using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct InputFir
	{
		public uint FirHandle
		{
			set
			{
				_prvFingerPtr = Marshal.AllocHGlobal(4);
				Marshal.WriteInt32(_prvFingerPtr, (int)value);
				_prvFrom = InputFirForm.FirFromHandle;
			}
		}

		public FirTextEncode TextFir
		{
			set
			{
				var cb = Marshal.SizeOf(typeof(FirTextEncode));
				_prvFingerPtr = Marshal.AllocHGlobal(cb);
				Marshal.StructureToPtr(value, _prvFingerPtr, true);
				_prvFrom = InputFirForm.FirFormTextEncode;
			}
		}

		public void Release()
		{
			var flag = _prvFrom == InputFirForm.FirFormTextEncode;
			if (flag)
			{
				Marshal.FreeHGlobal(Marshal.ReadIntPtr(_prvFingerPtr + 4));
			}
			Marshal.FreeHGlobal(_prvFingerPtr);
		}

		private InputFirForm _prvFrom;

		private IntPtr _prvFingerPtr;
	}
}
