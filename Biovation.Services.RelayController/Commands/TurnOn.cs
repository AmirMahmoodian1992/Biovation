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

        public ResultViewModel Execute()
        {
         //   if (!Relay.IsConnected())
         //       if (!Relay.Connect())
         //           throw new Exception("connection error !");

            if (!Relay.TurnOn())
                return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} turned on failed!", Code = 1, Id = Relay.RelayInfo.Id };

            //   if (!Relay.Disconnect())
            //       throw new Exception("relay could not disconnect successfully!");

            return new ResultViewModel { Validate = 0, Success = true, Message = $"Relay Id: {Relay.RelayInfo.Id} turned on successfully.", Code = 1, Id = Relay.RelayInfo.Id };
        }
    }
}