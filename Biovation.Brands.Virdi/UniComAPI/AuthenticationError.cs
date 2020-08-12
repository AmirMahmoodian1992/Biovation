namespace Biovation.Brands.Virdi.UniComAPI
{
	public enum AuthenticationError : uint
	{
		Authorized,
		InvalidUser,
		Matching,
		Permission,
		Capture,
		DuplicatedAuthentication,
		Antipassback,
		Network,
		ServerBusy,
		FaceDetection
	}
}
