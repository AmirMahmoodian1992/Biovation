using System;
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

        public object Execute()
        {
         //   if (!Relay.IsConnected())
         //       if (!Relay.Connect())
         //           throw new Exception("connection error !");

            if (!Relay.TurnOn())
                throw new Exception("Contact command failed !");

         //   if (!Relay.Disconnect())
         //       throw new Exception("relay could not disconnect successfully!");

            return 1;
        }
    }
}