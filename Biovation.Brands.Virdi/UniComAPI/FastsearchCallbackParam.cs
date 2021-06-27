using System;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct FastsearchCallbackParam
	{
		public uint TotalCount;

		public uint MatchedIndex;

		public uint MatchedScore;

		public uint Reserved1;

		public uint Reserved2;

		public uint Reserved3;

		public IntPtr Reserved4;
	}
}
