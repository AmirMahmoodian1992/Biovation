using Biovation.Domain;

namespace Biovation.Brands.Suprema.Model
{
    /// <summary>
    /// مدل داده های مربوط به اطلاعات ساعت ها برای استفاده داخل برنامه
    /// </summary>
    public class SupremaDeviceModel : DeviceBasicInfo
    {
        public SupremaDeviceModel() : base() { }

        public SupremaDeviceModel(DeviceBasicInfo device) : base()
        {
            if (device == null)
            {
                return;
            }

            DeviceId = device.DeviceId;
            Code = device.Code;
            Model = device.Model;
            Brand = device.Brand;
            Name = device.Name;
            Active = device.Active;
            IpAddress = device.IpAddress;
            Port = device.Port;
            MacAddress = device.MacAddress;
            RegisterDate = device.RegisterDate;
            HardwareVersion = device.HardwareVersion;
            FirmwareVersion = device.FirmwareVersion;
            DeviceLockPassword = device.DeviceLockPassword;
            SSL = device.SSL;
            TimeSync = device.TimeSync;
            SerialNumber = device.SerialNumber;
        }

        /// <summary>
        /// شماره دستگاه در برنامه پس از اتصال
        /// </summary>
        /// <value>شماره دستگاه در برنامه پس از اتصال</value>
        public int Handle { get; set; }


        public string ConnectionType { get; set; }
    }


    /// <summary>
    /// مدل داده های مربوط به اطلاعات ساعت ها برای انتقال اطلاعات بین برنامه و دیتابیس
    /// </summary>
    //public class SupremaDeviceDataBaseModel
    //{
    //    /// <summary>
    //    /// کد دستگاه
    //    /// </summary>
    //    /// <value>کد دستگاه</value>
    //    public int DeviceId { get; set; }

    //    /// <summary>
    //    /// نام دستگاه
    //    /// </summary>
    //    /// <value>نام دستگاه</value>
    //    public string Name { get; set; }

    //    /// <summary>
    //    /// کد نوع دستگاه
    //    /// </summary>
    //    /// <value>کد نوع دستگاه</value>
    //    public int nDeviceType { get; set; }

    //    /// <summary>
    //    /// IP دستگاه
    //    /// </summary>
    //    /// <value>IP دستگاه</value>
    //    public string sIP { get; set; }

    //    /// <summary>
    //    /// نوع اتصال
    //    /// </summary>
    //    /// <value>نوع اتصال</value>
    //    public int nConnType { get; set; }

    //    /// <summary>
    //    /// نوع اتصال
    //    /// </summary>
    //    /// <value>نوع اتصال</value>
    //    public int nConnTopType { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <value></value>
    //    public int nParentIdn { get; set; }

    //    /// <summary>
    //    /// MAC address مربوط به دستگاه
    //    /// </summary>
    //    /// <value>MAC address مربوط به دستگاه</value>
    //    public string nMacAddress { get; set; }

    //    /// <summary>
    //    /// نام دستگاه، برای شناسایی
    //    /// </summary>
    //    /// <value>نام دستگاه، برای شناسایی</value>
    //    public string sProductName { get; set; }

    //    /// <summary>
    //    /// ورژن بورد استفاده شده روی ساعت
    //    /// </summary>
    //    /// <value>ورژن بورد استفاده شده روی ساعت</value>
    //    public string sBoardVersion { get; set; }

    //    /// <summary>
    //    /// ورژن فرمور استفاده شده روی ساعت
    //    /// </summary>
    //    /// <value>ورژن فرمور استفاده شده روی ساعت</value>
    //    public string sFirmwareVersion { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <value></value>
    //    public string sBlackfinVersion { get; set; }

    //    /// <summary>
    //    /// ورژن کرنل استفاده شده روی ساعت
    //    /// </summary>
    //    /// <value>ورژن کرنل استفاده شده روی ساعت</value>
    //    public string sKernelVersion { get; set; }

    //    /// <summary>
    //    /// منطقه ی زمانی
    //    /// </summary>
    //    /// <value>منطقه ی زمانی</value>
    //    public int nTimezone { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <value></value>
    //    public int nIsBioStar { get; set; }
    //}
}
