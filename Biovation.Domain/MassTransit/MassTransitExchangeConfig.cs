using System.Collections.Generic;

namespace Biovation.Domain.MassTransit
{
    public class MassTransitExchangeConfig
    {
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public bool? Durable { get; set; }
        public Dictionary<string, string> Arguments { get; set; }
    }
}
