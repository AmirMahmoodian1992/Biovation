using Biovation.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using System;
using System.Threading.Tasks;
using Biovation.Constants;
using Biovation.Service.Api.v2;

namespace Biovation.Server.Jobs
{
    public class ExecuteScheduledTaskJob : IJob
    {
        private readonly RestClient _restClient;
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;
        private readonly ILogger<ExecuteScheduledTaskJob> _logger;

        public ExecuteScheduledTaskJob(ILogger<ExecuteScheduledTaskJob> logger, RestClient restClient, TaskService taskService, TaskStatuses taskStatuses)
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
                _logger.LogDebug("Inside the ExecuteScheduledTaskJob");
                var taskInfoJson = context.MergedJobDataMap.GetString("TaskInfo");
                if (string.IsNullOrWhiteSpace(taskInfoJson))
                    return;
                
                var scheduledTaskInfo = JsonConvert.DeserializeObject<TaskInfo>(taskInfoJson);
                var executingTaskInfo = new TaskInfo
                {
                    Priority = scheduledTaskInfo.Priority,
                    DeviceBrand = scheduledTaskInfo.DeviceBrand,
                    DueDate = scheduledTaskInfo.DueDate,
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = scheduledTaskInfo.CreatedBy,
                    Status = _taskStatuses.Queued,
                    TaskItems = scheduledTaskInfo.TaskItems,
                    TaskType = scheduledTaskInfo.TaskType,
                    Parent = scheduledTaskInfo
                };

                executingTaskInfo.TaskItems.ForEach(taskItem => taskItem.Status = _taskStatuses.Queued);
                var insertionResult = _taskService.InsertTask(executingTaskInfo);
                
                if (insertionResult?.Success ?? false)
                {
                    //executingTaskInfo.Id = (int) insertionResult.Id;

                    var restRequest =
                        new RestRequest(
                            $"{executingTaskInfo.DeviceBrand.Name}/{executingTaskInfo.DeviceBrand.Name}Task/RunProcessQueue",
                            Method.POST);
                    
                    // restRequest.AddHeader("Authorization", _biovationConfigurationManager.DefaultToken);
                    // restRequest.AddJsonBody(executingTaskInfo);
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
