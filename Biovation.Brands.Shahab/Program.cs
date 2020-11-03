using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Biovation.Brands.Shahab
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureMetricsWithDefaults(
                    builder =>
                    {
                        builder.Report.OverHttp(options =>
                        {
                            options.HttpSettings.RequestUri = new Uri("http://localhost:9038/biovation/api/dashboard/metrics");
                            options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
                            options.HttpPolicy.FailuresBeforeBackoff = 2;
                            options.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
                            options.MetricsOutputFormatter = new MetricsJsonOutputFormatter();
                            options.FlushInterval = TimeSpan.FromSeconds(20);
                        });
                    })
                .UseSerilog().UseMetrics().UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
