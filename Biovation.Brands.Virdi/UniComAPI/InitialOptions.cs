namespace Biovation.Brands.Virdi.UniComAPI
{
	public class InitialOptions
	{
		public SecurityLevel IdentifySecurityLevel
		{
			get;
			set;
		}

		public SecurityLevel VerifySecurityLevel
		{
			get;
			set;
		}

		public TemplateFormat TemlateFormat
		{
			get;
			set;
		}

		internal InitInfo0 InitInfo
		{
			get
			{
				return new InitInfo0
				{
					MaxFingersForEnroll = 10u,
					SamplesPerFinger = 2u,
					DefaultTimeout = 10000u,
					SecurityLevelForEnroll = 5u,
					PrvSecurityLevelForVerify = (uint)VerifySecurityLevel,
					PrvSecurityLevelForIdentify = (uint)IdentifySecurityLevel,
					TemplateFormat = TemlateFormat
				};
			}
			set
			{
				VerifySecurityLevel = (SecurityLevel)value.PrvSecurityLevelForVerify;
				IdentifySecurityLevel = (SecurityLevel)value.PrvSecurityLevelForIdentify;
				TemlateFormat = value.TemplateFormat;
			}
		}
	}
}
