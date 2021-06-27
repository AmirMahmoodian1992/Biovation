namespace Biovation.Brands.Virdi.UniComAPI
{
	public enum AuthenticationType : byte
	{
		OneToNFingerprint,
		OneToOneFingerprint,
		CardAndFingerprint,
		Card,
		Password,
		OneToNFace,
		OneToOneFace
	}
}
