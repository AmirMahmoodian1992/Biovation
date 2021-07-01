#nullable enable
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Services.RelayController.Common;
using System;
using System.Linq;
using System.Threading;
using CommandType = Biovation.Services.RelayController.Domain.CommandType;

namespace Biovation.Services.RelayController.Commands
{
    public class Open : ICommand
    {
        private const int WaitTimeInSeconds = 10;
        private readonly Timer _stopTimer;
        public IRelay Relay { get; set; }
        public Lookup _priority { get; set; }

        public Open(IRelay relay, Lookup priority)
        {
            Relay = relay;
            _priority = priority;
            _stopTimer = new Timer(OnTimerStop);
        }

        public ResultViewModel Execute( )
        {
            foreach (var scheduling in Relay.RelayInfo.Schedulings.Where(scheduling => DateTime.Now.TimeOfDay >= scheduling.StartTime & DateTime.Now.TimeOfDay <= scheduling.EndTime &
                scheduling.Mode.Name == "Close").Where(scheduling => _priority.Code != TaskPriorities.ImmediateCode))
                return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contact failed !.\nthe command conflicts with the scheduling with scheduling ID: {scheduling.Id}.", Code = 1, Id = Relay.RelayInfo.Id };

            if (!Relay.TurnOn())
                return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} turned on failed!", Code = 1, Id = Relay.RelayInfo.Id };

            Relay.lastExecutedCommand = Tuple.Create(CommandType.Open, DateTime.Now);
            _stopTimer.Change(TimeSpan.FromSeconds(WaitTimeInSeconds), TimeSpan.Zero);
            return new ResultViewModel { Validate = 0, Success = true, Message = $"Relay Id: {Relay.RelayInfo.Id} turned on successfully.", Code = 1, Id = Relay.RelayInfo.Id };
        }

        private void OnTimerStop(object? state)
        {
            if (Relay.RelayInfo.Schedulings.Any(scheduling => DateTime.Now.TimeOfDay >= scheduling.StartTime & DateTime.Now.TimeOfDay <= scheduling.EndTime & scheduling.Mode.Name == "Open"))
                return;

            Relay.TurnOff();
        }
    }
}