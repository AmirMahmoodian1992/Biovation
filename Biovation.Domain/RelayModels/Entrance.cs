using System.Collections.Generic;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain.RelayModels
{
    public class Entrance
    {
        [Id]
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }

        //TODO: DeviceGroup
        [OneToMany]
        public List<Camera> Cameras { get; set; }
        [OneToMany]
        public List<Scheduling> Schedulings { get; set; }
        public string Description { get; set; }
    }
}