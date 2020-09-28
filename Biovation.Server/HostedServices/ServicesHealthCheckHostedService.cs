using Biovation.Constants;
using Biovation.Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Biovation.Server.HostedServices
{
    public class ServicesHealthCheckHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly Lookups _lookups;
        private readonly RestClient _restClient;
        private readonly SystemInfo _systemInformation;
        private readonly ILogger<ServicesHealthCheckHostedService> _logger;

        public ServicesHealthCheckHostedService(RestClient restClient, SystemInfo systemInformation, Lookups lookups, ILogger<ServicesHealthCheckHostedService> logger)
        {
            _logger = logger;
            _lookups = lookups;
            _restClient = restClient;
            _systemInformation = systemInformation;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Services Health Check Hosted Service running.");

            _systemInformation.Services = new List<ServiceInfo>();
            _timer = new Timer(CheckServicesStatus, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void CheckServicesStatus(object state)
        {
            //var count = Interlocked.Increment(ref _executionCount);

            var deviceBrands = _lookups.DeviceBrands;
            Parallel.ForEach(deviceBrands, deviceBrand =>
            {
                var restRequest = new RestRequest(
                    $"{deviceBrand.Name}/health");
                var result = _restClient.Execute(restRequest);

                if (result.StatusCode == HttpStatusCode.OK && string.Equals(result.Content, "Healthy", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!_systemInformation.Services.Any(service => service.Name.Contains(deviceBrand.Name)))
                        _systemInformation.Services.Add(new ServiceInfo { Name = deviceBrand.Name });
                }
                else
                {
                    if (_systemInformation.Services.Any(service => service.Name.Contains(deviceBrand.Name)))
                        _systemInformation.Services.Remove(
                            _systemInformation.Services.Find(service => service.Name.Contains(deviceBrand.Name)));
                }
            });

            //_logger.LogInformation(
            //    "Timed Hosted Service is working. Count: {Count}", count);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Services Health Check Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
