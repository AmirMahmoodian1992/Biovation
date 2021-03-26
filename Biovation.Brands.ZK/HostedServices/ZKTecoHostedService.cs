using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Biovation.Brands.ZK.HostedServices
{
    public class ZKTecoHostedService : IHostedService, IDisposable
    {
        /// <summary>
        /// نمونه ی ساخته شده از سرور
        /// </summary>
        private readonly ZkTecoServer _zkTecoServer;


        public ZKTecoHostedService(ZkTecoServer zkTecoServer)
        {
            _zkTecoServer = zkTecoServer;
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _zkTecoServer.StartServer(cancellationToken);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _zkTecoServer.StopServer(cancellationToken);
        }
    }
}
