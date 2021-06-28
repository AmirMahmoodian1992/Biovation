using System;
using System.Globalization;
using Biovation.Domain.DataMappers;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class PlateDetectionLog
    {
        [Id]
        public long Id { get; set; }
        public int DetectorId { get; set; }
        public string DeviceName { get; set; }
        [OneToOne]
        public Lookup EventLog { get; set; }
        [OneToOne]
        public LicensePlate LicensePlate { get; set; }
        public DateTime LogDateTime
        {
            get => DDateTime;
            set
            {
                DDateTime = value;
                if (NDateTime == 0)
                {
                    var refDate = new DateTime(1970, 1, 1, new GregorianCalendar()).Ticks / 10000000;
                    try
                    {
                        NDateTime = Convert.ToUInt32(value.Ticks / 10000000 - refDate);

                    }
                    catch (Exception)
                    {
                        //Todo fix log
                        //Logger.Log("Date time value of log is too big.");
                        NDateTime = 0;
                    }
                }
            }
        }

        /// <summary>
        /// زمان گزارش
        /// </summary>
        /// <value>زمان گزارش</value>
        private DateTime DDateTime { get; set; }

        /// <summary>
        /// زمان گزارش
        /// </summary>
        /// <value>زمان گزارش</value>
        protected uint NDateTime { get; set; }

        /// <summary>
        /// زمان گزارش
        /// </summary>
        /// <value>زمان گزارش</value>
        [DataMapper(Mapper = typeof(BigIntToUIntMapper))]
        public virtual uint DateTimeTicks
        {
            get => NDateTime;
            set
            {
                if ((long)value / 10000000 > 0)
                    value /= 10000000;

                NDateTime = value;
                if (LogDateTime == default || LogDateTime < new DateTime(1970, 1, 1) || LogDateTime > DateTime.Now.AddYears(5))
                {
                    LogDateTime = new DateTime(1970, 1, 1, new GregorianCalendar()).AddTicks((long)value * 10000000);
                }
            }
        }
        public string UrlImage { get; set; }
        public byte[] FullImage { get; set; }
        public byte[] PlateImage { get; set; }
        public int InOrOut { get; set; }
        public float DetectionPrecision { get; set; }
        public bool SuccessTransfer { get; set; }
        public int Total { get; set; }
    }
}