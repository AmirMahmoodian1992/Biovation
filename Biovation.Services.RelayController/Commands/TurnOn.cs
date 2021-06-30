#nullable enable
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Services.RelayController.Common;
using System;
using System.Linq;
using System.Threading;

namespace Biovation.Services.RelayController.Commands
{
    public class TurnOn : ICommand
    {
        private const int WaitTimeInSeconds = 10;
        private readonly Timer _stopTimer;
        public IRelay Relay { get; set; }

        public TurnOn(IRelay relay)
        {
            Relay = relay;
            _stopTimer = new Timer(OnTimerStop);
        }

        public ResultViewModel Execute(Lookup priority)
        {
            foreach (var scheduling in Relay.RelayInfo.Schedulings.Where(scheduling => DateTime.Now.TimeOfDay >= scheduling.StartTime & DateTime.Now.TimeOfDay <= scheduling.EndTime &
                scheduling.Mode.Name == "Close").Where(scheduling => priority.Code != TaskPriorities.ImmediateCode))
                return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contact failed !.\nthe command conflicts with the scheduling with scheduling ID: {scheduling.Id}.", Code = 1, Id = Relay.RelayInfo.Id };

            if (!Relay.TurnOn())
                return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} turned on failed!", Code = 1, Id = Relay.RelayInfo.Id };

            _stopTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(WaitTimeInSeconds));
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