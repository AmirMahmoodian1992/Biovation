using System.Diagnostics.CodeAnalysis;

namespace Biovation.Brands.Suprema.Model
{
    /// <summary>
    /// مدل داده های مربوط به اطلاعات اثر انگشت
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class SupremaFingerModel
    {
        /// <summary>
        /// شماره نمونه اثر انگشت ثبت شده برای هر فرد است
        /// </summary>
        /// <value>شماره نمونه ی ثبت شده برای هر فرد است</value>
        public short nIndex { get; set; }

        /// <summary>
        /// نمونه ی اثر انگشت به صورت آرایه ای از بایت
        /// </summary>
        /// <value>نمونه ی اثر انگشت به صورت byte</value>
        public byte[] bTemplate { get; set; }

        /// <summary>
        /// CheckSum هر اثر انگشت که جمع تمامی خانه های آرایی ی بایتی اثر انگشت است
        /// </summary>
        /// <value>CheckSum هر اثر انگشت که جمع تمامی خانه های آرایی ی بایتی اثر انگشت است</value>
        public int nTemplatecs { get; set; }

        /// <summary>
        /// شماره ی انگشت فرد
        /// </summary>
        /// <value>شماره ی انگشت فرد</value>
        public int nFingerIndex { get; set; }

        /// <summary>
        /// سطح امنیت است که در داکیومنت biostar به شرح آمده
        /// </summary>
        /// <value>سطح امنیت</value>
        public int nSecurityLevel { get; set; }

        /// <summary>
        /// لزوم یا عدم نیاز حتمی دریافت اثر این اثر انگشت حتی بعد از شناسایی فرد با روش دیگری را تعیین می کند
        /// </summary>
        /// <value>The duress.</value>
        public short nDuress { get; set; }

        /// <summary>
        /// برای هر اثر انگشت، دو داده دریافت می شود که این مفدار شماره ی آن ها را مشخص می کند.
        /// </summary>
        /// <value>شماره ی نمونه ی گرفته شده برای یک انگشت</value>
        public short nTemplateIndex { get; set; }

        /// <summary>
        /// کیفیت اثر انگشت بر اساس عدد بین 0 تا 100
        /// </summary>
        public int nEnrollQuality { get; set; }
    }
}
