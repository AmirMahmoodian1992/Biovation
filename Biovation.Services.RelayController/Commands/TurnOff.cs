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
            if (!Relay.IsConnected())
                Relay.Connect();

            try
            {
                Relay.TurnOff();
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