using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;
using System.Collections.Generic;

namespace Biovation.Brands.Virdi.Service
{
    /// <summary>
    /// کلاس مربوط به ایجاد امکان استفاده از یک ریپوزیتوری برای انتقال داده های ساعت ها به دیتابیس
    /// </summary>
    public class VirdiDeviceService
    {
        private readonly DeviceRepository _deviceRepository;

        public VirdiDeviceService(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        /// <summary>
        /// <En>Call a repository method to get all devices from database.</En>
        /// <Fa>با صدا کردن یک تابع در ریپوزیتوری اطلاعات تمامی دستگاه ها را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        public List<DeviceBasicInfo> GetAllDevicesBasicInfos()
        {
            return _deviceRepository.GetAllDevicesBasicInfos();
        }

        /// <summary>
        /// <En>Call a repository method to get the devices info from database.</En>
        /// <Fa>با صدا کردن یک تابع در ریپوزیتوری اطلاعات یک دستگاه را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="deviceId">کد ساعت</param>
        /// <returns></returns>
        public DeviceBasicInfo GetDeviceInfo(int deviceId)
        {
            return _deviceRepository.GetDeviceBasicInfo(deviceId);
        }
    }
}
