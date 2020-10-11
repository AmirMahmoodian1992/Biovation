using Biovation.Domain;

namespace Biovation.Brands.Suprema.Model
{
    /// <summary>
    /// مدل داده های مربوط به رویداد ها در زمان متصل نبودن یک دستگاه
    /// </summary>
    public class SupremaOfflineUserEventModel : OfflineEvent
    {


        /// <summary>
        /// کد کاربر در دیتابیس
        /// </summary>
        /// <value>کد کاربر در دیتابیس</value>
        public int NUserIdn { get; set; }

        ///// <summary>
        ///// کد دستگاه
        ///// </summary>
        ///// <value>کد دستگاه</value>
        //public int NDeviceId { get; set; }

        /// <summary>
        /// مقدار نشان دهنده اضافه شدن یا حذف شدن کاربر برای انتقال به دستگاه
        /// </summary>
        /// <value>مقدار نشان دهنده نوع اتفاق  برای انتقال به دستگاه</value>
        public int NDeviceUpdate { get; set; }
    }
}
