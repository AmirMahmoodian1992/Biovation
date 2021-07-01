
using Biovation.Domain;
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
                return new ResultViewModel<ICommand> { Validate = 0, Success = false, Message = $"relay Id :{relayId} is not connected", Data = null, Code = 400, Id = 1 };


            return commandId switch
            {
                CommandType.Contact => new ResultViewModel<ICommand> { Validate = 0, Success = true, Message = $"Command Id :{commandId} is returned.", Data = new Contact(relay.Data), Code = 1, Id = commandId },
                CommandType.TurnOn => new ResultViewModel<ICommand> { Validate = 0, Success = true, Message = $"Command Id :{commandId} is returned.", Data = new TurnOn(relay.Data), Code = 1, Id = commandId },
                CommandType.TurnOff => new ResultViewModel<ICommand> { Validate = 0, Success = true, Message = $"Command Id :{commandId} is returned.", Data = new TurnOff(relay.Data), Code = 1, Id = commandId },
                CommandType.FlashOn => new ResultViewModel<ICommand> { Validate = 0, Success = true, Message = $"Command Id :{commandId} is returned.", Data = new FlashOn(relay.Data), Code = 1, Id = commandId },
                CommandType.FlashOff => new ResultViewModel<ICommand> { Validate = 0, Success = true, Message = $"Command Id :{commandId} is returned.", Data = new FlashOff(relay.Data), Code = 1, Id = commandId },
                CommandType.Open => new ResultViewModel<ICommand> { Validate = 0, Success = true, Message = $"Command Id :{commandId} is returned.", Data = new Open(relay.Data), Code = 1, Id = commandId },
                _ => new ResultViewModel<ICommand> { Validate = 0, Success = false, Message = $"Command Id :{commandId} not found!.", Data = null, Code = 1, Id = commandId },
            };
        }
    }
}