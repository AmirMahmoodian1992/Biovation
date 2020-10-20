using System.Diagnostics.CodeAnalysis;

// ReSharper disable All

namespace Biovation.Brands.Suprema.Model
{
    /// <summary>
    /// مدل داده های مربوط به اطلاعات گروه های دسترسی
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SupremaAccessGroupModel
    {
        /// <summary>
        /// شماره گروه در دیتابیس
        /// </summary>
        /// <value>شماره گروه در دیتابیس</value>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int GroupMaskId { get; set; }

        /// <summary>
        /// کد کاربر - شماره پرسنلی
        /// </summary>
        /// <value>کد کاربر - شماره پرسنلی</value>
        public int UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int AccessGroupType { get; set; }

        /// <summary>
        /// نام گروه دسترسی
        /// </summary>
        /// <value>نام گروه دسترسی</value>
        public string Name { get; set; }

        /// <summary>
        /// کد دستگاه
        /// </summary>
        /// <value>کد دستگاه</value>
        public int DeviceId { get; set; }

        /// <summary>
        /// کد گروه زمانی
        /// </summary>
        /// <value>کد گروه زمانی</value>
        public int nTimezone { get; set; }
    }
}
