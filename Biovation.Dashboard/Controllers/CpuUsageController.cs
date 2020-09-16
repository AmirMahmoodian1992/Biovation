using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Biovation.Dashboard.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    public class CpuUsageController : ControllerBase
    {


        private readonly ILogger<CpuUsageController> _logger;

        public CpuUsageController(ILogger<CpuUsageController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<List<CpuUsage>> Get()
        {
            SaveData();
            var devicesCount = 3;
            var cpuUsage = new List<CpuUsage>();
            for (int i = 0; i < devicesCount; i++)
            {
                var value = ReadData(i);
                CpuUsage cp = new CpuUsage
                {
                    Name = $"Device_Cpu:{i}",
                    CpuUsagePercentage = value,
                    Date = DateTime.Now.Ticks,

                };
                cpuUsage.Add(cp);
            }


            yield return cpuUsage;
        }




        private double GetCpuUsageForProcess()
        {

            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            Thread.Sleep(5000);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return cpuUsageTotal * 100;
        }





        public StackExchange.Redis.RedisValue ReadData(int i)
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();


            var value = cache.StringGet($"Device_Cpu:{i}");

            return value;
        }

        public void SaveData()
        {
            var devicesCount = 3;
            var rnd = new Random();
            var cache = RedisConnectorHelper.Connection.GetDatabase();

            for (int i = 1; i < devicesCount; i++)
            {

                var currentProcessName = Process.GetCurrentProcess().ProcessName;
                var cpuCounter = new PerformanceCounter("Process", "% Processor Time", currentProcessName);
                cpuCounter.NextValue();

                var percent = (int)cpuCounter.NextValue();


                var rng = new Random();
                double cpuPercentage = GetCpuUsageForProcess();
                cache.StringSet($"Device_Cpu:{i}", cpuPercentage);
            }
        }
    }
}
