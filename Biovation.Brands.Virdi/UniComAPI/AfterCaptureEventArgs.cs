using System;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public class AfterCaptureEventArgs : EventArgs
	{
		public AfterCaptureEventArgs(FingerprintData fingerprint, ApiReturn result)
		{
			Fingerprint = fingerprint;
			Result = result;
		}

		public FingerprintData Fingerprint
		{
			get;
			private set;
		}

		public ApiReturn Result
		{
			get;
			private set;
		}
	}
}
