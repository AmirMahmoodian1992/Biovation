using System;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Services.RelayController.Common;

namespace Biovation.Services.RelayController.Commands
{
    public class FlashOff : ICommand
    {
        public IRelay Relay { get; set; }

        public FlashOff(IRelay relay)
        {
            Relay = relay;
        }

        public ResultViewModel Execute(Lookup priority)
        {
            foreach (var scheduling in Relay.RelayInfo.Schedulings)
            {
                if (DateTime.Now.TimeOfDay >= scheduling.StartTime & DateTime.Now.TimeOfDay <= scheduling.EndTime & scheduling.Mode.Name == "Open")
                    if (priority.Code != TaskPriorities.ImmediateCode)
                        return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contact failed !.\nthe command conflicts with the scheduling with scheduling ID: {scheduling.Id}.", Code = 1, Id = Relay.RelayInfo.Id };
            }

            if (!Relay.FlashOff())
                return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} flash off command failed !.", Code = 1, Id = Relay.RelayInfo.Id };

            return new ResultViewModel { Validate = 0, Success = true, Message = $"Relay Id: {Relay.RelayInfo.Id} stopped flashing successfully.", Code = 1, Id = Relay.RelayInfo.Id };
        }
    }
}