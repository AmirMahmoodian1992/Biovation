using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Biovation.CommonClasses.Manager
{
    public static class NetworkManager
    {
        public static List<IPAddress> GetLocalIpAddresses()
        {
            var localIps = new List<IPAddress>();
            
            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.Name.ToLower().Contains("vm") || netInterface.Description.ToLower().Contains("virtual") ||
                    netInterface.Name.ToLower().Contains("loopback") || netInterface.Name.ToLower().Contains("bluetooth") ||
                    netInterface.Description.ToLower().Contains("vpn") || netInterface.NetworkInterfaceType == NetworkInterfaceType.Ppp ||
                    netInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback || netInterface.OperationalStatus != OperationalStatus.Up)
                    continue;

                var ipProps = netInterface.GetIPProperties();
                var ipAddresses = ipProps.UnicastAddresses.FirstOrDefault(x => x.Address.AddressFamily == AddressFamily.InterNetwork);

                if (ipAddresses != null)
                    localIps.Add(ipAddresses.Address);
            }

            if (localIps.Count > 0) return localIps;

            var localIpAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            localIps = localIpAddresses.Where(x => x.AddressFamily == AddressFamily.InterNetwork).ToList();

            return localIps;
        }

        public static bool Ping(string inputIpAddress)
        {
            try
            {
                var ipAddress = IPAddress.Parse(inputIpAddress);

                var pingSender = new Ping();
                var options = new PingOptions
                {
                    DontFragment = true
                };

                // Create a buffer of 32 bytes of data to be transmitted. 
                const string data = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var buffer = Encoding.ASCII.GetBytes(data);
                const int timeout = 120;
                var reply = pingSender.Send(ipAddress, timeout, buffer, options);

                return reply != null && reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
