using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Histogram;
using Biovation.Constants;
using Biovation.Domain;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Biovation.Brands.Paliz.Manager
{
    public class PingCollector
    {
        private readonly string _localIpAddress;
        private readonly IMetricsRoot _metrics;
        private readonly RestClient _restClient;
        private readonly ILogger<PingCollector> _logger;
        private readonly HistogramOptions _histogramOptions;

        public PingCollector(RestClient restClient, IMetricsRoot metrics, ILogger<PingCollector> logger, string localIpAddress)
        {
            _logger = logger;
            _restClient = restClient;
            _metrics = metrics;
            _localIpAddress = localIpAddress;

            _histogramOptions = new HistogramOptions
            {
                Name = "Device Ping Round Trip Time",
                MeasurementUnit = Unit.Custom("Millisecond")
            };
        }

        public void CollectPing()
        {
            try
            {
                var restRequest = new RestRequest("/device/devices", Method.GET);
                restRequest.AddQueryParameter("brandId", DeviceBrands.PalizCode);
                var restResult = _restClient.ExecuteAsync<List<DeviceBasicInfo>>(restRequest).Result.Data;

                if (restResult is null)
                    return;

                foreach (var device in restResult)
                {
                    Task.Run(() =>
                    {
                        var ping = new Ping();
                        var reply = ping.Send(device.IpAddress, 1000);
                        var pingReply = new PingStatus
                        {
                            HostAddress = _localIpAddress,
                            DestinationAddress = device.IpAddress,
                            TimeToLive = reply?.Options?.Ttl ?? 0,
                            RoundTripTime = reply?.RoundtripTime ?? int.MaxValue,
                            Status = reply?.Status.ToString()
                        };

                        _metrics.Measure.Histogram.Update(_histogramOptions, new MetricTags("host_address", pingReply.HostAddress), pingReply.RoundTripTime, pingReply.DestinationAddress);
                    });
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, exception.Message);
            }
        }

    }
}
