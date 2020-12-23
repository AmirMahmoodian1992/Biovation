using System.Collections.Generic;

namespace Biovation.Domain.RelayControllerModels
{
    public class Entrance
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DeviceBasicInfo> Devices { get; set; }
        public List<Scheduling> Schedulings { get; set; }
        public string Description { get; set; }
    }
}