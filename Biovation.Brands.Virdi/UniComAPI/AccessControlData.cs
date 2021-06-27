using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AccessControlData
	{
		public AccessTimezoneData? GetTimeZoneData()
		{
			var flag = _prvData != IntPtr.Zero && _prvDataType == AccessControlDataType.TimeZone;
			AccessTimezoneData? result;
			if (flag)
			{
				result = new AccessTimezoneData?((AccessTimezoneData)Marshal.PtrToStructure(_prvData, typeof(AccessTimezoneData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetTimeZoneData(AccessTimezoneData value, UnmanegedMemoryScope scope)
		{
			_prvDataType = AccessControlDataType.TimeZone;
			_prvData = scope.GetIntPtr(Marshal.SizeOf(typeof(AccessTimezoneData)));
			Marshal.StructureToPtr(value, _prvData, false);
		}

		public AccessHolidayData? GetHolidayData()
		{
			var flag = _prvData != IntPtr.Zero && _prvDataType == AccessControlDataType.Holiday;
			AccessHolidayData? result;
			if (flag)
			{
				result = new AccessHolidayData?((AccessHolidayData)Marshal.PtrToStructure(_prvData, typeof(AccessHolidayData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetHolidayData(AccessHolidayData value, UnmanegedMemoryScope scope)
		{
			_prvDataType = AccessControlDataType.Holiday;
			_prvData = scope.GetIntPtr(Marshal.SizeOf(typeof(AccessHolidayData)));
			Marshal.StructureToPtr(value, _prvData, false);
		}

		public AccessTimeData? GetTimeData()
		{
			var flag = _prvData != IntPtr.Zero && _prvDataType == AccessControlDataType.Time;
			AccessTimeData? result;
			if (flag)
			{
				result = new AccessTimeData?((AccessTimeData)Marshal.PtrToStructure(_prvData, typeof(AccessTimeData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetTimeData(AccessTimeData value, UnmanegedMemoryScope scope)
		{
			_prvDataType = AccessControlDataType.Time;
			_prvData = scope.GetIntPtr(Marshal.SizeOf(typeof(AccessTimeData)));
			Marshal.StructureToPtr(value, _prvData, false);
		}

		public AccessGroupData? GetGroupData()
		{
			var flag = _prvData != IntPtr.Zero && _prvDataType == AccessControlDataType.Group;
			AccessGroupData? result;
			if (flag)
			{
				result = new AccessGroupData?((AccessGroupData)Marshal.PtrToStructure(_prvData, typeof(AccessGroupData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetGroupData(AccessGroupData value, UnmanegedMemoryScope scope)
		{
			_prvDataType = AccessControlDataType.Group;
			_prvData = scope.GetIntPtr(Marshal.SizeOf(typeof(AccessGroupData)));
			Marshal.StructureToPtr(value, _prvData, false);
		}

		private AccessControlDataType _prvDataType;

		private IntPtr _prvData;
	}
}
