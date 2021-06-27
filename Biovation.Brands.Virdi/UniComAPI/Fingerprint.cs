using System;
using System.IO;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public class Fingerprint
	{
		internal Fingerprint(BinaryReader reader, FingerPrintProfile profile)
		{
			_prvProfile = profile;
			var flag = reader.ReadUInt16() == 20550;
			if (!flag)
			{
				throw new InvalidDataException("fingerprint data is corrupt (Finger Block)");
			}
			var b = reader.ReadByte();
			var flag2 = profile.SamplesPerFinger == 0;
			if (flag2)
			{
				profile.SamplesPerFinger = b;
			}
			FingerID = (FingerID)reader.ReadByte();
			var num = reader.ReadInt32();
			var flag3 = profile.UserID != (ulong)num;
			if (flag3)
			{
				throw new InvalidOperationException("UserID Mismatch");
			}
			var num2 = reader.ReadUInt16();
			var flag4 = profile.TemplateSize == 0u;
			if (flag4)
			{
				profile.TemplateSize = (TemplateSize)num2;
			}
			var flag5 = reader.ReadInt16() == (short)num2;
			if (!flag5)
			{
				throw new InvalidOperationException("Template Size Mismatch");
			}
			var flag6 = reader.ReadUInt32() != 65u;
			if (flag6)
			{
				throw new InvalidOperationException("Invalid Fix Data 3");
			}
			reader.ReadBytes(8);
			Samples = new byte[b][];
			for (var i = 0; i < b; i++)
			{
				Samples[i] = reader.ReadBytes(num2);
			}
		}

		internal Fingerprint(FingerPrintProfile profile)
		{
			_prvProfile = profile;
			Samples = new byte[profile.SamplesPerFinger][];
		}

	    public byte[][] Samples { get; }


		public FingerID FingerID
		{
			get;
			set;
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write(20550);
			writer.Write(_prvProfile.SamplesPerFinger);
			writer.Write((byte)FingerID);
			writer.Write(_prvProfile.UserID);
			writer.Write((ushort)_prvProfile.TemplateSize);
			writer.Write((ushort)_prvProfile.TemplateSize);
			writer.Write(65);
			writer.Write(0);
			writer.Write(0);
			var samples = Samples;
			for (var i = 0; i < samples.Length; i++)
			{
				var array = samples[i];
				_prvProfile.CheckMinutia(array);
				writer.Write(array);
			}
		}

		private const ushort Header = 20550;

		private const uint FixData3 = 65u;

		private FingerPrintProfile _prvProfile;
	}
}
