using System.Collections.Generic;

namespace Biovation.Domain.DashboardModels
{
    public class ContextMetrics
    {
        public List<ApdexScoreMetrics> ApdexScores { get; set; }
        public string Context { get; set; }
        public List<CounterMetrics> Counters { get; set; }
        public List<GaugeMetrics> Gauges { get; set; }
        public List<HistogramMetrics> Histograms { get; set; }
        public List<MeterMetrics> Meters { get; set; }
        public List<TimerMetrics> Timers { get; set; }
    }
}
