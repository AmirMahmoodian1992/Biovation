using System;
using Biovation.Services.RelayController.Common;

namespace Biovation.Services.RelayController.Commands
{
    public class TurnOff : ICommand
    {
        public IRelay Relay { get; set; }

        public TurnOff(IRelay relay)
        {
            Relay = relay;
        }

        public object Execute()
        {
         //   if (!Relay.IsConnected())
         //       if (!Relay.Connect())
         //           throw new Exception("connection error !");

            if (!Relay.TurnOff())
                throw new Exception("Contact command failed !");

         //   if (!Relay.Disconnect())
         //       throw new Exception("relay could not disconnect successfully!");

            return 1;
        }
    }
}