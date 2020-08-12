namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct InitFlag
	{
		public bool PacketCrypto
		{
			get
			{
				return (_prvFlag & 1u) == 1u;
			}
			set
			{
				_prvFlag = value ? _prvFlag | 1u : _prvFlag & 4294967294u;
			}
		}

		public uint Reserved
		{
			get
			{
				return _prvFlag & 4294967294u;
			}
			set
			{
				_prvFlag &= value | 1u;
			}
		}

		private uint _prvFlag;
	}
}
