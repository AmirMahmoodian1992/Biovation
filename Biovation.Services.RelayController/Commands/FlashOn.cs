using System;
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

        public object Execute()
        {
            if (!Relay.IsConnected())
                if (!Relay.Connect())
                    throw new Exception("connection error !");

            if (!Relay.FlashOn())
                throw new Exception("Contact command failed !");

            if (!Relay.Disconnect())
                throw new Exception("relay could not disconnect successfully!");

            return 1;
        }
    }
}