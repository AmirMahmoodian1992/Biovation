using Biovation.Domain;
using Biovation.Services.RelayController.Common;

namespace Biovation.Services.RelayController.Commands
{
    public class FlashOn : ICommand
    {
        public IRelay Relay { get; set; }

        public FlashOn(IRelay relay)
        {
            Relay = relay;
        }

        public ResultViewModel Execute()
        {
         //   if (!Relay.IsConnected())
         //       if (!Relay.Connect())
         //           throw new Exception("connection error !");

            if (!Relay.FlashOn())
                return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} flash on command failed !.", Code = 1, Id = Relay.RelayInfo.Id };

            //   if (!Relay.Disconnect())
            //       throw new Exception("relay could not disconnect successfully!");

            return new ResultViewModel { Validate = 0, Success = true, Message = $"Relay Id: {Relay.RelayInfo.Id} started flashing successfully.", Code = 1, Id = Relay.RelayInfo.Id };
        }
    }
}