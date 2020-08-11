using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	[StructLayout(LayoutKind.Sequential)]
	public class WindowOption
	{
		public WindowOption()
		{
			prvWindowStyle = WindowStyle.Invisible;
			prvCallBackType1 = 1u;
			prvLength = (uint)Marshal.SizeOf(GetType());
		}

		private uint prvLength;

		private WindowStyle prvWindowStyle;

		private IntPtr prvParentWnd;

		private IntPtr prvFingerWnd;

		private uint prvCallBackType0;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		public CaptureCallBack CaptureCallBack;

		private IntPtr prvUserCallBackParam0;

		private uint prvCallBackType1;

		private IntPtr prvFinishCallBack;

		private IntPtr prvUserCallBackParam1;

		[MarshalAs(UnmanagedType.LPStr)]
		private string prvCaptionMsg;

		[MarshalAs(UnmanagedType.LPStr)]
		private string prvCancelMsg;

		private IntPtr prvWindowOptionTwo;
	}
}
