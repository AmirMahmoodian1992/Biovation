﻿using System;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain.RestaurantModels
{
    public class MealTiming
    {
        [Id]
        public int Id { get; set; }
        [OneToOne]
        public Meal Meal { get; set; }
        [OneToOne]
        public DeviceBasicInfo Device { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StartTimeInMinutes { get; set; }
        public int EndTimeInMinutes { get; set; }
    }
}
