using Biovation.Brands.EOS.Manager;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.Brands.EOS.HostedServices
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
