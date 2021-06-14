using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Biovation.Brands.PW.HostedServices
{
    public class PwHostedService : IHostedService, IDisposable
    {
        /// <summary>
        /// نمونه ی ساخته شده از سرور
        /// </summary>
        private readonly PwServer _pwServer;


        public PwHostedService(PwServer pwServer)
        {
            _pwServer = pwServer;
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _pwServer.StartServer(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _pwServer.StopServer();
            return Task.CompletedTask;
        }
    }
}
