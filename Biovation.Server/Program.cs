using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Globalization;

namespace Biovation.Server
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
            //    //.Enrich.WithHttpRequestId()
            //    //.Enrich.WithWebApiActionName()
            //    //.Enrich.WithWebApiControllerName()
            //    //.Enrich.WithWebApiRouteData()
            //    //.WriteTo.Console();
            //    .WriteTo.Console(
            //        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}, {ThreadId}] {Message}{NewLine}{Exception}"
            //        /*,restrictedToMinimumLevel: minimumConsoleLogLevel*/)
            //    .CreateLogger();
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                    .ConfigureMetricsWithDefaults(
                    builder =>
                    {
                        builder.Report.OverHttp(options =>
                        {
                            options.HttpSettings.RequestUri = new Uri("http://localhost:9038/biovation/api/dashboard/metrics");
                            //options.HttpSettings.UserName = "admin";
                            //options.HttpSettings.Password = "password";
                            options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
                            options.HttpPolicy.FailuresBeforeBackoff = 2;
                            options.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
                            options.MetricsOutputFormatter = new MetricsJsonOutputFormatter();
                            //options.Filter = filter;
                            options.FlushInterval = TimeSpan.FromSeconds(20);
                        });
                    }).UseSerilog().UseMetrics().UseWindowsService()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    });
        }
    }
}
