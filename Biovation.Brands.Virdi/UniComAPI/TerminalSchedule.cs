using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct TerminalSchedule
	{
		public TerminalDaySchedule Sun;

		public TerminalDaySchedule Mon;

		public TerminalDaySchedule Tue;

		public TerminalDaySchedule Wed;

		public TerminalDaySchedule Thu;

		public TerminalDaySchedule Fri;

		public TerminalDaySchedule Sat;

		public TerminalDaySchedule Holiday1;

		public TerminalDaySchedule Holiday2;

		public TerminalDaySchedule Holiday3;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
		public TerminalHolidayInfo[] Holidays;
	}
}
