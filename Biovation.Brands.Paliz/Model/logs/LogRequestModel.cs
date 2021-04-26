namespace Biovation.Brands.Paliz.Model.Logs
{
    public class LogRequestModel
    {
        public LogRequestModel() { }
        public LogRequestModel(long userId, long startDate, long endDate, int page)
        {
            UserId = userId;
            StartDate = startDate;
            EndDate = endDate;
            Page = page;
        }

        public long UserId { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        public int Page { get; set; }
    }
}
