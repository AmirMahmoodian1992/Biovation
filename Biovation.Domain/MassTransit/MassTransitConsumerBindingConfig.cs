using System.Collections.Generic;

namespace Biovation.Domain.MassTransit
{
    public class MassTransitConsumerBindingConfig
    {
        public string RoutingKey { get; set; }
        public MassTransitExchangeConfig Exchange { get; set; }
        public Dictionary<string, string> Arguments { get; set; }
    }
}
