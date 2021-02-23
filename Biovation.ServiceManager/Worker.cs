using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var service in _baseServices)
                {
                    _logger.LogDebug("Checking status of {service} service at {time}", service, DateTimeOffset.Now);
                    CheckServiceStatus(service);
                }

                var services = ServiceController.GetServices();
                var biovationBrandsServices = services.Where(service => service.ServiceName.ToLower().Contains("biovation.brands"));

                Parallel.ForEach(biovationBrandsServices, brandService =>
                {
                    _logger.LogDebug("Checking status of {service} service at {time}", brandService, DateTimeOffset.Now);
                    CheckServiceStatus(brandService.ServiceName);
                });

                await Task.Delay(30000, stoppingToken);
            }
        }


        private void CheckServiceStatus(string serviceName)
        {
            var service = new ServiceController(serviceName);
            
            try
            {
                if (service.Status == ServiceControllerStatus.Running)
                    return;

                if (service.Status != ServiceControllerStatus.Stopped)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(3));
                }

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(20));
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
        }
    }
}
