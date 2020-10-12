namespace Biovation.Brands.Suprema.Model
{
    /// <summary>
    /// مدل داده های مربوط به رویداد ها در زمان متصل نبودن یک دستگاه
    /// </summary>
    public class SupremaOfflineAccessAndTimeEventModel
    {
        /// <summary>
        /// مقدار نشان دهنده ی نوع آیتم که accessgroup می باشد
        /// </summary>
        public const int OfflineAccessGroup = 0;

        /// <summary>
        /// مقدار نشان دهنده ی نوع آیتم که timezone می باشد
        /// </summary>
        public const int OfflineTimeZone = 1;

        /// <summary>
        /// کد کاربر در دیتابیس
        /// </summary>
        /// <value>کد کاربر در دیتابیس</value>
        public int NItemId { get; set; }

        /// <summary>
        /// کد دستگاه
        /// </summary>
        /// <value>کد دستگاه</value>
        public int NDeviceId { get; set; }

        /// <summary>
        /// کد دستگاه
        /// </summary>
        /// <value>کد دستگاه</value>
        public int NItemType { get; set; }
    }
}
