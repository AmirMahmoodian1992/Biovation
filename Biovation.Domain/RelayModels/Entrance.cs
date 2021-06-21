using System.Collections.Generic;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain.RelayModels
{
    public class Entrance
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        [OneToMany]
        public List<DeviceBasicInfo> Devices { get; set; }
        [OneToMany]
        public List<Scheduling> Schedulings { get; set; }
        public string Description { get; set; }
    }
}