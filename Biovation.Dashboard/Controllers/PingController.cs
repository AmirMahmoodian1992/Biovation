using Biovation.CommonClasses.Manager;
using Biovation.Dashboard.Repository;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Timers;

namespace Biovation.Dashboard.Controllers
{
    [Route("[controller]")]
    public class PingController : Controller
    {
        private readonly RestClient _restClient;
        private readonly PingRepository _pingRepository;
        public PingController(PingRepository pingRepository, RestClient restClient)
        {
            _restClient = restClient;
            _pingRepository = pingRepository;
        }

        [HttpGet]
        [Obsolete]
        public void GetPing(int cnt = 1000)
        {
            Task.Run(() =>
            {
                const double interval60Minutes = 5 * 1000; // milliseconds to one hour

                var checkForTime = new Timer(interval60Minutes);
                checkForTime.Elapsed += TimeElapsed;
                checkForTime.Enabled = true;
            });
        }

        [Obsolete]
        private void TimeElapsed(object sender, ElapsedEventArgs e)
        {
            var myIp = NetworkManager.GetLocalIpAddresses().FirstOrDefault()?.ToString();

            var restRequest = new RestRequest("/device/devices", Method.GET);
            var restResult = _restClient.ExecuteAsync<List<DeviceBasicInfo>>(restRequest).Result.Data;

            foreach (var device in restResult)
            {
                _ = Task.Run(() =>
                {
                    var ping = new Ping();
                    var reply = ping.Send(device.IpAddress, 1000);
                    var pingReply = new PingStatus
                    {
                        HostAddress = myIp,
                        DestinationAddress = device.IpAddress,
                        TimeToLive = reply?.Options?.Ttl ?? 0,
                        RoundTripTime = reply?.RoundtripTime ?? int.MaxValue,
                        Status = reply?.Status.ToString()
                    };

                    _pingRepository.AddPingTimeStamp(pingReply);
                });
            }
        }
    }
}
