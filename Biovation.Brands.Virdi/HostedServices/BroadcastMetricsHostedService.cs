using App.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.Brands.Virdi.HostedServices
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

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            //var count = Interlocked.Increment(ref _executionCount);
            Task.WaitAll(_metrics.ReportRunner.RunAllAsync().ToArray());

            //_logger.LogInformation(
            //    "Timed Hosted Service is working. Count: {Count}", count);
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
