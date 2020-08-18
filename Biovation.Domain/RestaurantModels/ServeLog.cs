using System;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain.RestaurantModels
{
    public class ServeLog
    {
        [Id]
        public int Id { get; set; }
        [OneToOne]
        public User User { get; set; }
        [OneToOne]
        public Food Food { get; set; }
        [OneToOne]
        public Meal Meal { get; set; }
        [OneToOne]
        public ServeLogStatus Status { get; set; }
        [OneToOne]
        public DeviceBasicInfo Device { get; set; }
        public int Count { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsSynced { get; set; }
    }
}
