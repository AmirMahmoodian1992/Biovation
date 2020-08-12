namespace Biovation.Brands.Virdi.UniComAPI
{
	public enum FirPurpose : ushort
	{
		Verify = 1,
		Identify,
		Enroll,
		EnrollForVerificationOnly,
		EnrollForIdentificationOnly,
		Audit,
		Update = 16
	}
}
