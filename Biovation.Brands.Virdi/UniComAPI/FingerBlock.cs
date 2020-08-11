using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct FingerBlock
	{
		public TemplateBlock[] GetTemplateInfoes(byte samplePerFinger)
		{
			var array = new TemplateBlock[(int)samplePerFinger];
			var intPtr = _prvTemplateInfo;
			var offset = Marshal.SizeOf(typeof(TemplateBlock));
			for (var i = 0; i < (int)samplePerFinger; i++)
			{
				array[i] = (TemplateBlock)Marshal.PtrToStructure(intPtr, typeof(TemplateBlock));
				intPtr += offset;
			}
			return array;
		}

		public void SetTemplateInfoes(TemplateBlock[] templates, UnmanegedMemoryScope scope)
		{
			_prvLength = (uint)Marshal.SizeOf(typeof(FingerBlock));
			var flag = templates == null || templates.Length == 0;
			if (flag)
			{
				_prvTemplateInfo = IntPtr.Zero;
			}
			else
			{
				var num = templates.Length;
				var num2 = Marshal.SizeOf(typeof(TemplateBlock));
				_prvTemplateInfo = scope.GetIntPtr(num * num2);
				var intPtr = _prvTemplateInfo;
				for (var i = 0; i < num; i++)
				{
					Marshal.StructureToPtr(templates[i], intPtr, false);
					intPtr += num2;
				}
			}
		}

		private uint _prvLength;

		public FingerID FingerID;

		private IntPtr _prvTemplateInfo;
	}
}
