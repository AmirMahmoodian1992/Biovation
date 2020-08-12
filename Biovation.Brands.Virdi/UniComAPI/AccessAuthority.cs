using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AccessAuthority
	{
		public UnmanagedData? GetAccessGroup()
		{
			var flag = _prvAccessGroup != IntPtr.Zero;
			UnmanagedData? result;
			if (flag)
			{
				result = new UnmanagedData?((UnmanagedData)Marshal.PtrToStructure(_prvAccessGroup, typeof(UnmanagedData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetAccessGroup(UnmanagedData value, UnmanegedMemoryScope scope)
		{
			_prvAccessGroup = scope.GetIntPtr(Marshal.SizeOf(typeof(UnmanagedData)));
			Marshal.StructureToPtr(value, _prvAccessGroup, false);
		}

		public AccessDate? GetAccessDate()
		{
			var flag = _prvAccessDate != IntPtr.Zero;
			AccessDate? result;
			if (flag)
			{
				result = new AccessDate?((AccessDate)Marshal.PtrToStructure(_prvAccessDate, typeof(AccessDate)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetAccessDate(AccessDate value, UnmanegedMemoryScope scope)
		{
			_prvAccessDate = scope.GetIntPtr(Marshal.SizeOf(typeof(AccessDate)));
			Marshal.StructureToPtr(value, _prvAccessDate, false);
		}

		private IntPtr _prvAccessGroup;

		public AccessDateType AccessDateType;

		private IntPtr _prvAccessDate;
	}
}
