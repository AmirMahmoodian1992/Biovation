using Biovation.CommonClasses.DataMappers;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Models.RestaurantModels;
using DataAccessLayer.Attributes;
using System;
using System.Linq;

namespace Biovation.CommonClasses.Models
{

    /// <summary>
    /// مدل داده های مربوط به فقط شماره ساعت برای دریافت از بانک
    /// </summary>
    public class DeviceBasicInfo
    {
        public DeviceBasicInfo() { }

        public DeviceBasicInfo(DeviceBasicInfo baseDeviceBasicInfo)
        {
            foreach (var propertyInfo in typeof(DeviceBasicInfo).GetProperties())
            {
                try { propertyInfo.SetValue(this, propertyInfo.GetValue(baseDeviceBasicInfo)); }
                catch (Exception) { /*ignore*/ }
            }
        }

        /// <summary>
        /// آیدی دستگاه
        /// </summary>
        /// <value>کد دستگاه</value>
        [Id]
        public int DeviceId { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        [DataMapper(Mapper = typeof(ToUIntMapper))]
        public uint Code { get; set; }
        public int ModelId
        {
            get => Model?.Id ?? 0;
            set
            {
                if (Model == null)
                {
                    Model = new DeviceModel
                    {
                        Id = value
                    };
                }
                else
                {
                    Model.Id = value;
                }
            }
        }

        /// <summary>
        /// کد نوع دستگاه
        /// </summary>
        /// <value>کد نوع دستگاه</value>
        [OneToOne]
        public DeviceModel Model { get; set; }
        [OneToOne]
        public Lookup Brand { get; set; }

        /*
        public string BrandId
        {

            get => Brand?.Id ?? "0";
            set => Brand = DeviceBrands.Brands.FirstOrDefault(brand => string.Equals(brand.Id, value));
        }
        public string BrandName
        {
            get => Brand?.Name ?? string.Empty;
            set => Brand = DeviceBrands.Brands.FirstOrDefault(brand => string.Equals(brand.Id, value));
        }*/
        /// <summary>
        /// نوع دستگاه
        /// </summary>
        /// <value>نوع دستگاه</value>

        [OneToOne]
        public Restaurant Restaurant { get; set; }

        /// <summary>
        /// نام دستگاه
        /// </summary>
        /// <value>نام دستگاه</value>
        public string Name { get; set; }

        /// <summary>
        /// فعال یا غیر فعال بودن دستگاه
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// IP دستگاه
        /// </summary>
        /// <value>IP دستگاه</value>
        public string IpAddress { get; set; }

        /// <summary>
        /// پورت اتصال دستگاه
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// آدرس فیزیکی دستگاه
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// زمان تعریف اولیه دستگاه
        /// </summary>
        public DateTime RegisterDate { get; set; }

        /// <summary>
        /// ورژن سخت افزار دستگاه
        /// </summary>
        public string HardwareVersion { get; set; }

        /// <summary>
        /// ورژن نرم افزار دستگاه
        /// </summary>
        public string FirmwareVersion { get; set; }
        public int DeviceTypeId { get; set; }
        /// <summary>
        /// رمز دستگاه در صورت قفل بودن
        /// </summary>
        public string DeviceLockPassword { get; set; }

        /// <summary>
        /// اتصال با امنست SSL
        /// </summary>
        public bool SSL { get; set; }

        /// <summary>
        /// همسان سازی ساعت و تاریخ دستگاه با سرور در زمان هر اتصال
        /// </summary>
        public bool TimeSync { get; set; }

        /// <summary>
        /// سریال دستگاه
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// کد دستگاه موجو در sdk
        /// </summary>
        public int ManufactureCode { get; set; }
        public string JsonProperty { get; set; }
    }
}