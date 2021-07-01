﻿using System;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Services.RelayController.Common;
using CommandType = Biovation.Services.RelayController.Domain.CommandType;

namespace Biovation.Services.RelayController.Commands
{
    public class TurnOff : ICommand
    {
        public IRelay Relay { get; set; }

        public TurnOff(IRelay relay)
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

            if (Relay.lastExecutedCommand != null && Relay.lastExecutedCommand.Item1 == CommandType.Open && DateTime.Now <= Relay.lastExecutedCommand.Item2.AddSeconds(10) && priority.Code!=TaskPriorities.ImmediateCode)
                return new ResultViewModel { Validate = 0, Success = false, Message = $"You are not allowed to close Relay Id: {Relay.RelayInfo.Id} ", Code = 1, Id = Relay.RelayInfo.Id };

            if (!Relay.TurnOff())
                return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} turn off command failed !.", Code = 1, Id = Relay.RelayInfo.Id };

            Relay.lastExecutedCommand = Tuple.Create(CommandType.TurnOff, DateTime.Now);
            return new ResultViewModel { Validate = 0, Success = true, Message = $"Relay Id: {Relay.RelayInfo.Id} turned off successfully !.", Code = 1, Id = Relay.RelayInfo.Id };
        }
    }
}