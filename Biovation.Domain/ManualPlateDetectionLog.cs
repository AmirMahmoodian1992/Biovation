using System;

namespace Biovation.Domain
{
    public class ManualPlateDetectionLog : PlateDetectionLog
    {
        public ManualPlateDetectionLog() { }

        public ManualPlateDetectionLog(PlateDetectionLog log)
        {
            foreach (var propertyInfo in typeof(PlateDetectionLog).GetProperties())
            {
                try { propertyInfo.SetValue(this, propertyInfo.GetValue(log)); }
                catch (Exception) { /*ignore*/ }
            }
        }

        public User User { get; set; }
        public PlateDetectionLog ParentLog { get; set; }
    }
}