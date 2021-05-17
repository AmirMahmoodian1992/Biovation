using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using KasraLockRequests;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private Timer _lockTimer;
        private readonly Lookups _lookups;
        private readonly RestClient _restClient;
        private readonly SystemInfo _systemInformation;
        private readonly ILogger<ServicesHealthCheckHostedService> _logger;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public ServicesHealthCheckHostedService(RestClient restClient, SystemInfo systemInformation, Lookups lookups, ILogger<ServicesHealthCheckHostedService> logger, BiovationConfigurationManager biovationConfigurationManager)
        {
            _logger = logger;
            _lookups = lookups;
            _restClient = restClient;
            _systemInformation = systemInformation;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Services Health Check Hosted Service running.");

            _timer = new Timer(CheckServicesStatus, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));
            _lockTimer = new Timer(CheckLockStatus, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private void CheckServicesStatus(object state)
        {
            if (!_biovationConfigurationManager.UseHealthCheck)
                return;

            _systemInformation.Services ??= new List<ServiceInfo>();

            var deviceBrands = _lookups.DeviceBrands;
            Parallel.ForEach(deviceBrands, deviceBrand =>
            {
                try
                {
                    var restRequest = new RestRequest(
                        $"{deviceBrand.Name}/health");
                    var result = _restClient.Execute(restRequest);

                    if (result.StatusCode == HttpStatusCode.OK && result.Content.ToLowerInvariant().Contains("Healthy".ToLowerInvariant()))
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
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, exception.Message);
                }
            });
        }

        private void CheckLockStatus(object state)
        {
            try
            {
                var requests = new Requests();

                var response = new CoconutServiceResultModel();
                if (_biovationConfigurationManager.SoftwareLockAddress != default &&
                    _biovationConfigurationManager.SoftwareLockPort != default)
                {
                    response = requests.RequestInfo(_biovationConfigurationManager.SoftwareLockAddress, _biovationConfigurationManager.SoftwareLockPort, "info", "1", "1");
                }

                if (response == null)
                {
                    CallStopServices();
                    return;
                }

                JObject lockInfo;
                try
                {
                    lockInfo = JsonConvert.DeserializeObject<JObject>(response.Message);
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, exception.Message);
                    CallStopServices();
                    return;
                }

                string expirationDate = null;
                if (!(lockInfo is null))
                {
                    try
                    {
                        var subsystemsInfo =
                            JsonConvert.DeserializeObject<JArray>(lockInfo["SubSystems"]?.ToString() ?? string.Empty);
                        foreach (var subsystemInfo in subsystemsInfo)
                        {
                            if (!string.Equals(subsystemInfo["SubSystemId"]?.ToString(), "92"
                                , StringComparison.InvariantCultureIgnoreCase)) continue;
                            if (subsystemInfo["ExpirationDate"]?.ToString() != null)
                            {
                                expirationDate = subsystemInfo["ExpirationDate"].ToString();
                            }
                        }

                        if (expirationDate != null && DateTime.Parse(expirationDate) >= DateTime.Now) return;
                    }
                    catch (Exception innerException)
                    {
                        _logger.LogWarning(innerException, innerException.Message);
                        CallStopServices();
                    }

                    CallStopServices();
                    return;
                }

                CallStopServices();
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, exception.Message);
                CallStopServices();
            }
        }

        private void CallStopServices()
        {
            try
            {
                var deviceBrands = _lookups.DeviceBrands;
                foreach (var restRequest in deviceBrands.Select(deviceBrand => new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}SystemInfo/StopService", Method.GET)))
                {
                    _restClient.ExecuteAsync(restRequest);
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, exception.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Services Health Check Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);
            _lockTimer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _lockTimer?.Dispose();
        }
    }
}
