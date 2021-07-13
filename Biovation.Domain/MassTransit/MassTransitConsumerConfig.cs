using System;
using System.Collections.Generic;
using System.Text;

namespace Biovation.Domain.MassTransit
{
    public class MassTransitConsumerConfig
    {
        public string Name { get; set; }
        public string QueueName { get; set; }
        public int? PrefetchCount { get; set; }
        public bool? Durable { get; set; }
        public bool? AutoDelete { get; set; }
        public Dictionary<string, string> QueueArguments { get; set; }
        public bool? PurgeOnStartup { get; set; }
        public List<MassTransitConsumerBindingConfig> Bindings { get; set; }
    }
}
