using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable All

namespace Biovation.Brands.Suprema.Model
{
    /// <summary>
    /// مدل داده های مربوط به اطلاعات تایم زون ها
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SupremaTimeZoneModel
    {

        /// <summary>
        /// شماره تایم زون در دیتابیس
        /// </summary>
        /// <value>شماره گروه در دیتابیس</value>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int ScheduleId { get; set; }

        /// <summary>
        /// نام تایم زون
        /// </summary>
        /// <value>نام تایم زون</value>
        public string Name { get; set; }

        /// <summary>
        /// شماره تعطیلی
        /// </summary>
        /// <value>شماره تعطیلی</value>
        public int HolidayIdn { get; set; }

        /// <summary>
        /// شماره محدوه ساعت
        /// </summary>
        /// <value>شماره محدوه ساعت</value>
        public int TimeCode { get; set; }

        /// <summary>
        /// بازه های ساعت
        /// </summary>
        /// <value>بازه های ساعت</value>
        public int StartTime1 { get; set; }
        public int EndTime1 { get; set; }
        public int StartTime2 { get; set; }
        public int EndTime2 { get; set; }
        public int StartTime3 { get; set; }
        public int EndTime3 { get; set; }
        public int StartTime4 { get; set; }
        public int EndTime4 { get; set; }
        public int StartTime5 { get; set; }
        public int EndTime5 { get; set; }
    }
}
