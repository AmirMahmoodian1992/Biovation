﻿using System;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Services.RelayController.Common;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TimeZone = Biovation.Domain.TimeZone;
using CommandType = Biovation.Services.RelayController.Domain.CommandType;

namespace Biovation.Services.RelayController.Commands
{
    public class Contact : ICommand
    {
        public IRelay Relay{ get; set; }

        public Contact(IRelay relay)
        {
            Relay = relay;
        }

        public ResultViewModel Execute(Lookup priority)
        {
            foreach (var scheduling in Relay.RelayInfo.Schedulings)
            {
                if(DateTime.Now.TimeOfDay >= scheduling.StartTime & DateTime.Now.TimeOfDay <= scheduling.EndTime & scheduling.Mode.Name=="Close")
                    if (priority.Code != TaskPriorities.ImmediateCode)
                        return new ResultViewModel{ Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contact failed !.\nthe command conflicts with the scheduling with scheduling ID: {scheduling.Id}.", Code = 1, Id = Relay.RelayInfo.Id };
            }

            if (!Relay.Contact())
                return new ResultViewModel {Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contact failed !.", Code = 1, Id = Relay.RelayInfo.Id};

            Relay.lastExecutedCommand = Tuple.Create(CommandType.Contact, DateTime.Now);
            return new ResultViewModel { Validate = 0, Success = true, Message = $"Relay Id: {Relay.RelayInfo.Id} Contacted successfully.", Code = 1, Id = Relay.RelayInfo.Id };
        }
    }
}