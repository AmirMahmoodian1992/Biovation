using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Services.RelayController.Common;
using Biovation.Services.RelayController.Relays;


namespace Biovation.Services.RelayController.Services
{
    public class RelaysConnectionHolderHostedService : BackgroundService
    {
        public Dictionary<int, IRelay> ConnectedRelays { get; set; }
        private readonly RelayFactory _relayFactory;

        public RelaysConnectionHolderHostedService(Dictionary<int, IRelay> connectedRelays, RelayFactory relayFactory)
        {
            ConnectedRelays = connectedRelays;
            _relayFactory = relayFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                for (var relayId = 1; relayId <= 4; relayId++)
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
                        var relayInfo = new Relay
                        {
                            Id = relayId,
                            Name = $"relay_{relayId}",
                            NodeNumber = relayId,
                            Hub = new RelayHub
                            {
                                Id = 1,
                                IpAddress = "192.168.1.200",
                                Port = 23,
                                Capacity = 4,
                                RelayHubModel = new DeviceModel { Name = "Behsan" },
                                Description = "Blah Blah Blah"
                            },
                            Entrance = new Entrance
                            {
                                Id = 1,
                                Name = "MainEntrance",
                                Description = "Blah Blah Blah"
                            },
                            Description = "Blah Blah Blah"
                        };
                        var relay = _relayFactory.Factory(relayInfo);
                        try
                        {
                            relay.Connect();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"relay number {relayId} not found!");
                            continue;
                        }
                        ConnectedRelays.Add(relayId,relay);
                    }

                }
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}