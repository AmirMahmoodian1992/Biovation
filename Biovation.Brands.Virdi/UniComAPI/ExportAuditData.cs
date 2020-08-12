using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct ExportAuditData
	{
		public AuditData[] AuditData
		{
			get
			{
				var num = Marshal.SizeOf(typeof(AuditData));
				var array = new AuditData[(int)_prvFingerNum];
				for (var i = 0; i < (int)_prvFingerNum; i++)
				{
					array[i] = (AuditData)Marshal.PtrToStructure(_prvAuditData + i * num, typeof(AuditData));
				}
				return array;
			}
		}

		private uint _prvLength;

		private byte _prvFingerNum;

		public byte SamplesPerFinger;

		public uint ImageWidth;

		public uint ImageHeight;

		private IntPtr _prvAuditData;

		private uint _reserved;
	}
}
