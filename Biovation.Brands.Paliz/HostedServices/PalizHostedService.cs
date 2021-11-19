using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.Brands.Paliz.HostedServices
{
    public class PalizHostedService : IHostedService, IDisposable
    {
        /// <summary>
        /// نمونه ی ساخته شده از سرور
        /// </summary>
        private readonly PalizServer _palizServer;

        public PalizHostedService(PalizServer palizServer)
        {
            _palizServer = palizServer;
        }

        public void Dispose()
        {
            // pass
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _palizServer.StartServer();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _palizServer.StopServer();
            return Task.CompletedTask;
        }
    }
}
