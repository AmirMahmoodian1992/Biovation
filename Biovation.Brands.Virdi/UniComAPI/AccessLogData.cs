using System;
using System.Runtime.InteropServices;
// ReSharper disable All
#pragma warning disable 169
#pragma warning disable 649

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AccessLogData
	{
		public UnmanagedData? GetRfid()
		{
			var flag = _prvRfid != IntPtr.Zero;
			UnmanagedData? result;
			if (flag)
			{
				result = new UnmanagedData?((UnmanagedData)Marshal.PtrToStructure(_prvRfid, typeof(UnmanagedData)));
			}
			else
			{
				result = null;
			}
			return result;
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

		[MarshalAs(UnmanagedType.U4)]
		public uint UserID;

		[MarshalAs(UnmanagedType.Struct)]
		public DateTimeInfo DateTime;

		[MarshalAs(UnmanagedType.U1)]
		public VerifyAuthenticationMode AuthMode;

		[MarshalAs(UnmanagedType.U1)]
		public AuthenticationType AuthType;

		[MarshalAs(UnmanagedType.U1)]
		private byte _deviceID;

		[MarshalAs(UnmanagedType.U1)]
		private byte _readerID;

		[MarshalAs(UnmanagedType.Bool)]
		public bool IsAuthorized;

		private IntPtr _prvRfid;

		[MarshalAs(UnmanagedType.U4)]
		public AuthenticationError ErrorCode;

		private IntPtr _prvPictureData;

		public float Latitude;

		public float Longitude;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] Reserved2;
	}
}
