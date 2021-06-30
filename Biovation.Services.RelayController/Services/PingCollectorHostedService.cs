using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Biovation.Services.RelayController.Services
{
    public class PingCollectorHostedService : BackgroundService
    {
        public Ping Pinger; 
        public bool Pingable;

        public PingCollectorHostedService()
        {
            Pinger = new Ping();
            Pingable = false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Pinger = new Ping();
                    var reply = Pinger.Send("192.168.1.200");
                    if (reply != null) Pingable = reply.Status == IPStatus.Success;
                    if (reply != null) Console.WriteLine($"device {reply.Address} status : {reply.Status}");
                }
                catch (PingException)
                {
                    Console.WriteLine("device is not accessible!");
                }
                finally
                {
                    Pinger?.Dispose();
                }
                await Task.Delay(new TimeSpan(0, 0, 5), stoppingToken);
            }
        }
    }
}