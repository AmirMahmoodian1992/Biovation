using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biovation.Brands.ZK.HostedServices
{
    public class BroadcastMetricsHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IMetricsRoot _metrics;
        private readonly ILogger<BroadcastMetricsHostedService> _logger;

        public BroadcastMetricsHostedService(IMetricsRoot metrics, ILogger<BroadcastMetricsHostedService> logger)
        {
            _logger = logger;
            _metrics = metrics;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Broadcast Metrics Timed Hosted Service running.");

            _timer = new Timer(ReportMetrics, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void ReportMetrics(object state)
        {
            Task.WaitAll(_metrics.ReportRunner.RunAllAsync().ToArray());
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
