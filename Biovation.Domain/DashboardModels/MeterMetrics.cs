using System.Collections.Generic;

namespace Biovation.Domain.DashboardModels
{
    public class MeterMetrics
    {
        public int Count { get; set; }
        public float FifteenMinuteRate { get; set; }
        public float FiveMinuteRate { get; set; }
        public List<object> Items { get; set; }
        public float MeanRateMin { get; set; }
        public float OneMinuteRate { get; set; }
        public string RateUnit { get; set; }
        public string Name { get; set; }
        public TagsItem Tags { get; set; }
        public string Unit { get; set; }
    }
}
