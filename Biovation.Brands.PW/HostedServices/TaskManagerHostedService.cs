using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using Biovation.Brands.PW.HostedServices;
using Biovation.Brands.PW.Manager;
using Biovation.CommonClasses.Manager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Biovation.Brands.PW.HostedServices
{
    public class TaskManagerHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private CancellationToken _cancellationToken;
        private readonly TaskManager _taskManager;
        private readonly IServiceProvider _services;
        private readonly ILogger<PingCollectorHostedService> _logger;
        public TaskManagerHostedService(ILogger<PingCollectorHostedService> logger, IServiceProvider services, TaskManager taskManager)
        {
            _logger = logger;
            _services = services;
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
            _taskManager.ProcessQueue().ConfigureAwait(false);
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
