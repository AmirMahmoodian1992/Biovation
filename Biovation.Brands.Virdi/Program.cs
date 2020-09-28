using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Json;

namespace Biovation.Brands.Virdi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .Enrich.FromLogContext()
            //    .MinimumLevel.Verbose()
            //    .Enrich.With(new ThreadIdEnricher())
            //    .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version)
            //    .WriteTo.Console(
            //        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}, {ThreadId}] {Message}{NewLine}{Exception}"
            //        /*,restrictedToMinimumLevel: minimumConsoleLogLevel*/)
            //    .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureMetricsWithDefaults(
                    builder =>
                    {
                        builder.Report.OverHttp(options => {
                            options.HttpSettings.RequestUri = new Uri("http://localhost:9060/api/metrics");
                            //options.HttpSettings.UserName = "admin";
                            //options.HttpSettings.Password = "password";
                            options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
                            options.HttpPolicy.FailuresBeforeBackoff = 2;
                            options.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
                            options.MetricsOutputFormatter = new MetricsJsonOutputFormatter();
                            //options.Filter = filter;
                            options.FlushInterval = TimeSpan.FromSeconds(20);
                        });
                    })
                .UseSerilog().UseMetrics()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
