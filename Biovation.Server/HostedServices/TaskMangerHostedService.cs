using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Server.Jobs;
using Biovation.Service.Api.v2;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Biovation.Server.HostedServices
{
    public class TaskMangerHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly TaskService _taskService;
        private readonly IScheduler _scheduler;
        private readonly ILogger<TaskMangerHostedService> _logger;

        private List<TaskInfo> _scheduledTasks;

        public static bool NewTasksAdded = true;

        public TaskMangerHostedService(ILogger<TaskMangerHostedService> logger, TaskService taskService, ISchedulerFactory schedulerFactory)
        {
            _logger = logger;
            _taskService = taskService;
            _scheduler = schedulerFactory.GetScheduler().Result;
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

                var storedScheduledTasks = _taskService.GetTasks(taskStatusCodes: new List<string> { TaskStatuses.ScheduledCode }).Data?.Data;
                if (storedScheduledTasks != null)
                {
                    foreach (var storedScheduledTask in storedScheduledTasks)
                    {
                        if (_scheduledTasks.All(task => task.Id != storedScheduledTask.Id)) continue;
                        
                        var job = JobBuilder.Create<ExecuteScheduledTaskJob>().WithIdentity(storedScheduledTask.Id.ToString(), "ScheduledTasks")
                            .UsingJobData("TaskId", storedScheduledTask.Id).Build();
                        var trigger = TriggerBuilder.Create().StartAt(storedScheduledTask.DueDate).Build();
                        _scheduler.ScheduleJob(job, trigger);
                    }
                }
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
