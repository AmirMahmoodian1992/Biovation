using Biovation.CommonClasses;
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

namespace Biovation.Server.Managers
{
    public class RecurringTasksManager
    {
        private readonly IScheduler _scheduler;
        private readonly TaskService _taskService;

        public RecurringTasksManager(ISchedulerFactory schedulerFactory, TaskService taskService)
        {
            _taskService = taskService;
            _scheduler = schedulerFactory.GetScheduler().Result;
        }

        public Task ScheduleTasks(List<TaskInfo> recurringTasks)
        {
            try
            {
                var storedRecurringTasks = _taskService.GetTasks(taskStatusCodes: new List<string> { TaskStatuses.RecurringCode })?.Data?.Data;
                if (storedRecurringTasks != null)
                {
                    //var upcomingRecurringTasks = storedRecurringTasks.Where(task => task.DueDate >= DateTimeOffset.Now);
                    foreach (var storedRecurringTask in storedRecurringTasks)
                    {
                        if (recurringTasks.Any(task => task.Id == storedRecurringTask.Id)) continue;

                        var job = JobBuilder.Create<ExecuteRecurringTaskJob>().WithIdentity(storedRecurringTask.Id.ToString(), "RecurringTasks")
                            .UsingJobData("TaskInfo", JsonConvert.SerializeObject(storedRecurringTask)).Build();
                        var trigger = TriggerBuilder.Create().StartAt(storedRecurringTask.DueDate < DateTimeOffset.Now ? DateTimeOffset.Now.AddSeconds(3) : storedRecurringTask.DueDate).WithCronSchedule(storedRecurringTask.SchedulingPattern).Build();
                        _scheduler.ScheduleJob(job, trigger);

                        recurringTasks.Add(storedRecurringTask);
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
