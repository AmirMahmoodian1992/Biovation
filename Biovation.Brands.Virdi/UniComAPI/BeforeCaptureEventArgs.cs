using System;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public class BeforeCaptureEventArgs : EventArgs
	{
		public BeforeCaptureEventArgs(byte quality, byte[] image, uint deviceError)
		{
			Quality = quality;
			Image = image;
			DeviceError = deviceError;
		}

		public byte Quality
		{
			get;
			private set;
		}

		public byte[] Image
		{
			get;
			private set;
		}

		public uint DeviceError
		{
			get;
			private set;
		}

		public bool Cancel
		{
			get;
			set;
		}
	}
}
