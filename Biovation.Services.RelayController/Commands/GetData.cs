using System;
using Biovation.Services.RelayController.Common;

namespace Biovation.Services.RelayController.Commands
{
    public class GetData : ICommand
    {
        public IRelay Relay { get; set; }

        public GetData(IRelay relay)
        {
            Relay = relay;
        }

        public object Execute()
        {
            if (!Relay.IsConnected())
                Relay.Connect();

            string data;
            try
            {
                data = Relay.GetData();
            }
            catch (Exception)
            {
                Relay.Disconnect();
                throw;
            }

            Relay.Disconnect();

            return data;
        }
    }
}