namespace Biovation.Domain
{
    public class ManualPlateDetectionLog : PlateDetectionLog
    {
        public User User { get; set; }
        public PlateDetectionLog ParentLog { get; set; }
    }
}