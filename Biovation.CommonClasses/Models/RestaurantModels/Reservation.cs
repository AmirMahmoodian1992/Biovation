using System;
using DataAccessLayerCore.Attributes;

namespace Biovation.CommonClasses.Models.RestaurantModels
{
    public class Reservation
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
        public Restaurant Restaurant { get; set; }
        public int Count { get; set; }
        public DateTime ReserveTime { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
