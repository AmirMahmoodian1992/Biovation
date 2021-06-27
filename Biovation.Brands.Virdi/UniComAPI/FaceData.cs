using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct FaceData
	{
		public FaceInfo[] GetFaceInfo()
		{
			var array = new FaceInfo[_prvFaceNumber];
			var num = 0;
			while ((long)num < (long)(ulong)_prvFaceNumber)
			{
				array[num] = (FaceInfo)Marshal.PtrToStructure(_prvFaceInfo[num], typeof(FaceInfo));
				num++;
			}
			return array;
		}

		public void SetFaceInfo(FaceInfo[] value, UnmanegedMemoryScope scope)
		{
			_prvFaceInfo = new IntPtr[10];
			var num = value.Length;
			var flag = num > 10;
			if (flag)
			{
				num = 10;
			}
			for (var i = 0; i < num; i++)
			{
				_prvFaceInfo[i] = scope.GetIntPtr(Marshal.SizeOf(typeof(FaceInfo)));
				Marshal.StructureToPtr(value[i], _prvFaceInfo[i], true);
			}
			_prvFaceNumber = (uint)num;
		}

		private uint _prvFaceNumber;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		private IntPtr[] _prvFaceInfo;
	}
}
