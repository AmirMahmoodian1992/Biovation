using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Biovation.Domain.DataMappers;
using DataAccessLayerCore.Attributes;
using Newtonsoft.Json;

namespace Biovation.Domain
{
    /// <summary>
    /// مدل داده های مربوط به اطلاعات گزارش
    /// </summary>
    //[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Log
    {
        public Log() {}

        public Log(Log log)
        {
            foreach (var propertyInfo in typeof(Log).GetProperties())
            {
                try { propertyInfo.SetValue(this, propertyInfo.GetValue(log)); }
                catch (Exception) { /*ignore*/ }
            }
        }

        [Id]
        public long Id { get; set; }
        /// <summary>
        /// کد نوع گزارش
        /// </summary>
        /// <value>کد نوع گزارش</value>
        [OneToOne]
        public Lookup EventLog { get; set; }

        /// <summary>
        /// کد دستگاه
        /// </summary>
        [DataMapper(Mapper = typeof(BigIntToUIntMapper))]
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
        [DataMapper(Mapper = typeof(BigIntToUIntMapper))]
        public virtual uint DateTimeTicks
        {
            get => nDateTime;
            set
            {
                if (nDateTime != default)
                    return;
                
                if ((long)value / 10000000000 > 0)
                    value /= 10000000;

                nDateTime = value;
                if (LogDateTime == default || LogDateTime < new DateTime(1970, 1, 1) || LogDateTime > DateTime.Now.AddYears(5))
                {
                    LogDateTime = new DateTime(1970, 1, 1, new GregorianCalendar()).AddTicks((long)value * 10000000);
                }
            }
        }

        [OneToOne]
        public Lookup SubEvent { get; set; }

        [DataMapper(Mapper = typeof(IntToUshortMapper))]
        public ushort TnaEvent { get; set; }
        public int AuthType { get; set; }
        public int AuthResult { get; set; }
        [OneToOne]
        public Lookup MatchingType { get; set; }

        //[JsonProperty("DeviceIOType")]
        public int InOutMode
        {
            get => DeviceIOType;
            set
            {
                DeviceIOType = value;
            }
        }

        //[JsonProperty("InOutMode")]
        public int DeviceIOType { get; set; }
        public override string ToString()
        {
            return $@"UserId:{UserId} DeviceId:{DeviceId} DeviceCode:{DeviceCode} Ticks:{DateTimeTicks} LogTime:{LogDateTime} EventId:{EventLog.Code} SubEventId:{SubEvent?.Code ?? 0.ToString()}";
        }

        public string Image { get; set; }
        public string Tumb { get; set; }
        public byte[] PicByte { get; set; }
    }
}
