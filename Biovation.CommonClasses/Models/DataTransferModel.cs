using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Biovation.CommonClasses.Models
{ 
    /// <summary>
    /// مدل داده های مربوط به یک اتفاق، برای انتقال از کلاینت به سرور
    /// </summary>
    public class DataTransferModel
    {
        /// <summary>
        /// کد اتفاق
        /// </summary>
        /// <value>کد اتفاق</value>
        public int EventId { get; set; }

        /// <summary>
        /// شماره کاربر در دیتابیس
        /// </summary>
        /// <value>شماره کاربر در دیتابیس</value>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public List<object> Items {  get; set; }

        /// <summary>
        /// برند دستگاه
        /// </summary>
        /// <value>برند دستگاه</value>
        public string ClientName { get; set; }


        public override string ToString()
        {
            var data = "EventId: " + EventId + ", Items: [";

            foreach (var item in Items)
            {
                data += item + ", ";
            }

            data = data.Remove(data.Length - 1);
            data += "] ClientName: " + ClientName + "/n";

            return data;
        }
    }
}
