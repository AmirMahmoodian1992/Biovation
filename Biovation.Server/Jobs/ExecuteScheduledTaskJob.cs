using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Biovation.Server.Jobs
{
    public class ExecuteScheduledTaskJob : IJob
    {
        private readonly ILogger<ExecuteScheduledTaskJob> _logger;
        public ExecuteScheduledTaskJob(ILogger<ExecuteScheduledTaskJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogDebug("Inside the job");
            return Task.CompletedTask;
        }
    }
}
