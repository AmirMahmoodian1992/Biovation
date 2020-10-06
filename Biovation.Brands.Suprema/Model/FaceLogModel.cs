namespace Biovation.Brands.Suprema.Model
{
    public class FaceLogModel
    {
        
        ///// <summary>
        ///// شماره کاربر در دیتابیس
        ///// </summary>
        ///// <value>شماره کاربر در دیتابیس</value>
        //public int nUserID { get; set; }

        /// <summary>
        /// کد نوع گزارش
        /// </summary>
        /// <value>کد نوع گزارش</value>
        public int EventIdn { get; set; }

        /// <summary>
        /// شماره ی کاربر - شماره پرسنلی
        /// </summary>
        /// <value>شماره کاربر - شماره پرسنلی</value>
        public uint UserID { get; set; }

        /// <summary>
        /// کد دستگاهی که گزارش را ارسال کرده
        /// </summary>
        /// <value>کد دستگاه</value>
        public uint ReaderIdn { get; set; }

        /// <summary>
        /// زمان گزارش
        /// </summary>
        /// <value>زمان گزارش</value>
        public int DateTime { get; set; }

        /// <summary>
        /// عکس فرد به صورت آرایه ای از بایت ها
        /// </summary>
        /// <value>عکس فرد به صورت آرایه ای از بایت ها</value>
        public byte[] Image { get; set; }

        public short Type = 1;

        public uint FaceImageLen { get; set; }
        
    }
}