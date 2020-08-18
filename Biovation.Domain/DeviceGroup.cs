using System.Collections.Generic;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class DeviceGroup
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [OneToMany]
        public List<DeviceBasicInfo> Devices { get; set; }
    }
}
