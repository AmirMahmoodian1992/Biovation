using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct VerifyInputData
	{
		public VerifyCardData CardData
		{
			get
			{
				var flag = _prvDataType != VerifyInputDataType.Card;
				if (flag)
				{
					throw new InvalidOperationException();
				}
				return (VerifyCardData)Marshal.PtrToStructure(_prvData, typeof(VerifyCardData));
			}
		}

		public VerifyPasswordData PasswordData
		{
			get
			{
				var flag = _prvDataType != VerifyInputDataType.Password;
				if (flag)
				{
					throw new InvalidOperationException();
				}
				return (VerifyPasswordData)Marshal.PtrToStructure(_prvData, typeof(VerifyPasswordData));
			}
		}

		public VerifyFingerOneToNData FingerOneToN
		{
			get
			{
				var flag = _prvDataType > VerifyInputDataType.FingerOneToN;
				if (flag)
				{
					throw new InvalidOperationException();
				}
				return (VerifyFingerOneToNData)Marshal.PtrToStructure(_prvData, typeof(VerifyFingerOneToNData));
			}
		}

		public VerifyFingerOneToOneData FingerOneToOne
		{
			get
			{
				var flag = _prvDataType != VerifyInputDataType.FingerOneToOne;
				if (flag)
				{
					throw new InvalidOperationException();
				}
				return (VerifyFingerOneToOneData)Marshal.PtrToStructure(_prvData, typeof(VerifyFingerOneToOneData));
			}
		}

		public VerifyInputDataType InputDataType
		{
			get
			{
				return _prvDataType;
			}
		}

		public bool Door1
		{
			get
			{
				return (Door & 1) == 1;
			}
			set
			{
				if (value)
				{
					Door |= 1;
				}
				else
				{
					Door &= 254;
				}
			}
		}

		public bool Door2
		{
			get
			{
				return (Door & 2) == 2;
			}
			set
			{
				if (value)
				{
					Door |= 2;
				}
				else
				{
					Door &= 253;
				}
			}
		}

		public bool Door3
		{
			get
			{
				return (Door & 4) == 4;
			}
			set
			{
				if (value)
				{
					Door |= 4;
				}
				else
				{
					Door |= 251;
				}
			}
		}

		public bool Door4
		{
			get
			{
				return (Door & 8) == 8;
			}
			set
			{
				if (value)
				{
					Door |= 8;
				}
				else
				{
					Door |= 247;
				}
			}
		}

		[MarshalAs(UnmanagedType.U4)]
		public AntiPassbackLevel AntipassbackLevel;

		[MarshalAs(UnmanagedType.U1)]
		public byte DeviceID;

		[MarshalAs(UnmanagedType.U4)]
		private VerifyInputDataType _prvDataType;

		private IntPtr _prvData;

		public byte ReaderID;

		public byte WiegandID;

		public byte Door;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 45)]
		private byte[] _reserved;
	}
}
