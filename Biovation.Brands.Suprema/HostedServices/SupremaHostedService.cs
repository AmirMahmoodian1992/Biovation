using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Biovation.Brands.Suprema.HostedServices
{
    public class SupremaHostedService : IHostedService, IDisposable
    {
        /// <summary>
        /// نمونه ی ساخته شده از سرور
        /// </summary>
        private readonly BioStarServer _supremaServer;


        public SupremaHostedService(BioStarServer supremaServer)
        {
            _supremaServer = supremaServer;
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _supremaServer.StartService(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _supremaServer.StopService();
            return Task.CompletedTask;
        }
    }
}
