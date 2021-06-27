namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct FirHeader
	{
		public uint Length;

		public uint DataLength;

		public ushort Version;

		public ushort DataType;

		public FirPurpose Purpose;

		public ushort Quality;

		public FirOptionalData OptionalData;

		public uint Reserved;
	}
}
