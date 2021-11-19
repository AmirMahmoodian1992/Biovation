#nullable enable
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Services.RelayController.Common;
using System;
using System.Linq;
using System.Threading;
using Biovation.Services.RelayController.Domain;
using CommandType = Biovation.Services.RelayController.Domain.CommandType;

namespace Biovation.Services.RelayController.Commands
{
    public class TurnOn : ICommand
    {
        private const int WaitTimeInSeconds = 10;
        private readonly Timer _stopTimer;
        public IRelay Relay { get; set; }
        public Lookup _priority { get; set; }

        public TurnOn(IRelay relay, Lookup priority)
        {
            Relay = relay;
            _priority = priority;
        }

        public ResultViewModel Execute()
        {
            if (Relay.lastExecutedCommand != null)
            {
                var criteria = new Criteria(Relay.RelayInfo.RelayType, Relay.lastExecutedCommand.Item1);
                var crucialTime = criteria.getCrucialTime();
                if (Relay.lastExecutedCommand != null && Relay.lastExecutedCommand.Item1 == CommandType.Open &&
                    DateTime.Now <= Relay.lastExecutedCommand.Item2.AddSeconds(crucialTime.Seconds) &&
                    _priority.Code != TaskPriorities.ImmediateCode)
                    return new ResultViewModel
                    {
                        Validate = 0,
                        Success = false,
                        Message = $"You are not allowed to close Relay Id: {Relay.RelayInfo.Id} ",
                        Code = 1,
                        Id = Relay.RelayInfo.Id
                    };
            }

            foreach (var scheduling in Relay.RelayInfo.Schedulings.Where(scheduling => DateTime.Now.TimeOfDay >= scheduling.StartTime & DateTime.Now.TimeOfDay <= scheduling.EndTime &
                scheduling.Mode.Name == "Close").Where(scheduling => _priority.Code != TaskPriorities.ImmediateCode))
                return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contact failed !.\nthe command conflicts with the scheduling with scheduling ID: {scheduling.Id}.", Code = 1, Id = Relay.RelayInfo.Id };


            if (Relay.RelayInfo?.Entrance?.Schedulings != null)
                foreach (var entranceScheduling in Relay.RelayInfo.Entrance.Schedulings)
                {
                    if (DateTime.Now.TimeOfDay >= entranceScheduling.StartTime & DateTime.Now.TimeOfDay <= entranceScheduling.EndTime & entranceScheduling.Mode.Name == "Close")
                        if (_priority.Code != TaskPriorities.ImmediateCode)
                            return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contact failed !.\nthe command conflicts with the scheduling with scheduling ID: {entranceScheduling.Id}.", Code = 1, Id = Relay.RelayInfo.Id };
                }



                if (!Relay.TurnOn())
                return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} turned on failed!", Code = 1, Id = Relay.RelayInfo.Id };

            Relay.lastExecutedCommand = Tuple.Create(CommandType.TurnOn, DateTime.Now);
            return new ResultViewModel { Validate = 0, Success = true, Message = $"Relay Id: {Relay.RelayInfo.Id} turned on successfully.", Code = 1, Id = Relay.RelayInfo.Id };
        }

    }
}