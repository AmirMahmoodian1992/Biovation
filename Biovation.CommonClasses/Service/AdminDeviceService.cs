using System.Collections.Generic;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;

namespace Biovation.CommonClasses.Service
{
    public class AdminDeviceService
    {
        /// <summary>
        /// <En>Call a repository method to get all devices from database.</En>
        /// <Fa>با صدا کردن یک تابع در ریپوزیتوری اطلاعات تمامی دستگاه ها را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        public List<AdminDeviceGroup> GetAdminDeviceGroupsByUserId(int userId)
        {
            var adminDeviceRepository = new AdminDeviceRepository();
            return adminDeviceRepository.GetAdminDeviceGroupsByUserId(userId);
        }

        public List<Models.AdminDevice> GetAdminDevicesByUserId(int userId)
        {
            var adminDeviceRepository = new AdminDeviceRepository();
            return adminDeviceRepository.GetAdminDevicesByUserId(userId);
        }

        public ResultViewModel ModifyAdminDevice(string xml)
        {
            var adminDeviceRepository = new AdminDeviceRepository();
            return adminDeviceRepository.ModifyAdminDevice(xml);
        }
    }
}
