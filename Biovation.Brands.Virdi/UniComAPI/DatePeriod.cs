using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct DatePeriod
	{
		[MarshalAs(UnmanagedType.Struct)]
		public DateInfo StartDate;

		[MarshalAs(UnmanagedType.Struct)]
		public DateInfo EndDate;
	}
}
