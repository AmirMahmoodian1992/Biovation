using System.Diagnostics.CodeAnalysis;

namespace Biovation.Brands.Suprema.Model
{
    /// <summary>
    /// مدل داده های مربوط به اطلاعات چهره
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class SupremaFaceModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int ImageType { get; set; }

        /// <summary>
        /// عکس فرد به صورت آرایه ای از بایت ها
        /// </summary>
        /// <value>عکس فرد به صورت آرایه ای از بایت ها</value>
        public byte[] Image { get; set; }

        /// <summary>
        /// اندازه ی داده تصویر
        /// </summary>
        /// <value>اندازه داده های تصویر</value>
        public int ImageLenght { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int TemplateType { get; set; }

        /// <summary>
        /// شماره چهره برای یک فرد
        /// </summary>
        /// <value>شماره چهره برای یک فرد</value>
        public int TemplateIndex { get; set; }

        /// <summary>
        /// نمونه ی چهره به صورت آرایه ی بایت
        /// </summary>
        /// <value>نمونه ی چهره به صورت آرایه ی بایت</value>
        public byte[] Template { get; set; }

        /// <summary>
        /// طول آرایه ی مربوط به نمونه ی چهره
        /// </summary>
        /// <value>طول آرایه ی مربوط به نمونه ی چهره</value>
        public int TemplateLenght { get; set; }

        /// <summary>
        /// CheckSum چهره که مجموع مقادیر تمامی خانه های آرایه ی نمونه ی چهره است
        /// </summary>
        /// <value>CheckSum چهره که مجموع مقادیر تمامی خانه های آرایه ی نمونه ی چهره است</value>
        public int CheckSum { get; set; }


    }
}
