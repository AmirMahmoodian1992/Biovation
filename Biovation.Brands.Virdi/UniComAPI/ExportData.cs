using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct ExportData
	{
		public FingerBlock[] GetFingerInfo()
		{
			var flag = _prvFingerNum == 0;
			FingerBlock[] result;
			if (flag)
			{
				result = new FingerBlock[0];
			}
			else
			{
				var array = new FingerBlock[(int)_prvFingerNum];
				var intPtr = _prvFingerInfo;
				var offset = Marshal.SizeOf(typeof(FingerBlock));
				for (var i = 0; i < (int)_prvFingerNum; i++)
				{
					array[i] = (FingerBlock)Marshal.PtrToStructure(intPtr, typeof(FingerBlock));
					intPtr += offset;
				}
				result = array;
			}
			return result;
		}

		public void SetFingerInfo(FingerBlock[] value, UnmanegedMemoryScope scope)
		{
			_prvLength = (uint)Marshal.SizeOf(typeof(ExportData));
			var flag = value == null || value.Length == 0;
			if (flag)
			{
				_prvFingerInfo = IntPtr.Zero;
				_prvFingerNum = 0;
			}
			else
			{
				_prvFingerNum = (byte)value.Length;
				var num = Marshal.SizeOf(typeof(FingerBlock));
				_prvFingerInfo = scope.GetIntPtr((int)_prvFingerNum * num);
				var intPtr = _prvFingerInfo;
				for (var i = 0; i < (int)_prvFingerNum; i++)
				{
					Marshal.StructureToPtr(value[i], intPtr, false);
					intPtr += num;
				}
			}
		}

		internal void DangerousSetFingerInfo(FingerBlock[] value)
		{
			var flag = _prvFingerInfo == IntPtr.Zero;
			if (flag)
			{
				throw new InvalidOperationException("Invalid FingerInfo Pointer");
			}
			var flag2 = value.Length != (int)FingerNumber;
			if (flag2)
			{
				throw new InvalidOperationException("New fingerblock array length must match current FingerNumber");
			}
			_prvFingerNum = (byte)value.Length;
			var offset = Marshal.SizeOf(typeof(FingerBlock));
			var intPtr = _prvFingerInfo;
			for (var i = 0; i < (int)_prvFingerNum; i++)
			{
				Marshal.StructureToPtr(value[i], intPtr, false);
				intPtr += offset;
			}
		}

		public byte FingerNumber
		{
			get
			{
				return _prvFingerNum;
			}
		}

		private uint _prvLength;

		public TemplateSize TemplateType;

		private byte _prvFingerNum;

		public FingerID DefaultFingerID;

		public byte SamplesPerFinger;

		private byte _reserved;

		private IntPtr _prvFingerInfo;
	}
}
