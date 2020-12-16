using System.Collections.Generic;
using Biovation.Domain;

namespace Biovation.Services.RelayController.Models
{
    public class Entrance
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public List<DeviceBasicInfo> Devices { get; set; }
        //public List<Scheduling> Schedulings { get; set; }
        public string Description { get; set; }
    }
}