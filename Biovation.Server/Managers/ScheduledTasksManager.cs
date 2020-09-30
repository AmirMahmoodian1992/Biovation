using Biovation.Constants;
using Biovation.Domain;
using Biovation.Server.Jobs;
using Biovation.Service.Api.v2;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses;

namespace Biovation.Server.Managers
{
    public class ScheduledTasksManager
    {
        private readonly IScheduler _scheduler;
        private readonly TaskService _taskService;

        public ScheduledTasksManager(ISchedulerFactory schedulerFactory, TaskService taskService)
        {
            _taskService = taskService;
            _scheduler = schedulerFactory.GetScheduler().Result;
        }

        public Task ScheduleTasks(List<TaskInfo> scheduledTasks)
        {
            try
            {
                var storedScheduledTasks = _taskService.GetTasks(taskStatusCodes: new List<string> { TaskStatuses.ScheduledCode })?.Data?.Data;
                if (storedScheduledTasks != null)
                {
                    var upcomingScheduledTasks = storedScheduledTasks.Where(task => task.DueDate >= DateTimeOffset.Now);
                    foreach (var storedScheduledTask in upcomingScheduledTasks)
                    {
                        if (scheduledTasks.Any(task => task.Id == storedScheduledTask.Id)) continue;

                        var job = JobBuilder.Create<ExecuteScheduledTaskJob>().WithIdentity(storedScheduledTask.Id.ToString(), "ScheduledTasks")
                            .UsingJobData("TaskInfo", JsonConvert.SerializeObject(storedScheduledTask)).Build();
                        var trigger = TriggerBuilder.Create().StartAt(storedScheduledTask.DueDate).Build();
                        _scheduler.ScheduleJob(job, trigger);

                        scheduledTasks.Add(storedScheduledTask);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }

            return Task.CompletedTask;
        }
    }
}
