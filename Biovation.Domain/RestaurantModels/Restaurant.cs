using System.Collections.Generic;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain.RestaurantModels
{
    public class Restaurant
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        [OneToMany]
        public List<DeviceBasicInfo> Devices { get; set; }
        public string Description { get; set; }
    }
}
