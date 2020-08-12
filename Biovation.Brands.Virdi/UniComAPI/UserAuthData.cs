using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct UserAuthData
	{
		public UnmanagedData? GetPassword()
		{
			var flag = _prvPassword != IntPtr.Zero;
			UnmanagedData? result;
			if (flag)
			{
				result = new UnmanagedData?((UnmanagedData)Marshal.PtrToStructure(_prvPassword, typeof(UnmanagedData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetPassword(UnmanagedData value, UnmanegedMemoryScope scope)
		{
			_prvPassword = scope.GetIntPtr(Marshal.SizeOf(typeof(UnmanagedData)));
			Marshal.StructureToPtr(value, _prvPassword, false);
		}

		public CardData? GetCardIDs()
		{
			var flag = _prvCardData != IntPtr.Zero;
			CardData? result;
			if (flag)
			{
				result = new CardData?((CardData)Marshal.PtrToStructure(_prvCardData, typeof(CardData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetCardIDs(CardData value, UnmanegedMemoryScope scope)
		{
			_prvCardData = scope.GetIntPtr(Marshal.SizeOf(typeof(CardData)));
			Marshal.StructureToPtr(value, _prvCardData, false);
		}

		public FingerData? FingerData
		{
			get
			{
				var flag = _prvFinger != IntPtr.Zero;
				FingerData? result;
				if (flag)
				{
					result = new FingerData?((FingerData)Marshal.PtrToStructure(_prvFinger, typeof(FingerData)));
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				_prvFinger = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FingerData)));
				Marshal.StructureToPtr(value, _prvFinger, false);
			}
		}

		public FingerData? GetFingerData()
		{
			var flag = _prvFinger != IntPtr.Zero;
			FingerData? result;
			if (flag)
			{
				result = new FingerData?((FingerData)Marshal.PtrToStructure(_prvFinger, typeof(FingerData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetFingerData(FingerData value, UnmanegedMemoryScope scope)
		{
			_prvFinger = scope.GetIntPtr(Marshal.SizeOf(typeof(FingerData)));
			Marshal.StructureToPtr(value, _prvFinger, false);
		}

		public FaceData? GetFace()
		{
			var flag = _prvFace != IntPtr.Zero;
			FaceData? result;
			if (flag)
			{
				result = new FaceData?((FaceData)Marshal.PtrToStructure(_prvFace, typeof(FaceData)));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetFace(FaceData value, UnmanegedMemoryScope scope)
		{
			_prvFace = scope.GetIntPtr(Marshal.SizeOf(typeof(FaceData)));
			Marshal.StructureToPtr(value, _prvFace, false);
		}

		private IntPtr _prvPassword;

		private IntPtr _prvCardData;

		private IntPtr _prvFinger;

		private IntPtr _prvFace;
	}
}
