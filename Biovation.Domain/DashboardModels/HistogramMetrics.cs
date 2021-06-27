namespace Biovation.Domain.DashboardModels
{
    public class HistogramMetrics
    {
        public float LastValue { get; set; }
        public float Max { get; set; }
        public float Mean { get; set; }
        public float Median { get; set; }
        public float Min { get; set; }
        public float Percentile75 { get; set; }
        public float Percentile95 { get; set; }
        public float Percentile98 { get; set; }
        public float Percentile99 { get; set; }
        public float Percentile999 { get; set; }
        public int SampleSize { get; set; }
        public float StdDev { get; set; }
        public float Sum { get; set; }
    }
}
