using System.Collections.Generic;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain.RelayModels
{
    public class Relay
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        public int NodeNumber { get; set; }
        [OneToOne]
        public RelayHub RelayHub { get; set; }
        [OneToMany]
        public List<Scheduling> Schedulings { get; set; }
        [OneToMany]
        public List<DeviceBasicInfo> Devices { get; set; }
        public string Description { get; set; }
    }
}
