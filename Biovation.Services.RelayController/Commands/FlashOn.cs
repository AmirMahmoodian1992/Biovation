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
                Relay.Connect();

            try
            {
                Relay.FlashOn();
            }
            catch (Exception)
            {
                Relay.Disconnect();
                throw;
            }

            Relay.Disconnect();

            return 1;
        }
    }
}