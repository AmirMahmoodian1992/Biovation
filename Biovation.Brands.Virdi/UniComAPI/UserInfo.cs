using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct UserInfo
	{
		public UnmanagedData? GetUserName()
		{
			var flag = _prvUserName != IntPtr.Zero;
			UnmanagedData? result;
			if (flag)
			{
				result = new UnmanagedData?((UnmanagedData)Marshal.PtrToStructure(_prvUserName, typeof(UnmanagedData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetUserName(UnmanagedData value, UnmanegedMemoryScope scope)
		{
			_prvUserName = scope.GetIntPtr(Marshal.SizeOf(typeof(UnmanagedData)));
			Marshal.StructureToPtr(value, _prvUserName, false);
		}

		public UnmanagedData? GetUniqueID()
		{
			var flag = _prvUniqueID != IntPtr.Zero;
			UnmanagedData? result;
			if (flag)
			{
				result = new UnmanagedData?((UnmanagedData)Marshal.PtrToStructure(_prvUniqueID, typeof(UnmanagedData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetUniqueID(UnmanagedData value, UnmanegedMemoryScope scope)
		{
			_prvUniqueID = scope.GetIntPtr(Marshal.SizeOf(typeof(UnmanagedData)));
			Marshal.StructureToPtr(value, _prvUniqueID, false);
		}

		public AccessAuthority? GetAccessAuthority()
		{
			var flag = _prvAccessAuthority != IntPtr.Zero;
			AccessAuthority? result;
			if (flag)
			{
				result = new AccessAuthority?((AccessAuthority)Marshal.PtrToStructure(_prvAccessAuthority, typeof(AccessAuthority)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetAccessAuthority(AccessAuthority value, UnmanegedMemoryScope scope)
		{
			_prvAccessAuthority = scope.GetIntPtr(Marshal.SizeOf(typeof(AccessAuthority)));
			Marshal.StructureToPtr(value, _prvAccessAuthority, false);
		}

		public uint UserID;

		private IntPtr _prvUserName;

		private IntPtr _prvUniqueID;

		public UserProperty Property;

		public AuthType AuthType;

		public AccessFlag AccessFlag;

		private IntPtr _prvAccessAuthority;

		private byte _prvPartition;

		private byte _prvPropertyEx;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		private byte[] _reserved;
	}
}
