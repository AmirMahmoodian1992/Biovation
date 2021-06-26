using Biovation.Domain.RelayControllerModels;
using Biovation.Services.RelayController.Common;
using Biovation.Services.RelayController.Domain;
using System;
using System.Net.Sockets;

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
        public IRelay Factory(Relay relay)
        {
            return relay.Hub.RelayHubModel.Name switch
            {
                RelayBrands.Behsan => new BehsanRelay(relay),
                _ => throw new ArgumentException(message: $"{relay.Hub.RelayHubModel} is not defined as a relay's brand")
            };
        }
    }
}