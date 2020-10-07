using Biovation.Domain;
using EosClocks;

namespace Biovation.Brands.EOS.Model
{
    //ToDo remove it, first correct the query to match DeviceInfoModel format, then replace it.

    /// <summary>
    /// مدل داده های مربوط به اطلاعات ساعت ها برای استفاده داخل برنامه
    /// </summary>
    public class EosDevice : DeviceBasicInfo
    {
        /// <summary>
        /// شماره دستگاه در برنامه پس از اتصال
        /// </summary>
        /// <value>شماره دستگاه در برنامه پس از اتصال</value>
        public int TRT { get; set; }

        public int DeviceType { get; set; }
        //public int Port { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }
        public int WaitBeforRead { get; set; }
        public string EosDeviceType { get; set; }
        public ProtocolType DeviceProtocolType { get; set; }
        public ProtocolType SensorProtocolType { get; set; }
        
    }


    /// <summary>
    /// مدل داده های مربوط به اطلاعات ساعت ها برای انتقال اطلاعات بین برنامه و دیتابیس
    /// </summary>
    public class EosDeviceDataBaseModel
    {
        /// <summary>
        /// کد دستگاه
        /// </summary>
        /// <value>کد دستگاه</value>
        public int DeviceId;

        /// <summary>
        /// نام دستگاه
        /// </summary>
        /// <value>نام دستگاه</value>
        public string Name;

        /// <summary>
        /// کد نوع دستگاه
        /// </summary>
        /// <value>کد نوع دستگاه</value>
        public int nDeviceType;

        /// <summary>
        /// IP دستگاه
        /// </summary>
        /// <value>IP دستگاه</value>
        public string sIP;

        /// <summary>
        /// نوع اتصال
        /// </summary>
        /// <value>نوع اتصال</value>
        public int nConnType;

        /// <summary>
        /// نوع اتصال
        /// </summary>
        /// <value>نوع اتصال</value>
        public int nConnTopType;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int nParentIdn;

        /// <summary>
        /// MAC address مربوط به دستگاه
        /// </summary>
        /// <value>MAC address مربوط به دستگاه</value>
        public string nMacAddress;

        /// <summary>
        /// نام دستگاه، برای شناسایی
        /// </summary>
        /// <value>نام دستگاه، برای شناسایی</value>
        public string sProductName;

        /// <summary>
        /// ورژن بورد استفاده شده روی ساعت
        /// </summary>
        /// <value>ورژن بورد استفاده شده روی ساعت</value>
        public string sBoardVersion;

        /// <summary>
        /// ورژن فرمور استفاده شده روی ساعت
        /// </summary>
        /// <value>ورژن فرمور استفاده شده روی ساعت</value>
        public string sFirmwareVersion;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string sBlackfinVersion;
        /// <summary>
        /// ورژن کرنل استفاده شده روی ساعت
        /// </summary>
        /// <value>ورژن کرنل استفاده شده روی ساعت</value>
        public string sKernelVersion;
        /// <summary>
        /// منطقه ی زمانی
        /// </summary>
        /// <value>منطقه ی زمانی</value>
        public int nTimezone;
    }

}
