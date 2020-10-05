namespace Biovation.Domain.DashboardModels
{
    public class ApdexScoreMetrics
    {
        public int Frustrating { get; set; }
        public int SampleSize { get; set; }
        public int Satisfied { get; set; }
        public int Score { get; set; }
        public int Tolerating { get; set; }
        public string Name { get; set; }
        public TagsItem Tags { get; set; }
    }
}
