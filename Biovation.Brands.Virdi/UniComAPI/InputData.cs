using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct InputData
	{
		public InputDataFinger1To1 Finger1To1
		{
			get
			{
				var flag = InputDataType == InputDataType.Finger1To1;
				if (flag)
				{
					return (InputDataFinger1To1)Marshal.PtrToStructure(_inputData, typeof(InputDataFinger1To1));
				}
				throw new InvalidOperationException("InputDataType Is not Finger1To1");
			}
		}

		public InputDataFinger1ToN Finger1ToN
		{
			get
			{
				var flag = InputDataType == InputDataType.Finger1ToN;
				if (flag)
				{
					return (InputDataFinger1ToN)Marshal.PtrToStructure(_inputData, typeof(InputDataFinger1ToN));
				}
				throw new InvalidOperationException("InputDataType Is not Finger1ToN");
			}
		}

		public InputDataPassword Password
		{
			get
			{
				var flag = InputDataType == InputDataType.Password;
				if (flag)
				{
					return (InputDataPassword)Marshal.PtrToStructure(_inputData, typeof(InputDataPassword));
				}
				throw new InvalidOperationException("InputDataType Is not Password");
			}
		}

		public InputDataCard Card
		{
			get
			{
				var flag = InputDataType == InputDataType.Card;
				if (flag)
				{
					return (InputDataCard)Marshal.PtrToStructure(_inputData, typeof(InputDataCard));
				}
				throw new InvalidOperationException("InputDataType Is not Card");
			}
		}

		public AntiPassbackLevel AntiPassbackLevel;

		public byte DeviceID;

		public InputDataType InputDataType;

		private IntPtr _inputData;
	}
}
