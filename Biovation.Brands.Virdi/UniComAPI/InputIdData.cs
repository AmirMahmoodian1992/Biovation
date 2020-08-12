using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
    public struct InputIdData
    {
        public byte DeviceID => _prvDeviceID;

        public InputIdType IdType => _prvDataType;

        public uint UserID
        {
            get
            {
                var flag = _prvDataType > InputIdType.UserID;
                if (flag)
                {
                    throw new InvalidOperationException();
                }
                return (uint)Marshal.ReadInt32(_prvData);
            }
        }

        public string UniqueID
        {
            get
            {
                var flag = _prvDataType != InputIdType.UniqueID;
                if (flag)
                {
                    throw new InvalidOperationException();
                }
                return ((UnmanagedData)Marshal.PtrToStructure(_prvData, typeof(UnmanagedData))).GetString(VirdiEncoding.Ansi);
            }
        }

        public string Rfid
        {
            get
            {
                var flag = _prvDataType != InputIdType.Rfid;
                if (flag)
                {
                    throw new InvalidOperationException();
                }
                return ((UnmanagedData)Marshal.PtrToStructure(_prvData, typeof(UnmanagedData))).GetString(VirdiEncoding.Ansi);
            }
        }

        public bool Door1
        {
            get => (Door & 1) == 1;
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
            get => (Door & 2) == 2;
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
            get => (Door & 4) == 4;
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
            get => (Door & 8) == 8;
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

        private readonly byte _prvDeviceID;

        private readonly InputIdType _prvDataType;

        private readonly IntPtr _prvData;

        public byte ReaderID;

        public byte WiegandID;

        public byte Door;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 45)]
        private readonly byte[] _reserved;
    }
}
