using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Biovation.Services.RelayController.Services
{
    public class TcpClientGetterService
    {
        public Dictionary<int,TcpClient> RelaysTcpClients { get; set; }

        public TcpClientGetterService(Dictionary<int, TcpClient> relaysTcpClients)
        {
            RelaysTcpClients = relaysTcpClients;
        }

        public TcpClient GeTcpClient(int id)
        {
            try
            {
                if (RelaysTcpClients.ContainsKey(id))
                {
                    var tcpClient = RelaysTcpClients[id];

                    if (IsConnected(tcpClient))
                    {
                        return tcpClient;
                    }
                    else return null;
                }
                else return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool IsConnected(TcpClient tcpClient)
        {
            try
            {
                if (tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected)
                {
                    /* pear to the documentation on Poll:
                    * When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
                    * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                    * -or- true if data is available for reading; 
                    * -or- true if the connection has been closed, reset, or terminated; 
                    * otherwise, returns false
                    */
                    // Detect if client disconnected
                    if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
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
    }
}