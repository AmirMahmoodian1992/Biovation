using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct UserData
	{
		public UserAuthData? GetAuthData()
		{
			var flag = _prvAuthData != IntPtr.Zero;
			UserAuthData? result;
			if (flag)
			{
				result = new UserAuthData?((UserAuthData)Marshal.PtrToStructure(_prvAuthData, typeof(UserAuthData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetAuthData(UserAuthData value, UnmanegedMemoryScope scope)
		{
			_prvAuthData = scope.GetIntPtr(Marshal.SizeOf(typeof(UserAuthData)));
			Marshal.StructureToPtr(value, _prvAuthData, false);
		}

		public PictureData? GetPictureData()
		{
			var flag = _prvPictureData != IntPtr.Zero;
			PictureData? result;
			if (flag)
			{
				result = new PictureData?((PictureData)Marshal.PtrToStructure(_prvPictureData, typeof(PictureData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetPictureData(PictureData value, UnmanegedMemoryScope scope)
		{
			_prvPictureData = scope.GetIntPtr(Marshal.SizeOf(typeof(PictureData)));
			Marshal.StructureToPtr(value, _prvPictureData, false);
		}

		public UserInfo UserInfo;

		private IntPtr _prvAuthData;

		private IntPtr _prvPictureData;
	}
}
