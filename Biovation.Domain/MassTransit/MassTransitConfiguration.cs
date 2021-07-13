using System.Collections.Generic;

namespace Biovation.Domain.MassTransit
{
    public class MassTransitConfiguration
    {
        public MassTransitHostConfig Host { get; set; }
        public List<MassTransitReceiveEndpointConfig> ReceiveEndpoints { get; set; }
    }
}
