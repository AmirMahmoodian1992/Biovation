using System.Diagnostics.CodeAnalysis;

namespace Biovation.Brands.Suprema.Model
{
    /// <summary>
    /// مدل داده های مربوط به اطلاعات کارت
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class SupremaCardModel
    {
        /// <summary>
        /// کد یا شماره رکورد کارت در دیتابیس
        /// </summary>
        /// <value>کد یا شماره رکورد کارت در دیتابیس</value>
        public int NCardIdn { get; set; }

        /// <summary>
        /// کد کاربر در دیتابیس
        /// </summary>
        /// <value>کد کاربر در دیتابیس</value>
        public int NUserIdn { get; set; }

        /// <summary>
        /// شماره کارت
        /// </summary>
        /// <value>شماره کارت</value>
        public string SCardNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public short NBypass { get; set; }
        /// <summary>
        /// شماره کاربر - شماره پرسنلی
        /// </summary>
        /// <value>شماره کاربر - شماره پرسنلی</value>
        public string SUserId { get; set; }
        /// <summary>
        /// کد اختیاری برای ثبت کارت
        /// </summary>
        /// <value>کد اختیاری برای ثبت کارت</value>
        public string SCustomNo { get; set; }
    }
}
