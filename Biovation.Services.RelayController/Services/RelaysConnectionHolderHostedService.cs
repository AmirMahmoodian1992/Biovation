using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;


namespace Biovation.Services.RelayController.Services
{
    public class RelaysConnectionHolderHostedService : BackgroundService
    {
        public Dictionary<int, TcpClient> RelaysTcpClients { get; set; }

        public RelaysConnectionHolderHostedService(Dictionary<int, TcpClient> relaysTcpClients)
        {
            RelaysTcpClients = relaysTcpClients;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                for (var id = 1; id <= 4; id++)
                {
                    if (RelaysTcpClients.ContainsKey(id))
                    {
                        var tcpClient = RelaysTcpClients[id];

                        if (IsConnected(tcpClient))
                        {
                            Console.WriteLine($"relay number {id} is alive.");
                            continue;
                        }
                        else
                        {
                            Console.WriteLine($"relay number {id} is disconnected!");
                            RelaysTcpClients.Remove(id);
                        }
                    }
                    else
                    {
                        var tcpClient = new TcpClient();
                        try
                        {
                            tcpClient.ConnectAsync("192.168.1.200", 23).Wait(1000);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"relay number {id} not found!");
                            continue;
                        }
                        RelaysTcpClients.Add(id,tcpClient);
                    }

                }
                await Task.Delay(5000, stoppingToken);
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