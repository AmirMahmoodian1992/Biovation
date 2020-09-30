using Biovation.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Biovation.Server.Jobs
{
    public class ExecuteScheduledTaskJob : IJob
    {
        private readonly RestClient _restClient;
        private readonly ILogger<ExecuteScheduledTaskJob> _logger;
        public ExecuteScheduledTaskJob(ILogger<ExecuteScheduledTaskJob> logger, RestClient restClient)
        {
            _logger = logger;
            _restClient = restClient;
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogDebug("Inside the job");
                var task = JsonConvert.DeserializeObject<TaskInfo>(context.MergedJobDataMap.GetString("TaskInfo"));
                var restRequest = new RestRequest($"{task.DeviceBrand.Name}/{task.DeviceBrand.Name}Task/ExecuteTask", Method.POST);
                restRequest.AddJsonBody(task);
                return _restClient.ExecuteAsync(restRequest);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, exception.Message);
                return Task.CompletedTask;
            }
        }
    }
}
