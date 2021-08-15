using DataAccessLayerCore.Attributes;
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

        [OneToOne]
        public User User { get; set; }
        [OneToOne]
        public PlateDetectionLog ParentLog { get; set; }
    }
}