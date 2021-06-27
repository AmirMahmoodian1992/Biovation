using System;
using Biovation.Domain;
using Biovation.Services.RelayController.Common;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Biovation.Services.RelayController.Commands
{
    public class Contact : ICommand
    {
        public IRelay Relay{ get; set; }

        public Contact(IRelay relay)
        {
            Relay = relay;
        }

        public ResultViewModel Execute()
        {
            /*foreach (var scheduling in Relay.RelayInfo.Schedulings)
            {
                if(DateTime.Now.TimeOfDay >= scheduling.StartTime & DateTime.Now.TimeOfDay <= scheduling.EndTime)
                    return new ResultViewModel{ Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contact failed !.\nthe command conflicts with the scheduling.", Code = 1, Id = Relay.RelayInfo.Id };
            }*/

            if (!Relay.Contact())
                return new ResultViewModel {Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contact failed !.", Code = 1, Id = Relay.RelayInfo.Id};

            return new ResultViewModel { Validate = 0, Success = true, Message = $"Relay Id: {Relay.RelayInfo.Id} Contacted successfully.", Code = 1, Id = Relay.RelayInfo.Id };
        }
    }
}