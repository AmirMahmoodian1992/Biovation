using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct MealConfiguration
	{
		public MealTime Breakfast;

		public MealTime Lunch;

		public MealTime Dinner;

		public MealTime Snack;

		public MealTime Latesnack;

		public ushort LimitOfMonth;

		public byte LimitOfDay;

		private byte _reserved;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string MealName;
	}
}
