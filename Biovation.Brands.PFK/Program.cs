using System;

#if NET472
using Microsoft.Owin.Hosting;
#elif NETCORE31
    using App.Metrics;
    using App.Metrics.AspNetCore;
    using App.Metrics.Formatters.Json;
    using Microsoft.AspNetCore.Hosting;
#endif

namespace Biovation.Brands.PFK
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if NET472

            string baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine($"Host started on {baseAddress}");
                Console.ReadLine();
            }

#elif NETCORE31
            CreateHostBuilder(args).Build().Run();
#endif
        }

#if NETCORE31

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
#endif
    }
}
