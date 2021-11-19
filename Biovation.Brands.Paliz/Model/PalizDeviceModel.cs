using Biovation.Domain;

namespace Biovation.Brands.Paliz.Model
{
    /// <summary>
    /// مدل داده های مربوط به اطلاعات ساعت ها برای استفاده داخل برنامه
    /// </summary>
    public class PalizDeviceModel : DeviceBasicInfo
    {
        public PalizDeviceModel() : base() { }

        public PalizDeviceModel(DeviceBasicInfo device) : base()
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
}
