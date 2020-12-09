using Biovation.Services.RelayController.Domain;
using Biovation.Services.RelayController.Common;
using System;

namespace Biovation.Services.RelayController.Commands
{
    public class CommandFactory
    {
        public ICommand Factory(int commandId, IRelay relay)
        {
            return commandId switch
            {
                CommandType.Contact => new Contact(relay: relay),
                CommandType.TurnOn => new TurnOn(relay: relay),
                CommandType.TurnOff => new TurnOff(relay: relay),
                CommandType.FlashOn => new FlashOn(relay: relay),
                CommandType.FlashOff => new FlashOff(relay: relay),
                CommandType.GetData => new GetData(relay: relay),
                CommandType.GetStatus => new GetStatus(relay: relay),
                _ => throw new ArgumentException(message:$"No command matches with the id {commandId}")
            };
        }
    }
}