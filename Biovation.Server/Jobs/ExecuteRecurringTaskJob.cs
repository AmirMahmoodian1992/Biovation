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
                var recurringTask = JsonConvert.DeserializeObject<TaskInfo>(context.MergedJobDataMap.GetString("TaskInfo"));
                var task = new TaskInfo
                {
                    Priority = recurringTask.Priority,
                    DeviceBrand = recurringTask.DeviceBrand,
                    DueDate = recurringTask.DueDate,
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = recurringTask.CreatedBy,
                    Status = _taskStatuses.Queued,
                    TaskItems = recurringTask.TaskItems,
                    TaskType = recurringTask.TaskType,
                    Parent = recurringTask
                };

                var insertionResult = await _taskService.InsertTask(task);
                if (insertionResult.Success)
                {
                    task.Id = (int)insertionResult.Id;
                    var restRequest = new RestRequest($"{task.DeviceBrand.Name}/{task.DeviceBrand.Name}Task/ExecuteTask", Method.POST);
                    restRequest.AddJsonBody(task);
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
