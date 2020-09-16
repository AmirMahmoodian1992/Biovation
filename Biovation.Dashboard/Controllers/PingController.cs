using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using Biovation.CommonClasses.Manager;
using System.Text.Json;
using System.Threading.Tasks;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using ServiceStack.Redis;

namespace Biovation.Dashboard.Controllers
{
    [Route("[controller]")]
    public class PingController : Controller
    {
        private string host;
        private readonly RestClient _restClient;
        public PingController()
        {
           host =  "localhost";
           _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }


        [HttpGet]
        [Obsolete]
        public async Task<IActionResult> GetPing(int cnt = 1000)
        {
           
            const string key = "192.168.2.133";
            string myIp = Dns.GetHostByName(Dns.GetHostName()).AddressList.FirstOrDefault()?.ToString();


            PingReply reply = null;
            
                Ping myPing = new Ping();
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



                
                var restRequest = new RestRequest($"/Devices",
                    Method.GET);
                var restResult = await _restClient.ExecuteAsync<List<DeviceBasicInfo>>(restRequest);
                foreach (var device in restResult.Data)
                {
                _ = Task.Run(() =>
                  {
                       reply =  myPing.Send(device.IpAddress, 1000);
                      var replyPing = new PingModel()
                      {
                          //address = reply.Address.ToString(),
                          hostAddress = myIp,
                          DestinationAddress = device.IpAddress,
                          ttl = reply.Options.Ttl,
                          roundTripTime = reply.RoundtripTime,
                          status = reply?.Status.ToString()
                      };
                      var response = JsonConvert.SerializeObject(replyPing);
                      Save(host, device.IpAddress, response);

                  });
                }

            return Ok();

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
