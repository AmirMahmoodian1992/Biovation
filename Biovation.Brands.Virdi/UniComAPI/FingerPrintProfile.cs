using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public class FingerPrintProfile : IEnumerable<Fingerprint>, IEnumerable
	{
		public static FingerPrintProfile[] ImportFromVirdiSensor(Stream stream)
		{
			var list = new List<FingerPrintProfile>();
			while (stream.Position < stream.Length - 1L)
			{
				try
				{
					list.Add(new FingerPrintProfile(stream));
				}
				catch
				{
				}
			}
			return list.ToArray();
		}

		public static void ExportToVirdiSensor(Stream stream, FingerPrintProfile[] profiles)
		{
			for (var i = 0; i < profiles.Length; i++)
			{
				var fingerPrintProfile = profiles[i];
				fingerPrintProfile.Write(stream);
			}
		}

		public static void CheckMinutia(byte[] minutia, TemplateFormat templateFormat)
		{
			var flag = minutia == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			var flag2 = minutia.Length != (int)(ushort)FingerprintData.ToTemplateSize(templateFormat);
			if (flag2)
			{
				throw new ArgumentException("Invalid Minutia Data (Length:" + minutia.Length.ToString() + ")");
			}
		}

		public FingerPrintProfile(TemplateSize templateSize)
		{
			TemplateSize = templateSize;
		}

		internal FingerPrintProfile(byte[] data)
		{
			using (var memoryStream = new MemoryStream(data))
			{
				Load(memoryStream);
			}
		}

		public FingerPrintProfile(Stream stream)
		{
			Load(stream);
		}

		private void Load(Stream stream)
		{
			var position = stream.Position;
			var binaryReader = new BinaryReader(stream);
			var num = binaryReader.ReadUInt32();
			var flag = num == 1398140654u || num == 1381363438u;
			if (!flag)
			{
				throw new InvalidDataException("Invalid Profile Header");
			}
			var num2 = binaryReader.ReadUInt32();
			UserID = binaryReader.ReadUInt32();
			var b = binaryReader.ReadByte();
			binaryReader.ReadBytes(2);
			var b2 = binaryReader.ReadByte();
			binaryReader.ReadBytes(4);
			binaryReader.ReadBytes(12);
			_prvFingerprints = new List<Fingerprint>();
			for (var i = 0; i < (int)b; i++)
			{
				_prvFingerprints.Add(new Fingerprint(binaryReader, this));
			}
			var flag2 = stream.Position - position != (long)(ulong)num2;
			if (flag2)
			{
				throw new InvalidDataException("Profile Length Mismatch");
			}
		}

		internal void CheckMinutia(byte[] minutia)
		{
			CheckMinutia(minutia, FingerprintData.ToTemplateFormat(TemplateSize));
		}

		public Fingerprint this[int index]
		{
			get
			{
				return _prvFingerprints[index];
			}
		}

		public TemplateSize TemplateSize
		{
			get;
			set;
		}

		public uint UserID
		{
			get;
			set;
		}

		public byte SamplesPerFinger
		{
			get
			{
				return _prvSamplesPerFinger;
			}
			set
			{
				var flag = _prvFingerprints != null && _prvFingerprints.Count > 0;
				if (flag)
				{
					throw new InvalidOperationException("Clear Fingerprints before changing SamplesPerFinger");
				}
				_prvSamplesPerFinger = value;
			}
		}

		public void AddFingerprint(Fingerprint fingerprint)
		{
			var flag = _prvFingerprints == null;
			if (flag)
			{
				_prvFingerprints = new List<Fingerprint>();
			}
			_prvFingerprints.Add(fingerprint);
		}

		public void RemoveFingerprint(Fingerprint fingerprint)
		{
			var flag = fingerprint.Samples.Length != (int)_prvSamplesPerFinger;
			if (flag)
			{
				throw new InvalidOperationException("Fingerprint Sample Per Finger Mismatch");
			}
			_prvFingerprints.Remove(fingerprint);
		}

		public void ClearFingerprints()
		{
			_prvFingerprints.Clear();
		}

		public int FingerprintCount
		{
			get
			{
				return _prvFingerprints.Count;
			}
		}

		public byte[] GetBytes()
		{
			byte[] result;
			using (var memoryStream = new MemoryStream())
			{
				Write(memoryStream);
				result = memoryStream.ToArray();
			}
			return result;
		}

		public void Write(Stream stream)
		{
			var binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(1398140654u);
			binaryWriter.Write(_prvFingerprints.Count * (int)((ushort)TemplateSize * (ushort)SamplesPerFinger + 24) + 32);
			binaryWriter.Write(UserID);
			binaryWriter.Write((byte)_prvFingerprints.Count);
			binaryWriter.Write(65);
			binaryWriter.Write((byte)(_prvFingerprints.Count * (int)SamplesPerFinger));
			binaryWriter.Write(16844722u);
			binaryWriter.Write(0);
			binaryWriter.Write(0);
			binaryWriter.Write(0);
			foreach (var current in _prvFingerprints)
			{
				current.Write(binaryWriter);
			}
		}

		public Fingerprint CreateFingerprint()
		{
			var flag = _prvSamplesPerFinger == 0;
			if (flag)
			{
				throw new InvalidOperationException("Sample Per Finger can not be 0");
			}
			return new Fingerprint(this);
		}

		IEnumerator<Fingerprint> IEnumerable<Fingerprint>.GetEnumerator()
		{
			return _prvFingerprints.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _prvFingerprints.GetEnumerator();
		}

		private const uint Header1 = 1398140654u;

		private const uint Header2 = 1381363438u;

		private const ushort FixData1 = 65;

		private const uint FixData2 = 16844722u;

		private static readonly byte[] MinutiaeHeader = new byte[]
		{
			85,
			78,
			73,
			79,
			78
		};

		private byte _prvSamplesPerFinger;

		private List<Fingerprint> _prvFingerprints;
	}
}
