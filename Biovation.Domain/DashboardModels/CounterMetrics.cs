using System.Collections.Generic;

namespace Biovation.Domain.DashboardModels
{
    public class CounterMetrics
    {
        public int Count { get; set; }
        public List<CountItem> Items { get; set; }
        public string Name { get; set; }
        public TagsItem Tags { get; set; }
        public string Unit { get; set; }
    }
}
