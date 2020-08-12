namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct VerifyPasswordData
	{
		public VerifyAuthenticationMode AuthMode
		{
			get
			{
				return (VerifyAuthenticationMode)_prvAuthMode;
			}
			set
			{
				_prvAuthMode = (uint)value;
			}
		}

		public uint UserID;

		private uint _prvAuthMode;

		public UnmanagedData Password;
	}
}
