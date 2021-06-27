namespace Biovation.Domain.DashboardModels
{
    public class GaugeMetrics
    {
        public string Value { get; set; }
        public string Name { get; set; }
        public TagsItem Tags { get; set; }
        public string Unit { get; set; }
    }
}
