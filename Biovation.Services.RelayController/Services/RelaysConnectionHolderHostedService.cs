using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;
using Biovation.Service.Api.v2.RelayController;
using Biovation.Services.RelayController.Common;
using Biovation.Services.RelayController.Relays;
using Microsoft.CodeAnalysis;


namespace Biovation.Services.RelayController.Services
{
    public class RelaysConnectionHolderHostedService : BackgroundService
    {
        public Dictionary<int, IRelay> ConnectedRelays { get; set; }
        private readonly RelayFactory _relayFactory;
        private readonly RelayService _relayService;

        public RelaysConnectionHolderHostedService(Dictionary<int, IRelay> connectedRelays, RelayFactory relayFactory, RelayService relayService)
        {
            ConnectedRelays = connectedRelays;
            _relayFactory = relayFactory;
            _relayService = relayService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                for (var relayId = 2; relayId <= 5; relayId++)
                {
                    if (ConnectedRelays.ContainsKey(relayId))
                    {
                        var relay = ConnectedRelays[relayId];

                        if (relay.IsConnected())
                        {
                            Console.WriteLine($"relay number {relayId} is alive.");
                        }
                        else
                        {
                            Console.WriteLine($"relay number {relayId} is disconnected!");
                            ConnectedRelays.Remove(relayId);
                        }
                    }
                    else
                    {
                        //var relayInfo = new Relay
                        //{
                        //    Id = relayId,
                        //    Name = $"relay_{relayId}",
                        //    NodeNumber = relayId,
                        //    RelayHub = new RelayHub
                        //    {
                        //        Id = 1,
                        //        IpAddress = "192.168.3.200",
                        //        Port = 23,
                        //        Capacity = 4,
                        //        RelayHubModel = new RelayHubModel() { Name = "Behsan" },
                        //        Description = "Blah Blah Blah"
                        //    },
                        //    Entrance = new Entrance
                        //    {
                        //        Id = 1,
                        //        Name = "MainEntrance",
                        //        Description = "Blah Blah Blah"
                        //    },
                        //    Description = "Blah Blah Blah"
                        //};
                       
                        
                        var relayInfo = _relayService.GetRelay(id:relayId).Result?.Data?.Data?.FirstOrDefault();
                        var relay = _relayFactory.Factory(relayInfo);
                        if (relay.Connect())
                            ConnectedRelays.Add(relayId, relay);
                        else
                        {
                            Console.WriteLine($"relay number {relayId} not found!");
                            continue;
                        }
                        
                    }

                }
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}