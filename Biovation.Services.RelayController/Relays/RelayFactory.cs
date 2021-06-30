using Biovation.Services.RelayController.Common;
using System;
using Biovation.Constants;
using Biovation.Domain.RelayModels;
using Biovation.Services.RelayController.Domain;

namespace Biovation.Services.RelayController.Relays
{
    public class RelayFactory
    {

        private readonly Lookups _lookups;

        /// <summary>
        /// it creates a IRelay object regarding the relay's company.
        /// attributes are held in the Relay object passed to this function
        /// </summary>
        /// <param name="relay"> a model containing the relay's attributes </param>
        /// <returns> IRelay object </returns>

        public RelayFactory(Lookups lookups)
        {
            _lookups = lookups;
        }

        public IRelay Factory(Relay relay)
        {
            return relay.RelayHub.RelayHubModel.Brand.Code switch
            {
                RelayBrands.Behsan => new BehsanRelay(relay),
                _ => throw new ArgumentException(message: $"{relay.RelayHub.RelayHubModel.Name} is not defined as a relay's brand")
            };

            //return new BehsanRelay(relay);
        }
    }
}