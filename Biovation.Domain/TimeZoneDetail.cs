using System;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class TimeZoneDetail
    {
        [Id]
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
    }
}
