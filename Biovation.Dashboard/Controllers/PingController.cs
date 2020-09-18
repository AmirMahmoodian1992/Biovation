using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Formatters.Json;
using App.Metrics.Gauge;
using Biovation.Dashboard.Repository;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using ServiceStack.Redis;

namespace Biovation.Dashboard.Controllers
{
    [Route("[controller]")]
    public class PingController : Controller
    {
        private readonly PingRepository _pingRepository;
        private readonly RestClient _restClient;
        public PingController(PingRepository pingRepository, RestClient restClient)
        {
            _restClient = restClient;
            _pingRepository = pingRepository;
        }


        [HttpGet]
        [Obsolete]
        public Task<ResultViewModel> GetPing(int cnt = 1000)
        {

            return Task.Run(() =>
            {
                var myIp = Dns.GetHostByName(Dns.GetHostName()).AddressList.FirstOrDefault()?.ToString();
                PingReply reply = null;
                var myPing = new Ping();

                //reply = myPing.Send("192.168.2.196", 1000);
                //var replyPing = new PingModel()
                //{
                //    address = reply.Address.ToString(),
                //    ttl = reply.Options.Ttl,
                //    roundTripTime = reply.RoundtripTime,
                //    status = reply.Status.ToString()
                //};
                //if (reply != null)
                //{
                //    Console.WriteLine("Status :  " + reply.Status + " \n Time : " + reply.RoundtripTime.ToString() + " \n Address : " + reply.Address);
                //    //Console.WriteLine(reply.ToString());

                //}
                //var response = JsonConvert.SerializeObject(replyPing);
                //var success = Save(host, key , response);

                //Console.WriteLine("success: " + success);
                //Console.WriteLine("success: " + success);



                var result = new ResultViewModel()
                {
                    Validate = 1,
                    Message = " با موفقیت اضافه شدند"
                };
                var pingResult = new ResultViewModel();

                var restRequest = new RestRequest($"/Devices",
                    Method.GET);
                var restResult = _restClient.ExecuteAsync<List<DeviceBasicInfo>>(restRequest);
                foreach (var device in restResult.Result.Data)
                {
                    _ = Task.Run(() =>
                      {
                          reply = myPing.Send(device.IpAddress, 1000);
                          var replyPing = new PingModel()
                          {
                              //address = reply.Address.ToString(),
                              hostAddress = myIp,
                              DestinationAddress = device.IpAddress,
                              ttl = reply.Options.Ttl,
                              roundTripTime = reply.RoundtripTime,
                              status = reply?.Status.ToString()
                          };

                          pingResult = _pingRepository.AddPingTimeStamp(replyPing);
                          if (result.Validate == 1 && pingResult.Validate != 1)
                          {
                              result = pingResult;
                          }
                      });
                }

                return result;
            });

        }


        [HttpGet]
        [Route("cpu")]

        public IActionResult GetCpu()
        {

            var metrics = new MetricsBuilder().Report.ToConsole(
                    options =>
                    {
                        options.MetricsOutputFormatter = new MetricsJsonOutputFormatter();
                    })
                .Build();
            var processPhysicalMemoryGauge = new GaugeOptions
            {
                Name = "Process Physical Memory",
                MeasurementUnit = Unit.Bytes
            };

            var process = Process.GetCurrentProcess();

            metrics.Measure.Gauge.SetValue(processPhysicalMemoryGauge, process.WorkingSet64);
            metrics.ReportRunner.RunAllAsync();

            return Ok();
        }

        [HttpGet]
        [Route("readcpu")]
        public IActionResult ReadCpu(string str)
        {
            var result = JsonConvert.DeserializeObject<GaugeMetric>(str);

            return Ok(result);
        }




        private static bool Save(string host, string key, string value)
        {
            bool isSuccess = false;
            using (RedisClient redisClient = new RedisClient(host))
            {
                if (redisClient.Get<string>(key) == null)
                {
                    isSuccess = redisClient.Set(key, value);
                }
            }
            return isSuccess;
        }
        private static string Get(string host, string key)
        {
            using (RedisClient redisClient = new RedisClient(host))
            {
                return redisClient.Get<string>(key);
            }
        }



    }
}
