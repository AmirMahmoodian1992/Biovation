using Biovation.Domain;

namespace Biovation.Brands.EOS.Model
{
    /// <summary>
    /// مدل داده های مربوط به اطلاعات گزارش
    /// </summary>
    public class EosLog : Log
    {
        public string RawData { get; set; }

        public override string ToString()
        {
            return $@"UserId:{UserId} DeviceId:{DeviceId} LogTime:{LogDateTime} EventId:55 SubEventId:{SubEvent} RawData:{RawData}";
        }
    }
}
