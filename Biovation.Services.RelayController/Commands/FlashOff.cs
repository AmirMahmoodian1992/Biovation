using System;
using Biovation.Services.RelayController.Common;

namespace Biovation.Services.RelayController.Commands
{
    public class FlashOff : ICommand
    {
        public IRelay Relay { get; set; }

        public FlashOff(IRelay relay)
        {
            Relay = relay;
        }

        public object Execute()
        {
            if (!Relay.IsConnected())
                Relay.Connect();

            try
            {
                Relay.FlashOff();
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