using System.Collections.Generic;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain.RelayModels
{
    public class RelayHub
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int Capacity { get; set; }
        public bool Active { get; set; }
        [OneToMany]
        public List<Relay> Relays { get; set; }
        [OneToOne]
        public RelayHubModel RelayHubModel { get; set; }
        public string Description { get; set; }
    }

}