namespace Biovation.Domain
{
    /// <summary>
    /// مدل داده های مربوط به رویداد ها در زمان متصل نبودن یک دستگاه
    /// </summary>
    public class OfflineEvent
    {
        public int Id { get; set; }
        /// <summary>
        /// کد دستگاه
        /// </summary>
        /// <value>کد دستگاه</value>
        public uint DeviceCode { get; set; }
        public string Data { get; set; }

        public int Type { get; set; }
    }


    public class OfflineEventType
    {
        /// <summary>
        /// مقدار نشان دهنده اضافه شدن کاربر در هنگام متصل نبودن دستگاه
        /// </summary>
        public const int UserInserted = 0;

        /// <summary>
        /// مقدار نشان دهنده حذف شدن کاربر در هنگام متصل نبودن دستگاه
        /// </summary>
        public const int UserDeleted = 1;

        public const int AccessGroupChanged = 2;
        public const int TimeZoneChanged = 3;
    }

}
