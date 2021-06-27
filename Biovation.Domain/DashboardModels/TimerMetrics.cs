namespace Biovation.Domain.DashboardModels
{
    public class TimerMetrics
    {
        public int ActiveSessions { get; set; }
        public int Count { get; set; }
        public string DurationUnit { get; set; }
        public HistogramMetrics Histogram { get; set; }
        public Rates Rate { get; set; }
        public string RateUnit { get; set; }
        public string Name { get; set; }
        public TagsItem Tags { get; set; }
        public string Unit { get; set; }
    }
}
