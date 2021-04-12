using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
