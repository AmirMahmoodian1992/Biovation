using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
#if NET472
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
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
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", true, true);

            var configuration = builder.Build();
            var localIps = NetworkManager.GetLocalIpAddresses();
            var startupOption = new StartOptions();
            var urls = configuration["Urls"];
            var portString = urls.Split(':').LastOrDefault();

            if (!int.TryParse(portString, out var port))
                port = 9042;

            var message = $"Web server started on:{Environment.NewLine}                               localhost:{port}";
            startupOption.Urls.Add(new UriBuilder(Uri.UriSchemeHttp, "localhost", port).ToString());

            foreach (var ipAddress in localIps)
            {
                startupOption.Urls.Add(new UriBuilder(Uri.UriSchemeHttp, ipAddress.ToString(), port).ToString());
                message += $"{Environment.NewLine}                               {ipAddress}:{port}";
            }

            // Start OWIN host 
            using (WebApp.Start<Startup>(startupOption))
            {
                Console.WriteLine(message + Environment.NewLine);
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
