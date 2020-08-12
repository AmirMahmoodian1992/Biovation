using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct FingerData
	{
		public void SetExportData(ExportData value, UnmanegedMemoryScope scope)
		{
			_prvExportData = scope.GetIntPtr(Marshal.SizeOf(typeof(ExportData)));
			Marshal.StructureToPtr(value, _prvExportData, false);
		}

		public ExportData? GetExportData()
		{
			var flag = _prvExportData != IntPtr.Zero;
			ExportData? result;
			if (flag)
			{
				result = new ExportData?((ExportData)Marshal.PtrToStructure(_prvExportData, typeof(ExportData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public TemplateFormat TemplateFormat
		{
			get
			{
				return (TemplateFormat)_prvTemplateFormat;
			}
			set
			{
				_prvTemplateFormat = (byte)value;
			}
		}

		public SecurityLevel SecurityLevel;

		private byte _prvTemplateFormat;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		public byte[] DuressFinger;

		[MarshalAs(UnmanagedType.Bool)]
		public bool IsCheckSimilarFinger;

		private IntPtr _prvExportData;
	}
}
