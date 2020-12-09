using System;
using Biovation.Services.RelayController.Common;

namespace Biovation.Services.RelayController.Commands
{
    public class GetStatus : ICommand
    {
        public IRelay Relay { get; set; }

        public GetStatus(IRelay relay)
        {
            Relay = relay;
        }

        public object Execute()
        {
            if (!Relay.IsConnected())
                Relay.Connect();

            string status;
            try
            {
                status = Relay.GetStatus();
            }
            catch (Exception)
            {
                Relay.Disconnect();
                throw;
            }

            Relay.Disconnect();

            return status;
        }
    }
}