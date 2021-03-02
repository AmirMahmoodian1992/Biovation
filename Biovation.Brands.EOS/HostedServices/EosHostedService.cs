using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Biovation.Brands.EOS.HostedServices
{
    public class EosHostedService : IHostedService, IDisposable
    {
        /// <summary>
        /// نمونه ی ساخته شده از سرور
        /// </summary>
        private readonly EosServer _eosServer;


        public EosHostedService(EosServer eosServer)
        {
            _eosServer = eosServer;
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _eosServer.StartServer(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _eosServer.StopServer(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
