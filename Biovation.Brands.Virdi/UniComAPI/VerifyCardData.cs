namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct VerifyCardData
	{
		public VerifyAuthenticationMode AuthMode
		{
			get
			{
				return (VerifyAuthenticationMode)PrvAuthMode;
			}
			set
			{
				PrvAuthMode = (uint)value;
			}
		}

		public uint PrvAuthMode;

		public UnmanagedData RFID;
	}
}
