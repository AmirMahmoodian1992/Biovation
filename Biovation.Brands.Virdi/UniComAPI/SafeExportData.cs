using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public class SafeExportData : IDisposable
	{
		[DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_FreeExportData")]
		private static extern ApiReturn FreeExportData(ref ExportData pExportData);

		internal SafeExportData(ExportData exportData)
		{
			_prvExportData = exportData;
		}

		public ExportData ExportData
		{
			get
			{
				return _prvExportData;
			}
		}

		public void Dispose()
		{
			var flag = Interlocked.CompareExchange(ref _prvIsDisposed, 1, 0) == 0;
			if (flag)
			{
				FreeExportData(ref _prvExportData);
			}
		}

		~SafeExportData()
		{
			Dispose();
		}

		private ExportData _prvExportData;

		private int _prvIsDisposed;
	}
}
