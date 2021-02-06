using Biovation.Brands.ZK.Devices;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Biovation.Brands.ZK
{
    public class ZkTecoService : IHostedService, IDisposable
    {
        /// <summary>
        /// نمونه ی ساخته شده از سرور
        /// </summary>
        private readonly ZkTecoServer _zkTecoServer;


        public ZkTecoService(ZkTecoServer zkTecoServer)
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
