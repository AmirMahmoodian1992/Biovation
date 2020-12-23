using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Biovation.Domain.RelayControllerModels;
using Biovation.Services.RelayController.Common;

namespace Biovation.Services.RelayController.Relays
{
    public class BehsanRelay : IRelay
    {
        public Relay RelayInfo { get; set; }
        private readonly TcpClient _tcpClient;
        private Stream _stream;
        private readonly ASCIIEncoding _asciiEncoding;
        private readonly bool _autoReconnect = true;
        private readonly int _readBufferSize = 300;

        public BehsanRelay(Relay relayInfo, TcpClient tcpClient)
        {
            RelayInfo = relayInfo;
            _tcpClient = tcpClient;
            _asciiEncoding = new ASCIIEncoding();
            _stream = _tcpClient.GetStream();
        }

        public bool Connect()
        {
            try
            {
                _tcpClient.ConnectAsync(RelayInfo.Hub.IpAddress, RelayInfo.Hub.Port).Wait(2000);
                //_stream = _tcpClient.GetStream();
                _tcpClient.ReceiveTimeout = 2000;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                _tcpClient.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsConnected()
        {
            try
            {
                if (_tcpClient != null && _tcpClient.Client != null && _tcpClient.Client.Connected)
                {
                    /* pear to the documentation on Poll:
                    * When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
                    * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                    * -or- true if data is available for reading; 
                    * -or- true if the connection has been closed, reset, or terminated; 
                    * otherwise, returns false
                    */
                    // Detect if client disconnected
                    if (_tcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (_tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            // Client disconnected
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool Contact()
        {
            try
            {
                return SendData("__C0" + RelayInfo.Id);
            }
            catch (Exception )
            {
                return false;
            }
        }

        public bool TurnOn()
        {
            try
            {
                return SendData("__R0" + RelayInfo.Id + "1");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TurnOff()
        {
            try
            {
                return SendData("__R0" + RelayInfo.Id + "0");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool FlashOn()
        {
            try
            {
                return SendData("__F0" + RelayInfo.Id + "1");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool FlashOff()
        {
            try
            {
                return SendData("__F0" + RelayInfo.Id + "0");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetData()
        {
            try
            {
                SendData("%DEVICE");
                return ReadData();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public string GetStatus()
        {
            try
            {
                SendData("%STATUS");
                var data = ReadData();
                var ds = data.Replace(oldValue: "\0", newValue: "").Split(separator: new[] { "\r\n", "\r", "\n" }, options: StringSplitOptions.None);
                return ds[RelayInfo.Id - 1];
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        private bool SendData(string data)
        {
            if (_autoReconnect && !IsConnected())
                Connect();

            try
            {
                var buffer = _asciiEncoding.GetBytes(data);
                _stream.Write(buffer: buffer, offset: 0, count: buffer.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string ReadData()
        {
            try
            {
                var buffer = new byte[_readBufferSize];
                _stream.Read(buffer: buffer, offset: 0, count: _readBufferSize);
                return Encoding.UTF8.GetString(bytes: buffer, index: 0, count: buffer.Length);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
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

