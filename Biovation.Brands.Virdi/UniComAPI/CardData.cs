using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct CardData
	{
		public UnmanagedData[] GetCardIDs()
		{
			var array = new UnmanagedData[_prvCardNum];
			var num = 0;
			while ((long)num < (long)(ulong)_prvCardNum)
			{
				array[num] = (UnmanagedData)Marshal.PtrToStructure(_prvRfid[num], typeof(UnmanagedData));
				num++;
			}
			return array;
		}

		public void SetCardIDs(UnmanagedData[] value, UnmanegedMemoryScope scope)
		{
			_prvRfid = new IntPtr[5];
			var flag = value == null || value.Length == 0;
			if (flag)
			{
				_prvCardNum = 0u;
			}
			else
			{
				_prvCardNum = (uint)value.Length;
				var num = 0;
				while ((long)num < (long)(ulong)_prvCardNum)
				{
					_prvRfid[num] = scope.GetIntPtr(Marshal.SizeOf(typeof(UnmanagedData)));
					Marshal.StructureToPtr(value[num], _prvRfid[num], false);
					num++;
				}
			}
		}

		private uint _prvCardNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		private IntPtr[] _prvRfid;
	}
}
