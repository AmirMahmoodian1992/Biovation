using Biovation.Services.RelayController.Common;
using Biovation.Services.RelayController.Domain;
using System;
using System.Net.Sockets;
using Biovation.Domain.RelayModels;

namespace Biovation.Services.RelayController.Relays
{
    public class RelayFactory
    {
        /// <summary>
        /// it creates a IRelay object regarding the relay's company.
        /// attributes are held in the Relay object passed to this function
        /// </summary>
        /// <param name="relay"> a model containing the relay's attributes </param>
        /// <param name="tcpClient"></param>
        /// <returns> IRelay object </returns>
        public IRelay Factory(Relay relay, TcpClient tcpClient)
        {
            return relay.RelayHub.RelayHubModel.Name switch
            {
                RelayBrands.Behsan => new BehsanRelay(relay, tcpClient),
                _ => throw new ArgumentException(message: $"{relay.RelayHub.RelayHubModel} is not defined as a relay's brand")
            };
        }
    }
}