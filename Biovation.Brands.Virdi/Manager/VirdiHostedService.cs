using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Biovation.Brands.Virdi.Manager
{
    public class VirdiHostedService : IHostedService, IDisposable
    {
        /// <summary>
        /// نمونه ی ساخته شده از سرور
        /// </summary>
        private readonly VirdiServer _virdiServer;


        public VirdiHostedService(VirdiServer virdiServer)
        {
            _virdiServer = virdiServer;
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _virdiServer.StartServer();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _virdiServer.StopServer();
            return Task.CompletedTask;
        }
    }
}
