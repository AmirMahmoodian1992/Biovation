using Biovation.Services.RelayController.Domain;
using Biovation.Services.RelayController.Common;
using System;
using Biovation.Services.RelayController.Models;
using Biovation.Services.RelayController.Relays;

namespace Biovation.Services.RelayController.Commands
{
    public class CommandFactory
    {
        private readonly RelayFactory _relayFactory;

        public CommandFactory()
        {
            _relayFactory = new RelayFactory();
        }

        public ICommand Factory(int commandId, int relayId)
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
                    RelayHubModel = "Behsan",
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

            if(relay.RelayInfo.NodeNumber > relay.RelayInfo.Hub.Capacity)
                throw new Exception("the relay node number is out of range!");

            return commandId switch
            {
                CommandType.Contact => new Contact(relay: relay),
                CommandType.TurnOn => new TurnOn(relay: relay),
                CommandType.TurnOff => new TurnOff(relay: relay),
                CommandType.FlashOn => new FlashOn(relay: relay),
                CommandType.FlashOff => new FlashOff(relay: relay),
                _ => throw new ArgumentException(message:$"No command matches with the id {commandId}")
            };
        }
    }
}