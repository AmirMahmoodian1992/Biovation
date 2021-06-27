using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct CallbackParam1
	{
		public CallbackDataType DataType
		{
			get
			{
				return _prvDataType;
			}
		}

		public UserInfo? GetUserInfo()
		{
			var flag = _prvUserInfo != IntPtr.Zero && _prvDataType == CallbackDataType.UserInfo;
			UserInfo? result;
			if (flag)
			{
				result = new UserInfo?((UserInfo)Marshal.PtrToStructure(_prvUserInfo, typeof(UserInfo)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public UserData? GetUserData()
		{
			var flag = _prvUserInfo != IntPtr.Zero && _prvDataType == CallbackDataType.UserData;
			UserData? result;
			if (flag)
			{
				result = new UserData?((UserData)Marshal.PtrToStructure(_prvUserInfo, typeof(UserData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public AccessLogData? GetAccessLogData()
		{
			var flag = _prvUserInfo != IntPtr.Zero && _prvDataType == CallbackDataType.AccessLog;
			AccessLogData? result;
			if (flag)
			{
				result = new AccessLogData?((AccessLogData)Marshal.PtrToStructure(_prvUserInfo, typeof(AccessLogData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public FaceInfo? GetFaceInfo()
		{
			var flag = _prvUserInfo != IntPtr.Zero && _prvDataType == CallbackDataType.FaceInfo;
			FaceInfo? result;
			if (flag)
			{
				result = new FaceInfo?((FaceInfo)Marshal.PtrToStructure(_prvUserInfo, typeof(FaceInfo)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		private CallbackDataType _prvDataType;

		private IntPtr _prvUserInfo;
	}
}
