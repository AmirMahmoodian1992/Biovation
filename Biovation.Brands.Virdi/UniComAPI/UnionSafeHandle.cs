using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public class UnionSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		[DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_Terminate")]
		private static extern ApiReturn Terminate(IntPtr hHandle);

		public UnionSafeHandle() : base(true)
		{
		}

		protected override bool ReleaseHandle()
		{
			bool result;
			try
			{
				Terminate(handle);
			}
			catch
			{
				result = false;
				return result;
			}
			result = true;
			return result;
		}
	}
}
