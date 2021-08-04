using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Extensions.Configuration;
#if NET472
using System.Diagnostics;
using System.Threading;
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
            if (!RunningAsAdmin())
            {
                var proc = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Assembly.GetEntryAssembly()?.CodeBase!,
                    Verb = "runas"
                };

                try
                {
                    Process.Start(proc);
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("This program must be run as an administrator! \n\n" + ex);
                    Environment.Exit(0);
                }
            }

#if NET472
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", true, true);

            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                try
                {
                    //Process.GetProcesses()
                    //    .Where(pr => string.Equals(pr.ProcessName, "generalRawFixCamLPR.exe",StringComparison.InvariantCultureIgnoreCase)
                    //    || string.Equals(pr.ProcessName, "generalRawFixCamLPR", StringComparison.InvariantCultureIgnoreCase))
                    //    .ToList().ForEach(p => p.Kill());

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "taskkill",
                        Arguments = "/F /T /IM generalRawFixCamLPR.exe",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    })
                        ?.WaitForExit();
                }
                catch (Exception)
                {
                    //ignore
                }
            };

            Thread.GetDomain().ProcessExit += (_, _) =>
            {
                try
                {
                    Process.GetProcesses()
                        .Where(pr => string.Equals(pr.ProcessName, "generalRawFixCamLPR.exe", StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(pr.ProcessName, "generalRawFixCamLPR", StringComparison.InvariantCultureIgnoreCase))
                        .ToList().ForEach(p => p.Kill());

                    Process.Start(new ProcessStartInfo
                        {
                            FileName = "taskkill",
                            Arguments = "/F /T /IM generalRawFixCamLPR.exe",
                            WindowStyle = ProcessWindowStyle.Hidden,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        })
                        ?.WaitForExit();
                }
                catch (Exception)
                {
                    //ignore
                }
            };

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
                string input;

                do
                {
                    input = Console.ReadLine();
                } while (input != "biovation-pfk-exit");
            }


            try
            {
                Process.GetProcesses()
                    .Where(pr => string.Equals(pr.ProcessName, "generalRawFixCamLPR.exe", StringComparison.InvariantCultureIgnoreCase)
                                 || string.Equals(pr.ProcessName, "generalRawFixCamLPR", StringComparison.InvariantCultureIgnoreCase))
                    .ToList().ForEach(p => p.Kill());

                Process.Start(new ProcessStartInfo
                    {
                        FileName = "taskkill",
                        Arguments = "/F /T /IM generalRawFixCamLPR.exe",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    })
                    ?.WaitForExit();
            }
            catch (Exception)
            {
                //ignore
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

        private static bool RunningAsAdmin()
        {
            var id = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(id);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
