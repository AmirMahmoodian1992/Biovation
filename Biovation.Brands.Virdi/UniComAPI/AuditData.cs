using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AuditData
	{
		public byte[][] GetImageData(int samplePerFinger, int imageWidth, int imageHeight)
		{
			var num = Marshal.SizeOf(typeof(ImageData));
			var array = new byte[samplePerFinger][];
			using (var memoryStream = new MemoryStream(20480))
			{
				for (var i = 0; i < samplePerFinger; i++)
				{
					memoryStream.Position = 0L;
					memoryStream.SetLength(0L);
					var imageData = (ImageData)Marshal.PtrToStructure(_prvImageData + i * num, typeof(ImageData));
					BitmapSource source = BitmapSource.Create(imageWidth, imageHeight, 96.0, 96.0, PixelFormats.Gray8, null, imageData.GetImage(imageWidth * imageHeight), imageWidth);
					new JpegBitmapEncoder
					{
						Frames = 
						{
							BitmapFrame.Create(source)
						}
					}.Save(memoryStream);
					array[i] = memoryStream.ToArray();
				}
			}
			return array;
		}

		private uint _length;

		public FingerID FingerID;

		private IntPtr _prvImageData;
	}
}
