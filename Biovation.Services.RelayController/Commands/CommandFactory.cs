using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Services.RelayController.Common;
using Biovation.Services.RelayController.Services;
using CommandType = Biovation.Services.RelayController.Domain.CommandType;

namespace Biovation.Services.RelayController.Commands
{
    public class CommandFactory
    {
        private readonly GetRelayService _getRelayService;

        public CommandFactory(GetRelayService getRelayService)
        {
            _getRelayService = getRelayService;
        }

        public ResultViewModel<ICommand> Factory(int commandId, int relayId)
        {
            var relay = _getRelayService.GetRelay(relayId);
            if (!relay.Success)
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
                throw new Exception($"The relay with id {relayId} is not connected !");
            }
            var relay = _relayFactory.Factory(relayInfo, tcpClient);

            if (relay.RelayInfo.NodeNumber > relay.RelayInfo.RelayHub.Capacity)
                throw new Exception("the relay node number is out of range!");


            return commandId switch
            {
                CommandType.Contact => new ResultViewModel<ICommand> { Validate = 0, Success = true, Message = $"Command Id :{commandId} is returned.", Data = new Contact(relay.Data), Code = 1, Id = commandId },
                CommandType.TurnOn => new ResultViewModel<ICommand> { Validate = 0, Success = true, Message = $"Command Id :{commandId} is returned.", Data = new TurnOn(relay.Data), Code = 1, Id = commandId },
                CommandType.TurnOff => new ResultViewModel<ICommand> { Validate = 0, Success = true, Message = $"Command Id :{commandId} is returned.", Data = new TurnOff(relay.Data), Code = 1, Id = commandId },
                CommandType.FlashOn => new ResultViewModel<ICommand> { Validate = 0, Success = true, Message = $"Command Id :{commandId} is returned.", Data = new FlashOn(relay.Data), Code = 1, Id = commandId },
                CommandType.FlashOff => new ResultViewModel<ICommand> { Validate = 0, Success = true, Message = $"Command Id :{commandId} is returned.", Data = new FlashOff(relay.Data), Code = 1, Id = commandId },
                _ => new ResultViewModel<ICommand> { Validate = 0, Success = false, Message = $"Command Id :{commandId} not found!.", Data = null, Code = 1, Id = commandId },
            };
        }
    }
}