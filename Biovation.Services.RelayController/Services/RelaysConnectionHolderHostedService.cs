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
                foreach (var relayInRelays in _relayService.GetRelay().Result.Data.Data)
                {
                    if (ConnectedRelays.ContainsKey(relayInRelays.Id))
                    {
                        var relay = ConnectedRelays[relayInRelays.Id];

                        if (relay.IsConnected())
                        {
                            Console.WriteLine($"relay number {relay.RelayInfo.Id} is alive.");
                        }
                        else
                        {
                            Console.WriteLine($"relay number {relay.RelayInfo.Id} is disconnected!");
                            ConnectedRelays.Remove(relay.RelayInfo.Id);
                        }
                    }
                    else
                    {
                        var relayInfo = _relayService.GetRelay(id:relayInRelays.Id).Result?.Data?.Data?.FirstOrDefault();
                        var relay = _relayFactory.Factory(relayInfo);
                        if (relay.Connect())
                            ConnectedRelays.Add(relay.RelayInfo.Id, relay);
                        else
                        {
                            Console.WriteLine($"relay number {relay.RelayInfo.Id} not found!");
                            continue;
                        }
                        
                    }

                }
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}