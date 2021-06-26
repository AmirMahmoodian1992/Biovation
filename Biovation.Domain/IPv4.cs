using System;
using System.Net;
using System.Net.Sockets;

namespace Biovation.Domain
{
    public struct IPv4 : IEquatable<IPv4>, IComparable, IComparable<IPv4>, IFormattable
    {
        // Token: 0x060000D5 RID: 213 RVA: 0x00006AB8 File Offset: 0x00004CB8
        public static bool operator ==(IPv4 ip1, IPv4 ip2)
        {
            return ip1.Equals(ip2);
        }

        // Token: 0x060000D6 RID: 214 RVA: 0x00006AE0 File Offset: 0x00004CE0
        public static bool operator !=(IPv4 ip1, IPv4 ip2)
        {
            return !ip1.Equals(ip2);
        }

        // Token: 0x060000D7 RID: 215 RVA: 0x00006B08 File Offset: 0x00004D08
        public static bool operator >=(IPv4 ip1, IPv4 ip2)
        {
            return ip1.CompareTo(ip2) >= 0;
        }

        // Token: 0x060000D8 RID: 216 RVA: 0x00006B28 File Offset: 0x00004D28
        public static bool operator >(IPv4 ip1, IPv4 ip2)
        {
            return ip1.CompareTo(ip2) > 0;
        }

        // Token: 0x060000D9 RID: 217 RVA: 0x00006B48 File Offset: 0x00004D48
        public static bool operator <=(IPv4 ip1, IPv4 ip2)
        {
            return ip1.CompareTo(ip2) <= 0;
        }

        // Token: 0x060000DA RID: 218 RVA: 0x00006B68 File Offset: 0x00004D68
        public static bool operator <(IPv4 ip1, IPv4 ip2)
        {
            return ip1.CompareTo(ip2) < 0;
        }

        // Token: 0x060000DB RID: 219 RVA: 0x00006B88 File Offset: 0x00004D88
        public static IPv4 operator |(IPv4 ip1, IPv4 ip2)
        {
            return new IPv4(ip1._prvB1 | ip2._prvB1, ip1._prvB2 | ip2._prvB2, ip1._prvB3 | ip2._prvB3, ip1._prvB4 | ip2._prvB4);
        }

        // Token: 0x060000DC RID: 220 RVA: 0x00006BD8 File Offset: 0x00004DD8
        public static IPv4 operator &(IPv4 ip1, IPv4 ip2)
        {
            return new IPv4(ip1._prvB1 & ip2._prvB1, ip1._prvB2 & ip2._prvB2, ip1._prvB3 & ip2._prvB3, ip1._prvB4 & ip2._prvB4);
        }

        // Token: 0x060000DD RID: 221 RVA: 0x00006C28 File Offset: 0x00004E28
        public static IPv4 operator ~(IPv4 ip)
        {
            return new IPv4(~ip._prvB1, ~ip._prvB2, ~ip._prvB3, ~ip._prvB4);
        }

        // Token: 0x060000DE RID: 222 RVA: 0x00006C60 File Offset: 0x00004E60
        public static explicit operator IPAddress(IPv4 ip)
        {
            return new IPAddress(ip.Binary);
        }

        // Token: 0x060000DF RID: 223 RVA: 0x00006C80 File Offset: 0x00004E80
        public static explicit operator IPv4(IPAddress ip)
        {
            var flag = ip.AddressFamily != AddressFamily.InterNetwork;
            if (flag)
            {
                throw new InvalidCastException();
            }
            return new IPv4(ip.GetAddressBytes(), false);
        }

        // Token: 0x060000E0 RID: 224 RVA: 0x00006CB8 File Offset: 0x00004EB8
        public static bool IsValidIPv4Address(string ipAddress)
        {
            var flag = TryParse(ipAddress, out var pv);
            var flag2 = flag;
            if (flag2)
            {
                flag = pv.IsValidForNodeAssignment;
            }
            return flag;
        }

        // Token: 0x060000E1 RID: 225 RVA: 0x00006CE4 File Offset: 0x00004EE4
        public static IPv4 Parse(string s)
        {
            var flag = s == null;
            if (flag)
            {
                throw new ArgumentNullException("s");
            }
            var array = s.Split('.');
            var flag2 = array.Length < 4;
            if (flag2)
            {
                throw new FormatException();
            }
            byte b;
            byte b2;
            byte b3;
            byte b4;
            try
            {
                b = byte.Parse(array[0]);
                b2 = byte.Parse(array[1]);
                b3 = byte.Parse(array[2]);
                b4 = byte.Parse(array[3]);
            }
            catch (Exception innerException)
            {
                throw new FormatException("Input string is not in a correct format", innerException);
            }
            return new IPv4(b, b2, b3, b4);
        }

        // Token: 0x060000E2 RID: 226 RVA: 0x00006D88 File Offset: 0x00004F88
        public static bool TryParse(string s, out IPv4 ip)
        {
            //bool result;
            try
            {
                ip = Parse(s);
            }
            catch
            {
                ip = ThisNode;
                //result = false;
                return false;
            }
            //result = true;
            return true;
        }

        // Token: 0x060000E3 RID: 227 RVA: 0x00006DCC File Offset: 0x00004FCC
        public static string Reformat(string s)
        {
            return Parse(s).ToString();
        }

        // Token: 0x060000E4 RID: 228 RVA: 0x00006DF2 File Offset: 0x00004FF2
        public IPv4(int b1, int b2, int b3, int b4)
        {
            _prvB1 = (byte)b1;
            _prvB2 = (byte)b2;
            _prvB3 = (byte)b3;
            _prvB4 = (byte)b4;
        }

        // Token: 0x060000E5 RID: 229 RVA: 0x00006E14 File Offset: 0x00005014
        public IPv4(byte[] data, bool reverseOrder)
        {
            if (reverseOrder)
            {
                _prvB1 = data[3];
                _prvB2 = data[2];
                _prvB3 = data[1];
                _prvB4 = data[0];
            }
            else
            {
                _prvB1 = data[0];
                _prvB2 = data[1];
                _prvB3 = data[2];
                _prvB4 = data[3];
            }
        }

        // Token: 0x060000E6 RID: 230 RVA: 0x00006E78 File Offset: 0x00005078
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // Token: 0x060000E7 RID: 231 RVA: 0x00006E9C File Offset: 0x0000509C
        public override bool Equals(object obj)
        {
            var flag = obj is IPv4;
            bool result;
            if (flag)
            {
                var pv = (IPv4)obj;
                result = pv._prvB1 == _prvB1 && pv._prvB2 == _prvB2 && pv._prvB3 == _prvB3 && pv._prvB4 == _prvB4;
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x060000E8 RID: 232 RVA: 0x00006F04 File Offset: 0x00005104
        public override string ToString()
        {
            return string.Concat(_prvB1.ToString(), ".", _prvB2.ToString(), ".", _prvB3.ToString(), ".", _prvB4.ToString());
        }

        // Token: 0x060000E9 RID: 233 RVA: 0x00006F74 File Offset: 0x00005174
        public string ToString(string formatting)
        {
            var flag = formatting.ToLower() == "f";
            string result;
            if (flag)
            {
                result = string.Concat(_prvB1.ToString("D3"), ".", _prvB2.ToString("D3"), ".", _prvB3.ToString("D3"), ".", _prvB4.ToString("D3"));
            }
            else
            {
                result = ToString();
            }
            return result;
        }

        // Token: 0x060000EA RID: 234 RVA: 0x0000701C File Offset: 0x0000521C
        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            ICustomFormatter customFormatter = null;
            var flag = formatProvider != null;
            if (flag)
            {
                customFormatter = formatProvider.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;
            }
            var flag2 = customFormatter != null;
            string result;
            if (flag2)
            {
                result = customFormatter.Format(format, this, null);
            }
            else
            {
                result = ToString(format);
            }
            return result;
        }

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x060000EB RID: 235 RVA: 0x00007078 File Offset: 0x00005278
        public byte[] Binary => new[]
                {
                    _prvB1,
                    _prvB2,
                    _prvB3,
                    _prvB4
                };

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x060000EC RID: 236 RVA: 0x000070B4 File Offset: 0x000052B4
        public byte[] BinaryReverse => new[]
                {
                    _prvB4,
                    _prvB3,
                    _prvB2,
                    _prvB1
                };

        // Token: 0x1700001A RID: 26
        // (get) Token: 0x060000ED RID: 237 RVA: 0x000070F0 File Offset: 0x000052F0
        public byte B1 => _prvB1;

        // Token: 0x1700001B RID: 27
        // (get) Token: 0x060000EE RID: 238 RVA: 0x00007108 File Offset: 0x00005308
        public byte B2 => _prvB2;

        // Token: 0x1700001C RID: 28
        // (get) Token: 0x060000EF RID: 239 RVA: 0x00007120 File Offset: 0x00005320
        public byte B3 => _prvB3;

        // Token: 0x1700001D RID: 29
        // (get) Token: 0x060000F0 RID: 240 RVA: 0x00007138 File Offset: 0x00005338
        public byte B4 => _prvB4;

        // Token: 0x1700001E RID: 30
        // (get) Token: 0x060000F1 RID: 241 RVA: 0x00007150 File Offset: 0x00005350
        //public IpV4Class Class
        //{
        //    get
        //    {
        //        bool flag = this.prvB1 < 128;
        //        IpV4Class result;
        //        if (flag)
        //        {
        //            result = IpV4Class.A;
        //        }
        //        else
        //        {
        //            bool flag2 = this.prvB1 > 127 && this.prvB1 < 192;
        //            if (flag2)
        //            {
        //                result = IpV4Class.B;
        //            }
        //            else
        //            {
        //                bool flag3 = this.prvB1 > 191 && this.prvB1 < 224;
        //                if (flag3)
        //                {
        //                    result = IpV4Class.C;
        //                }
        //                else
        //                {
        //                    bool flag4 = this.prvB1 > 223 && this.prvB1 < 240;
        //                    if (flag4)
        //                    {
        //                        result = IpV4Class.D;
        //                    }
        //                    else
        //                    {
        //                        result = IpV4Class.E;
        //                    }
        //                }
        //            }
        //        }
        //        return result;
        //    }
        //}

        // Token: 0x1700001F RID: 31
        // (get) Token: 0x060000F2 RID: 242 RVA: 0x000071EC File Offset: 0x000053EC
        public bool IsLinkLocal => this <= LinkLocalMax && this >= LinkLocalMin;

        // Token: 0x17000020 RID: 32
        // (get) Token: 0x060000F3 RID: 243 RVA: 0x00007224 File Offset: 0x00005424
        public bool IsPrivate => this <= ClassAPrivateMax && this >= ClassAPrivateMin || this <= ClassBPrivateMax && this >= ClassBPrivateMin || this <= ClassCPrivateMax && this >= ClassCPrivateMin;

        // Token: 0x17000021 RID: 33
        // (get) Token: 0x060000F4 RID: 244 RVA: 0x000072A8 File Offset: 0x000054A8
        public bool IsValidForNodeAssignment => _prvB1 < 224 && (this > LoopBackMax || this < LoopBackMin) && !Equals(ClassAMin) && !Equals(ClassAMax) && !Equals(ClassBMin) && !Equals(ClassBMax) && !Equals(ClassCMin) && !Equals(ClassCMax);

        // Token: 0x17000022 RID: 34
        // (get) Token: 0x060000F5 RID: 245 RVA: 0x00007388 File Offset: 0x00005588
        public bool IsMulticast => _prvB1 > 223 && _prvB1 < 240;

        // Token: 0x17000023 RID: 35
        // (get) Token: 0x060000F6 RID: 246 RVA: 0x000073B8 File Offset: 0x000055B8
        public bool IsReserved => _prvB1 > 239;

        // Token: 0x060000F7 RID: 247 RVA: 0x000073D8 File Offset: 0x000055D8
        bool IEquatable<IPv4>.Equals(IPv4 other)
        {
            return Equals(other);
        }

        // Token: 0x060000F8 RID: 248 RVA: 0x000073FC File Offset: 0x000055FC
        public uint ToUInt32()
        {
            return ((_prvB1 * 256u + _prvB2) * 256u + _prvB3) * 256u + _prvB4;
        }

        // Token: 0x060000F9 RID: 249 RVA: 0x0000743C File Offset: 0x0000563C
        public int CompareTo(object obj)
        {
            var flag = obj != null && obj is IPv4;
            int result;
            if (flag)
            {
                result = ToUInt32().CompareTo(((IPv4)obj).ToUInt32());
            }
            else
            {
                result = 2147483647;
            }
            return result;
        }

        // Token: 0x060000FA RID: 250 RVA: 0x00007488 File Offset: 0x00005688
        public int CompareTo(IPv4 other)
        {
            return ToUInt32().CompareTo(other.ToUInt32());
        }

        // Token: 0x04000043 RID: 67
        public static readonly IPv4 ThisNode = default(IPv4);

        // Token: 0x04000044 RID: 68
        public static readonly IPv4 Broadcast = new IPv4(255, 255, 255, 255);

        // Token: 0x04000045 RID: 69
        public static readonly IPv4 LoopBackMin = new IPv4(127, 0, 0, 0);

        // Token: 0x04000046 RID: 70
        public static readonly IPv4 LoopBackMax = new IPv4(127, 255, 255, 255);

        // Token: 0x04000047 RID: 71
        public static readonly IPv4 LinkLocalMin = new IPv4(169, 254, 0, 0);

        // Token: 0x04000048 RID: 72
        public static readonly IPv4 LinkLocalMax = new IPv4(169, 254, 255, 255);

        // Token: 0x04000049 RID: 73
        public static readonly IPv4 ClassAMin = new IPv4(0, 0, 0, 0);

        // Token: 0x0400004A RID: 74
        public static readonly IPv4 ClassAMax = new IPv4(127, 255, 255, 255);

        // Token: 0x0400004B RID: 75
        public static readonly IPv4 ClassBMin = new IPv4(128, 0, 0, 0);

        // Token: 0x0400004C RID: 76
        public static readonly IPv4 ClassBMax = new IPv4(191, 255, 255, 255);

        // Token: 0x0400004D RID: 77
        public static readonly IPv4 ClassCMin = new IPv4(192, 0, 0, 0);

        // Token: 0x0400004E RID: 78
        public static readonly IPv4 ClassCMax = new IPv4(223, 255, 255, 255);

        // Token: 0x0400004F RID: 79
        public static readonly IPv4 ClassDMin = new IPv4(224, 0, 0, 0);

        // Token: 0x04000050 RID: 80
        public static readonly IPv4 ClassDMax = new IPv4(239, 255, 255, 255);

        // Token: 0x04000051 RID: 81
        public static readonly IPv4 ClassEMin = new IPv4(240, 0, 0, 0);

        // Token: 0x04000052 RID: 82
        public static readonly IPv4 ClassEMax = new IPv4(255, 255, 255, 255);

        // Token: 0x04000053 RID: 83
        public static readonly IPv4 ClassAPrivateMin = new IPv4(10, 0, 0, 0);

        // Token: 0x04000054 RID: 84
        public static readonly IPv4 ClassAPrivateMax = new IPv4(10, 255, 255, 255);

        // Token: 0x04000055 RID: 85
        public static readonly IPv4 ClassBPrivateMin = new IPv4(172, 16, 0, 0);

        // Token: 0x04000056 RID: 86
        public static readonly IPv4 ClassBPrivateMax = new IPv4(172, 31, 255, 255);

        // Token: 0x04000057 RID: 87
        public static readonly IPv4 ClassCPrivateMin = new IPv4(192, 168, 0, 0);

        // Token: 0x04000058 RID: 88
        public static readonly IPv4 ClassCPrivateMax = new IPv4(192, 168, 255, 255);

        // Token: 0x04000059 RID: 89
        private readonly byte _prvB1;

        // Token: 0x0400005A RID: 90
        private readonly byte _prvB2;

        // Token: 0x0400005B RID: 91
        private readonly byte _prvB3;

        // Token: 0x0400005C RID: 92
        private readonly byte _prvB4;
    }
}
