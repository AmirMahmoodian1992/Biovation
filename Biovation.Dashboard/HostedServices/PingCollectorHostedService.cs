using Biovation.CommonClasses.Manager;
using Biovation.Dashboard.Managers;
using Biovation.Dashboard.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.Dashboard.HostedServices
{
    public class PingCollectorHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private string _localIpAddress;
        private readonly IServiceProvider _services;
        private readonly ILogger<PingCollectorHostedService> _logger;
        public PingCollectorHostedService(ILogger<PingCollectorHostedService> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Log Collector Hosted Service running.");

            _localIpAddress = NetworkManager.GetLocalIpAddresses().FirstOrDefault()?.ToString();
            _timer = new Timer(CollectPing, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void CollectPing(object state)
        {
            using var scope = _services.CreateScope();
            //var pingCollector = scope.ServiceProvider.GetRequiredService<PingCollector>();
            var pingCollector = new PingCollector(scope.ServiceProvider.GetRequiredService<RestClient>(),
                scope.ServiceProvider.GetRequiredService<PingRepository>(),
                      scope.ServiceProvider.GetRequiredService<ILogger<PingCollector>>(), _localIpAddress);
            pingCollector.CollectPing();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
