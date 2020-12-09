using System;
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

        public object Execute()
        {
            if (!Relay.IsConnected())
                Relay.Connect();

            try
            {
                Relay.Contact();
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