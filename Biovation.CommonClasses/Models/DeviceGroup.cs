using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccessLayerCore.Attributes;

namespace Biovation.CommonClasses.Models
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
