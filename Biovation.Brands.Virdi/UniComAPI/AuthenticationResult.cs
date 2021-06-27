using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public struct AuthenticationResult
	{
		public bool Face
		{
			get
			{
				return (_propertyEx & 1) == 1;
			}
			set
			{
				if (value)
				{
					_propertyEx |= 1;
				}
				else
				{
					_propertyEx &= 254;
				}
			}
		}

		public bool MobileKey
		{
			get
			{
				return (_propertyEx & 2) == 2;
			}
			set
			{
				if (value)
				{
					_propertyEx |= 2;
				}
				else
				{
					_propertyEx &= 253;
				}
			}
		}

		public bool Door1
		{
			get
			{
				return (_passedDoor & 1) == 1;
			}
			set
			{
				if (value)
				{
					_passedDoor |= 1;
				}
				else
				{
					_passedDoor &= 254;
				}
			}
		}

		public bool Door2
		{
			get
			{
				return (_passedDoor & 2) == 2;
			}
			set
			{
				if (value)
				{
					_passedDoor |= 2;
				}
				else
				{
					_passedDoor &= 253;
				}
			}
		}

		public bool Door3
		{
			get
			{
				return (_passedDoor & 4) == 4;
			}
			set
			{
				if (value)
				{
					_passedDoor |= 4;
				}
				else
				{
					_passedDoor |= 251;
				}
			}
		}

		public bool Door4
		{
			get
			{
				return (_passedDoor & 8) == 8;
			}
			set
			{
				if (value)
				{
					_passedDoor |= 8;
				}
				else
				{
					_passedDoor |= 247;
				}
			}
		}

		public uint UserID;

		public UserProperty Property;

		public bool IsAuthorized;

		public bool IsVisitor;

		public DateTimeInfo AuthorizedTime;

		public VirdiError ErrorCode;

		private byte _propertyEx;

		private byte _passedDoor;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 46)]
		private byte[] _reserved;
	}
}
