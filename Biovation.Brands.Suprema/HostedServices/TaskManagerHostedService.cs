using System;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Brands.Suprema.Manager;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biovation.Brands.Suprema.HostedServices
{
    public class TaskManagerHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private CancellationToken _cancellationToken;
        private readonly TaskManager _taskManager;
        private readonly ILogger<TaskManagerHostedService> _logger;
        public TaskManagerHostedService(ILogger<TaskManagerHostedService> logger, TaskManager taskManager)
        {
            _logger = logger;
            _taskManager = taskManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task Manager Hosted Service running.");
            _cancellationToken = cancellationToken;
            _timer = new Timer(ProcessQueue, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        private void ProcessQueue(object state)
        {
            _taskManager.ProcessQueue(cancellationToken: _cancellationToken).ConfigureAwait(false);
            //using var scope = _services.CreateScope();
            //var pingCollector = new PingCollector(scope.ServiceProvider.GetRequiredService<RestClient>(),
            //    scope.ServiceProvider.GetRequiredService<IMetricsRoot>(),
            //          scope.ServiceProvider.GetRequiredService<ILogger<PingCollector>>(), _localIpAddress);
            //pingCollector.CollectPing();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ping Collector Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
