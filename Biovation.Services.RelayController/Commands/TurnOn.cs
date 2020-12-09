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
            if (!Relay.IsConnected())
                Relay.Connect();

            try
            {
                Relay.TurnOn();
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