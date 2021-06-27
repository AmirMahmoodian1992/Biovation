using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Biovation.Brands.Virdi.UniComAPI
{
	[StructLayout(LayoutKind.Sequential)]
	public class WindowCallbackParamOne
	{
		public byte[] GetImage(int width, int height)
		{
			var array = new byte[width * height];
			Marshal.Copy(prvImageBuf, array, 0, array.Length);
			using (var memoryStream = new MemoryStream(20480))
			{
				BitmapSource source = BitmapSource.Create(width, height, 96.0, 96.0, PixelFormats.Gray8, null, array, width);
				new JpegBitmapEncoder
				{
					Frames = 
					{
						BitmapFrame.Create(source)
					}
				}.Save(memoryStream);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public uint Quality;

		private IntPtr prvImageBuf;

		public uint DeviceError;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		private uint[] prvReserved;

		private IntPtr prvReservedPTR;
	}
}
