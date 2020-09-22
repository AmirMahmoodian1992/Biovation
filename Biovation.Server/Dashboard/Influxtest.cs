using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Writes;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biovation.Server.Dashboard
{
    public class Influxtest
    {
        public static async Task MainDisabled(string[] args)
        {
            const string database = "pingTestF";
            const string retentionPolicy = "autogen";

            var client = InfluxDBClientFactory.CreateV1("http://localhost:8086",
                "",
                "".ToCharArray(),
                database,
                retentionPolicy);

            //Console.WriteLine("*** Write Points ***");

            Ping myPing = new Ping();
            PingReply reply = null;
            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(30);
                reply  =  myPing.Send("192.168.2.57", 1000);

                using (var writeApi = client.GetWriteApi())
                {
                    var point = PointData.Measurement("ping")
                        .Tag("url", "192.168.2.57")
                        .Tag("host", "192.168.2.133")
                        .Field("status", reply.Status.ToString())
                        .Field("roundtripTime", reply.RoundtripTime.ToString())
                        .Field("ttl", reply.Options.Ttl.ToString());



                    writeApi.WritePoint(point);
                }
            }
            client.Dispose();
        }
    }
}