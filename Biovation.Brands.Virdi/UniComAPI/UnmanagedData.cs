using System;
using System.Runtime.InteropServices;

namespace Biovation.Brands.Virdi.UniComAPI
{
    public struct UnmanagedData
    {
        public void SetBytes(byte[] value, UnmanegedMemoryScope scope)
        {
            var flag = value == null || value.Length == 0;
            if (flag)
            {
                _prvLength = 0;
                _prvData = IntPtr.Zero;
            }
            else
            {
                _prvLength = value.Length;
                _prvData = scope.GetIntPtr(value.Length);
                Marshal.Copy(value, 0, _prvData, _prvLength);
            }
        }

        public byte[] GetBytes()
        {
            var array = new byte[_prvLength];
            var flag = _prvLength != 0;
            if (flag)
            {
                Marshal.Copy(_prvData, array, 0, _prvLength);
            }
            return array;
        }

        public string GetString(VirdiEncoding encoding)
        {
            var flag = _prvLength == 0;
            string result;
            if (flag)
            {
                result = "";
            }
            else if (encoding != VirdiEncoding.Ansi)
            {
                if (encoding != VirdiEncoding.Unicode)
                {
                    result = null;
                }
                else
                {
                    result = Marshal.PtrToStringUni(_prvData, _prvLength / 2);
                }
            }
            else
            {
                result = Marshal.PtrToStringAnsi(_prvData, _prvLength);
            }
            return result;
        }

        public void SetString(string str, UnmanegedMemoryScope scope, VirdiEncoding encoding)
        {
            var flag = string.IsNullOrEmpty(str);
            if (flag)
            {
                _prvData = IntPtr.Zero;
                _prvLength = 0;
            }
            else if (encoding != VirdiEncoding.Ansi)
            {
                if (encoding != VirdiEncoding.Unicode)
                {
                    _prvData = IntPtr.Zero;
                    _prvLength = 0;
                }
                else
                {
                    _prvLength = str.Length * 2;
                    _prvData = scope.StringToHGlobalUni(str);
                }
            }
            else
            {
                _prvLength = str.Length;
                _prvData = scope.StringToHGlobalAnsi(str);
            }
        }

        private int _prvLength;

        private IntPtr _prvData;
    }
}
