using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Domain.DataMappers;

namespace Biovation.Tools.ConnectionMiddleware.Models
{
    public class AttendanceV9
    {
        public AttendanceV9() { }

        public AttendanceV9(Log log)
        {
            foreach (var propertyInfo in typeof(Log).GetProperties())
            {
                if (string.Equals(propertyInfo.Name, MatchingType.GetType().Name, StringComparison.InvariantCultureIgnoreCase))
                    MatchingType = Convert.ToInt32(log.MatchingType?.Code ?? 0.ToString()) % 10;
                
                try { propertyInfo.SetValue(this, propertyInfo.GetValue(log)); }
                catch (Exception) { /*ignore*/ }
            }
        }

        public long Id { get; set; }
        /// <summary>
        /// کد نوع گزارش
        /// </summary>
        /// <value>کد نوع گزارش</value>
        public Lookup EventLog { get; set; }

        /// <summary>
        /// کد دستگاه
        /// </summary>
        public uint DeviceCode { get; set; }
        /// <summary>
        /// شماره ی کاربر - شماره پرسنلی
        /// </summary>
        /// <value>شماره کاربر - شماره پرسنلی</value>
        public long UserId { get; set; }

        /// <summary>
        /// کد دستگاهی که گزارش را ارسال کرده
        /// </summary>
        /// <value>کد دستگاه</value>
        public int DeviceId { get; set; }
        /// <summary>
        /// ردیف
        /// </summary>
        /// <value>کد دستگاه</value>
        public int Total { get; set; }
        /// <summary>
        /// نام دستگاه
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// فلگ تخلیه ی موفقیت آمیز
        /// </summary>
        public bool SuccessTransfer { get; set; }
        /// <summary>
        /// نام فرد
        /// </summary>
        public string SurName { get; set; }
        /// <summary>
        /// زمان
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// زمان گزارش
        /// </summary>
        /// <value>زمان گزارش</value>
        public DateTime LogDateTime
        {
            get => dDateTime;
            set
            {
                dDateTime = value;
                if (nDateTime == 0)
                {
                    var refDate = new DateTime(1970, 1, 1, new GregorianCalendar()).Ticks / 10000000;
                    try
                    {
                        nDateTime = Convert.ToUInt32(value.Ticks / 10000000 - refDate);

                    }
                    catch (Exception)
                    {
                        //Todo fix log
                        //Logger.Log("Date time value of log is too big.");
                        nDateTime = 0;
                    }
                }
            }
        }

        /// <summary>
        /// زمان گزارش
        /// </summary>
        /// <value>زمان گزارش</value>
        private DateTime dDateTime { get; set; }

        /// <summary>
        /// زمان گزارش
        /// </summary>
        /// <value>زمان گزارش</value>
        protected uint nDateTime { get; set; }

        /// <summary>
        /// زمان گزارش
        /// </summary>
        /// <value>زمان گزارش</value>
        public virtual uint DateTimeTicks
        {
            get => nDateTime;
            set
            {
                if (nDateTime != default)
                    return;

                if ((long)value / 10000000 > 0)
                    value /= 10000000;

                nDateTime = value;
                if (LogDateTime == default || LogDateTime < new DateTime(1970, 1, 1) || LogDateTime > DateTime.Now.AddYears(5))
                {
                    LogDateTime = new DateTime(1970, 1, 1, new GregorianCalendar()).AddTicks((long)value * 10000000);
                }
            }
        }

        public Lookup SubEvent { get; set; }

        public ushort TnaEvent { get; set; }
        public int AuthType { get; set; }
        public int AuthResult { get; set; }
        public int MatchingType { get; set; }
        public int InOutMode { get; set; }
        public override string ToString()
        {
            return $@"UserId:{UserId} DeviceId:{DeviceId} DeviceCode:{DeviceCode} Ticks:{DateTimeTicks} LogTime:{LogDateTime} EventId:{EventLog.Code} SubEventId:{SubEvent?.Code ?? 0.ToString()}";
        }

        public string Image { get; set; }
        public string Tumb { get; set; }
        public byte[] PicByte { get; set; }
    }
}
