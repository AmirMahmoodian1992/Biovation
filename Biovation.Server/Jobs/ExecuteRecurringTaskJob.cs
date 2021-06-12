using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Biovation.Server.Jobs
{
    public class ExecuteRecurringTaskJob : IJob
    {
        private readonly RestClient _restClient;
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;
        private readonly ILogger<ExecuteRecurringTaskJob> _logger;

        public ExecuteRecurringTaskJob(ILogger<ExecuteRecurringTaskJob> logger, RestClient restClient, TaskService taskService, TaskStatuses taskStatuses)
        {
            _logger = logger;
            _restClient = restClient;
            _taskService = taskService;
            _taskStatuses = taskStatuses;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogDebug("Inside the ExecuteRecurringTaskJob");
                var taskInfoJson = context.MergedJobDataMap.GetString("TaskInfo");
                if (string.IsNullOrWhiteSpace(taskInfoJson))
                    return;

                var recurringTaskInfo = JsonConvert.DeserializeObject<TaskInfo>(taskInfoJson);
                var executingTaskInfo = new TaskInfo
                {
                    Priority = recurringTaskInfo.Priority,
                    DeviceBrand = recurringTaskInfo.DeviceBrand,
                    DueDate = recurringTaskInfo.DueDate,
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = recurringTaskInfo.CreatedBy,
                    Status = _taskStatuses.Queued,
                    TaskItems = recurringTaskInfo.TaskItems,
                    TaskType = recurringTaskInfo.TaskType,
                    Parent = recurringTaskInfo
                };

                executingTaskInfo.TaskItems.ForEach(taskItem => taskItem.Status = _taskStatuses.Queued);
                var insertionResult = await _taskService.InsertTask(executingTaskInfo);
                
                if (insertionResult?.Success ?? false)
                {
                    var restRequest = new RestRequest($"{executingTaskInfo.DeviceBrand.Name}/{executingTaskInfo.DeviceBrand.Name}Task/RunProcessQueue", Method.POST);
                    await _restClient.ExecuteAsync(restRequest);
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, exception.Message);
            }
        }
    }
}
