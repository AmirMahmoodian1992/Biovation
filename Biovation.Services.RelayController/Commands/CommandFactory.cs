using Biovation.Domain.RelayModels;
using Biovation.Services.RelayController.Common;
using Biovation.Services.RelayController.Relays;
using Biovation.Services.RelayController.Services;
using System;
using CommandType = Biovation.Services.RelayController.Domain.CommandType;

namespace Biovation.Services.RelayController.Commands
{
    public class CommandFactory
    {
        private readonly RelayFactory _relayFactory;
        private readonly TcpClientGetterService _tcpClientGetter;

        public CommandFactory(TcpClientGetterService tcpClientGetter)
        {
            _relayFactory = new RelayFactory();
            _tcpClientGetter = tcpClientGetter;
        }

        public ICommand Factory(int commandId, int relayId)
        {
            var relayInfo = new Relay
            {
                Id = relayId,
                Name = $"relay_{relayId}",
                NodeNumber = relayId,
                RelayHub = new RelayHub
                {
                    Id = 1,
                    IpAddress = "192.168.1.200",
                    Port = 23,
                    Capacity = 4,
                    RelayHubModel = new RelayHubModel() { Name = "Behsan" },
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

            var tcpClient = _tcpClientGetter.GeTcpClient(relayId);
            if (tcpClient == null)
            {
                throw new Exception($"The relay with id {relayId} is not connected !");
            }

            var relay = _relayFactory.Factory(relayInfo, tcpClient);

            if (relay.RelayInfo.NodeNumber > relay.RelayInfo.RelayHub.Capacity)
                throw new Exception("the relay node number is out of range!");

            return commandId switch
            {
                CommandType.Contact => new Contact(relay: relay),
                CommandType.TurnOn => new TurnOn(relay: relay),
                CommandType.TurnOff => new TurnOff(relay: relay),
                CommandType.FlashOn => new FlashOn(relay: relay),
                CommandType.FlashOff => new FlashOff(relay: relay),
                _ => throw new ArgumentException(message: $"No command matches with the id {commandId}")
            };
        }
    }
}