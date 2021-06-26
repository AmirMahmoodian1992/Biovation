using Biovation.Domain;
using Biovation.Services.RelayController.Common;

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
            //if (!Relay.IsConnected())
            //        if (!Relay.Connect())
            //            throw new Exception("connection error !");

            if (!Relay.Contact())
                return new ResultViewModel {Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contact failed !.", Code = 1, Id = Relay.RelayInfo.Id};

            //if (!Relay.Disconnect())
            //    throw new Exception("relay could not disconnect successfully!");

            return new ResultViewModel { Validate = 0, Success = false, Message = $"Relay Id: {Relay.RelayInfo.Id} Contacted successfully.", Code = 1, Id = Relay.RelayInfo.Id };
        }
    }
}