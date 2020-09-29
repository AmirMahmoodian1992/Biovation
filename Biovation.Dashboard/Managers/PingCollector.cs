using Biovation.Dashboard.Repository;
using Biovation.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Biovation.Dashboard.Managers
{
    public class PingCollector
    {
        private readonly string _localIpAddress;
        private readonly RestClient _restClient;
        private readonly PingRepository _pingRepository;
        private readonly ILogger<PingCollector> _logger;

        public PingCollector(RestClient restClient, PingRepository pingRepository, ILogger<PingCollector> logger, string localIpAddress)
        {
            _logger = logger;
            _restClient = restClient;
            _pingRepository = pingRepository;
            _localIpAddress = localIpAddress;
        }

        public void CollectPing()
        {
            try
            {
                var restRequest = new RestRequest("/device/devices", Method.GET);
                var restResult = _restClient.ExecuteAsync<List<DeviceBasicInfo>>(restRequest).Result.Data;

                if (restResult is null)
                    return;

                foreach (var device in restResult)
                {
                    _ = Task.Run(() =>
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

                        _pingRepository.AddPingTimeStamp(pingReply);
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
