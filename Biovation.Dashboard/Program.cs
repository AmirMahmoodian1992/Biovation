using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using App.Metrics;
using App.Metrics.AspNetCore;
using Microsoft.AspNetCore;

namespace Biovation.Dashboard
{
    public class Program
    {


        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureMetricsWithDefaults(
                    builder =>
                    {
                        builder.Report.ToConsole(TimeSpan.FromSeconds(2));
                       // builder.Report.ToTextFile(@"C:\metrics.txt", TimeSpan.FromSeconds(20));
                    })
                .UseMetrics()
                .UseStartup<Startup>()
                .Build();
        }
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            BuildWebHost(args).Run();
            PerformanceCounter cpuCounter;
            PerformanceCounter ramCounter;

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
