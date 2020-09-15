using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Biovation.Dashboard.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CpuUsageController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<CpuUsageController> _logger;

        public CpuUsageController(ILogger<CpuUsageController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<CpuUsage> Get()
        {



            var currentProcessName = Process.GetCurrentProcess().ProcessName;
            var cpuCounter = new PerformanceCounter("Process", "% Processor Time", currentProcessName);
            cpuCounter.NextValue();

            var percent = (int)cpuCounter.NextValue();


            var rng = new Random();
            double cpu = GetCpuUsageForProcess();
            SaveBigData(cpu);

            ReadData();
            yield return new CpuUsage
            {
                Date = DateTime.Now.Ticks,
                CpuUsagePercentage = cpu
            };
        }


        private double GetCpuUsageForProcess()
        {

            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            Thread.Sleep(500);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return cpuUsageTotal * 100;
        }

        public void ReadData()
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();

            var value = cache.StringGet($"cpu");

        }

        public void SaveBigData(double cpuPercentage)
        {

            var cache = RedisConnectorHelper.Connection.GetDatabase();
            cache.StringSet($"cpu", cpuPercentage);
            cache.StringSet(DateTime.Now.Ticks.ToString(), cpuPercentage);

        }
    }
}
