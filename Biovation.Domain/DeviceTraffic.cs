using System;

namespace Biovation.Domain
{
   public class DeviceTraffic
    {
        /// <summary>
        /// شناسه گروه
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// نام گروه
        /// </summary>
        public string SurName { get; set; }
        public int UserId { get; set; }
        public int deviceGroupId { get; set; }
        public uint DeviceId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string DeviceName { get; set; }
        public string TrafficType { get; set; }
        public string IdentityType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        /// <summary>
        /// شماره صفحه ی گرید
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// تعداد رکورد در هر صفحه
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// شرط گرید
        /// </summary>
        public string Where { get; set; }
        /// <summary>
        /// ترتیب سورت گرید
        /// </summary>
        public string Order { get; set; }
        /// <summary>
        /// کد شخص آنلاین
        /// </summary>
        public long OnlineUserId { get; set; }
        ///<summary>
        /// موفق یا ناموفق بودن تردد
        /// </summary>
        public bool? State { get; set; }
        /// <summary>
        /// نوع شناسایی
        /// </summary>
        public Lookup MatchingType { get; set; }
    }
}
