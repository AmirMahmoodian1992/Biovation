using System;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Services.RelayController.Common;
using Biovation.Services.RelayController.Domain;
using CommandType = Biovation.Services.RelayController.Domain.CommandType;

namespace Biovation.Services.RelayController.Commands
{
    public class TurnOff : ICommand
    {
        public IRelay Relay { get; set; }
        public Lookup _priority { get; set; }

        public TurnOff(IRelay relay, Lookup priority)
        {
            Relay = relay;
            _priority = priority;
        }

        public ResultViewModel Execute( )
        {
            var criteria = new Criteria(Relay.RelayInfo.RelayType, Relay.lastExecutedCommand.Item1);
            var crucialTime = criteria.getCrucialTime();

            foreach (var scheduling in Relay.RelayInfo.Schedulings)
            {
                if (DateTime.Now.TimeOfDay >= scheduling.StartTime & DateTime.Now.TimeOfDay <= scheduling.EndTime & scheduling.Mode.Name == "Open")
                    if (_priority.Code != TaskPriorities.ImmediateCode)
                        return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contact failed !.\nthe command conflicts with the scheduling with scheduling ID: {scheduling.Id}.", Code = 1, Id = Relay.RelayInfo.Id };
            }

            if (Relay.lastExecutedCommand != null && Relay.lastExecutedCommand.Item1 == CommandType.Open && DateTime.Now <= Relay.lastExecutedCommand.Item2.AddSeconds(crucialTime.Seconds) && _priority.Code!=TaskPriorities.ImmediateCode)
                return new ResultViewModel { Validate = 0, Success = false, Message = $"You are not allowed to close Relay Id: {Relay.RelayInfo.Id} ", Code = 1, Id = Relay.RelayInfo.Id };

            if (!Relay.TurnOff())
                return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} turn off command failed !.", Code = 1, Id = Relay.RelayInfo.Id };

            Relay.lastExecutedCommand = Tuple.Create(CommandType.TurnOff, DateTime.Now);
            return new ResultViewModel { Validate = 0, Success = true, Message = $"Relay Id: {Relay.RelayInfo.Id} turned off successfully !.", Code = 1, Id = Relay.RelayInfo.Id };
        }
    }
}