using Biovation.Domain;
using Biovation.Server.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.Server.HostedServices
{
    public class TaskMangerHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _services;
        private readonly ILogger<TaskMangerHostedService> _logger;

        private readonly List<TaskInfo> _scheduledTasks = new List<TaskInfo>();

        public static bool NewTasksAdded = true;

        public TaskMangerHostedService(ILogger<TaskMangerHostedService> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ScheduleNewTasks, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));


            return Task.CompletedTask;
        }

        private void ScheduleNewTasks(object state)
        {
            _logger.LogDebug($"In TaskMangerHostedService, The value is: {NewTasksAdded}");

            if (NewTasksAdded)
            {
                NewTasksAdded = false;

                using var scope = _services.CreateScope();
                var scheduledTasksManager = scope.ServiceProvider.GetRequiredService<ScheduledTasksManager>();
                scheduledTasksManager.ScheduleTasks(_scheduledTasks);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
