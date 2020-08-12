using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct InitInfo0
	{
		private uint _structureType;

		public uint MaxFingersForEnroll;

		private uint _necessaryEnrollNum;

		public uint SamplesPerFinger;

		public uint DefaultTimeout;

		[MarshalAs(UnmanagedType.U4)]
		public uint SecurityLevelForEnroll;

		[MarshalAs(UnmanagedType.U4)]
		public uint PrvSecurityLevelForVerify;

		[MarshalAs(UnmanagedType.U4)]
		public uint PrvSecurityLevelForIdentify;

		public TemplateFormat TemplateFormat;

		private uint _reserved1;

		private uint _reserved2;
	}
}
