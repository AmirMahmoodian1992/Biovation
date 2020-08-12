using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public class FingerprintData
	{
		[DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_GetTextFIRFromHandle")]
		public static extern ApiReturn GetTextFIRFromHandle(uint hFir, out FirTextEncode pTextFir, bool bIsWide);

		[DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_FreeTextFIR")]
		private static extern ApiReturn FreeTextFIR(ref FirTextEncode textFir);

		[DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_FreeFIRHandle")]
		private static extern ApiReturn FreeFIRHandle(uint firHandle);

		public static TemplateSize ToTemplateSize(TemplateFormat templateFormat)
		{
			TemplateSize result;
			switch (templateFormat)
			{
			case TemplateFormat.Union400:
				result = TemplateSize.Size400;
				break;
			case TemplateFormat.Iso500:
				result = TemplateSize.Size500;
				break;
			case TemplateFormat.Iso600:
				result = TemplateSize.Size600;
				break;
			default:
				throw new InvalidOperationException(string.Format("Invalid TemplateFormat: '{0}'", templateFormat));
			}
			return result;
		}

		public static TemplateFormat ToTemplateFormat(TemplateSize templateSize)
		{
			TemplateFormat result;
			if (templateSize != TemplateSize.Size400)
			{
				if (templateSize != TemplateSize.Size500)
				{
					if (templateSize != TemplateSize.Size600)
					{
						throw new InvalidOperationException(string.Format("Invalid Template Size:{0}", templateSize));
					}
					result = TemplateFormat.Iso600;
				}
				else
				{
					result = TemplateFormat.Iso500;
				}
			}
			else
			{
				result = TemplateFormat.Union400;
			}
			return result;
		}

		internal FingerprintData(uint handle, TemplateFormat templateFormat, ulong uniqueID)
		{
			Encoding = VirdiEncoding.Ansi;
			FirTextEncode firTextEncode;
			var textFirFromHandle = GetTextFIRFromHandle(handle, out firTextEncode, false);
			var flag = textFirFromHandle > ApiReturn.BaseGeneral;
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Can not Convert finger handle to finger text - UError Code:{0} - ErrorName:{1}", (uint)textFirFromHandle, textFirFromHandle.ToString()));
			}
			StringFingerprintData = firTextEncode.TextFir;
			FreeTextFIR(ref firTextEncode);
			FreeFIRHandle(handle);
			TemplateFormat = templateFormat;
		    UniqueID = uniqueID;
		}

		public FingerprintData(string firString, TemplateFormat templateFormat, VirdiEncoding encoding, ulong uniqueID)
		{
			StringFingerprintData = firString;
			Encoding = encoding;
		    UniqueID = uniqueID;
		    TemplateFormat = templateFormat;
		}

		internal InputFir GetInputFir()
		{
			var result = default(InputFir);
			var flag = !string.IsNullOrEmpty(StringFingerprintData);
			if (flag)
			{
				result.TextFir = new FirTextEncode
				{
					IsWideChar = Encoding > VirdiEncoding.Ansi,
					TextFir = StringFingerprintData
				};
				return result;
			}
			throw new InvalidOperationException("Invalid fingerprint data");
		}

		public ulong UniqueID
		{
			get; private set;
		}

	    private string StringFingerprintData { get; }

	    private VirdiEncoding Encoding { get; }

	    public TemplateFormat TemplateFormat { get; }

	    public TemplateSize TemplateSize => ToTemplateSize(TemplateFormat);
	}
}
