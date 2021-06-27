using System;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Services.RelayController.Common;

namespace Biovation.Services.RelayController.Commands
{
    public class TurnOn : ICommand
    {
        public IRelay Relay { get; set; }

        public TurnOn(IRelay relay)
        {
            Relay = relay;
        }

        public ResultViewModel Execute(Lookup priority)
        {
            foreach (var scheduling in Relay.RelayInfo.Schedulings)
            {
                if (DateTime.Now.TimeOfDay >= scheduling.StartTime & DateTime.Now.TimeOfDay <= scheduling.EndTime & scheduling.Mode.Name == "Close")
                    if (priority.Code != TaskPriorities.ImmediateCode)
                        return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contact failed !.\nthe command conflicts with the scheduling with scheduling ID: {scheduling.Id}.", Code = 1, Id = Relay.RelayInfo.Id };
            }

            if (!Relay.TurnOn())
                return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} turned on failed!", Code = 1, Id = Relay.RelayInfo.Id };

            return new ResultViewModel { Validate = 0, Success = true, Message = $"Relay Id: {Relay.RelayInfo.Id} turned on successfully.", Code = 1, Id = Relay.RelayInfo.Id };
        }
    }
}