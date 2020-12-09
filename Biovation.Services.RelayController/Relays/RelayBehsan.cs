using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Biovation.Services.RelayController.Common;
using Biovation.Services.RelayController.Models;

namespace Biovation.Services.RelayController.Relays
{
    public class RelayBehsan : IRelay
    {
        public Relay RelayInfo { get; set; }
        public TcpClient TcpClnt;
        public Stream Strm;
        public ASCIIEncoding AsciiEncoding;
        public bool AutoReconnect = true;
        public int ReadBufferSize = 300;

        public RelayBehsan(Relay relayInfo)
        {
            RelayInfo = relayInfo;
        }

        public bool Connect()
        {
            TcpClnt = new TcpClient();
            TcpClnt.Connect(hostname: RelayInfo.Ip, port: RelayInfo.Port);
            Strm = TcpClnt.GetStream();
            AsciiEncoding = new ASCIIEncoding();
            TcpClnt.ReceiveTimeout = 2000;
            return true;
        }

        public bool Disconnect()
        {
            TcpClnt.Close();
            return true;
        }

        public bool IsConnected()
        {
            try
            {
                var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                var tcpConnections = ipProperties.GetActiveTcpConnections();
                return (from tcpConnection in tcpConnections
                    let stateOfConnection = tcpConnection.State
                    where tcpConnection.LocalEndPoint.Equals(TcpClnt.Client.LocalEndPoint) &&
                          tcpConnection.RemoteEndPoint.Equals(TcpClnt.Client.RemoteEndPoint)
                    select stateOfConnection == TcpState.Established).FirstOrDefault();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Contact()
        {
            return _sendData("__C0" + RelayInfo.Id);
        }

        public bool TurnOn()
        {
            return _sendData("__R0" + RelayInfo.Id + "1");
        }

        public bool TurnOff()
        {
            return _sendData("__R0" + RelayInfo.Id + "0");
        }

        public bool FlashOn()
        {
            return _sendData("__F0" + RelayInfo.Id + "1");
        }

        public bool FlashOff()
        {
            return _sendData("__F0" + RelayInfo.Id + "0");
        }

        public string GetData()
        {
            _sendData("%DEVICE");
            return _readData();
        }

        public string GetStatus()
        {
            _sendData("%STATUS");
            var data = _readData();
            var ds = data.Replace(oldValue: "\0", newValue: "").Split(separator: new[] { "\r\n", "\r", "\n" }, options: StringSplitOptions.None);
            return ds[ RelayInfo.Id - 1 ];
        }

        private bool _sendData(string data)
        {
            if (AutoReconnect && !IsConnected())
                Connect();

            var buffer = AsciiEncoding.GetBytes(data);
            Strm.Write(buffer: buffer, offset: 0, count: buffer.Length);
            return true;
        }

        private string _readData()
        {
            var buffer = new byte[ReadBufferSize];
            Strm.Read(buffer: buffer, offset: 0, count: ReadBufferSize);
            return Encoding.UTF8.GetString(bytes: buffer, index: 0, count: buffer.Length);
        }

        ///////////////////////////////////////
        public const string RELAY_1_ON = "__R011";
        public const string RELAY_2_ON = "__R021";
        public const string RELAY_3_ON = "__R031";
        public const string RELAY_4_ON = "__R041";

        public const string RELAY_1_OFF = "__R010";
        public const string RELAY_2_OFF = "__R020";
        public const string RELAY_3_OFF = "__R030";
        public const string RELAY_4_OFF = "__R040";

        public const string FLASHING_1_ON = "__F011";
        public const string FLASHING_2_ON = "__F021";
        public const string FLASHING_3_ON = "__F031";
        public const string FLASHING_4_ON = "__F041";

        public const string FLASHING_1_OFF = "__F010";
        public const string FLASHING_2_OFF = "__F020";
        public const string FLASHING_3_OFF = "__F030";
        public const string FLASHING_4_OFF = "__F040";

        public const string CONTACT_1 = "__C01";
        public const string CONTACT_2 = "__C02";
        public const string CONTACT_3 = "__C03";
        public const string CONTACT_4 = "__C04";

        public const string MAINTAIN_STATUS_1_ON = "__MS011";
        public const string MAINTAIN_STATUS_2_ON = "__MS021";
        public const string MAINTAIN_STATUS_3_ON = "__MS031";
        public const string MAINTAIN_STATUS_4_ON = "__MS041";

        public const string MAINTAIN_STATUS_1_OFF = "__MS010";
        public const string MAINTAIN_STATUS_2_OFF = "__MS020";
        public const string MAINTAIN_STATUS_3_OFF = "__MS030";
    }
}

