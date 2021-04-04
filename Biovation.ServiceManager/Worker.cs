using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.ServiceManager
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IList<string> _baseServices;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _baseServices = new List<string> { "Biovation.Gateway", "Biovation.Data.Queries", "Biovation.Data.Commands", "Biovation.Server" };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var waitValue = 1.584;
            while (!stoppingToken.IsCancellationRequested)
            {
                var startBaseServices = true;
                foreach (var service in _baseServices)
                {
                    if (!startBaseServices)
                    {
                        StopService(service);
                        continue;
                    }

                    _logger.LogDebug("Checking status of {service} service at {time}", service, DateTimeOffset.Now);
                    startBaseServices = CheckServiceStatus(service);
                }

                var services = ServiceController.GetServices();
                var biovationBrandsServices = services.Where(service => service.ServiceName.ToLower().Contains("biovation.brands"));
                
                if (!startBaseServices)
                {
                    Parallel.ForEach(biovationBrandsServices, brandService =>
                    {
                        _logger.LogDebug("Checking status of {service} service at {time}", brandService, DateTimeOffset.Now);
                        StopService(brandService.ServiceName);
                    });

                    await Task.Delay(3000, stoppingToken);
                    waitValue = 1.584; 
                    continue;
                }

                Parallel.ForEach(biovationBrandsServices, brandService =>
                {
                    _logger.LogDebug("Checking status of {service} service at {time}", brandService, DateTimeOffset.Now);
                    CheckServiceStatus(brandService.ServiceName);
                });

                await Task.Delay((int)(10000 * Math.Log10(waitValue)), stoppingToken);
                if (waitValue < 1000)
                    waitValue *= 5;
            }
        }


        private static bool CheckServiceStatus(string serviceName)
        {
            var service = new ServiceController(serviceName);

            try
            {
                if (service.Status == ServiceControllerStatus.Running)
                    return true;

                if (service.Status != ServiceControllerStatus.Stopped)
                {
                    //service.Stop();
                    //service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(3));
                    StopService(serviceName);
                }

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(20));
                return true;
            }
            catch
            {
                //ignore
                return false;
            }
            finally
            {
                service.Close();
                service.Dispose();
            }
        }

        private static void StopService(string serviceName)
        {
            var service = new ServiceController(serviceName);

            try
            {
                if (service.Status == ServiceControllerStatus.Stopped) return;
                
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
            }
            catch
            {
                //ignore
            }
            finally
            {
                service.Close();
                service.Dispose();
            }

            StopProcess(serviceName);
        }

        private static void StopProcess(string processName)
        {
            var serviceProcesses = new List<Process>();
            serviceProcesses.AddRange(Process.GetProcessesByName(processName));
            serviceProcesses.AddRange(Process.GetProcessesByName(processName + ".exe"));
            foreach (var serviceProcess in serviceProcesses)
            {
                try
                {
                    serviceProcess.Kill();
                    serviceProcess.WaitForExit(10000);
                    serviceProcess.Dispose();
                }
                catch (Exception)
                {
                    //ignore
                }
            }
        }
    }
}
