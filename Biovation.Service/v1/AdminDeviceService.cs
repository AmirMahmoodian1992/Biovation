﻿using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.Sql.v1;

namespace Biovation.Service.Sql.v1
{
    public class AdminDeviceService
    {
        private readonly AdminDeviceRepository _adminDeviceRepository;

        public AdminDeviceService(AdminDeviceRepository adminDeviceRepository)
        {
            _adminDeviceRepository = adminDeviceRepository;
        }

        /// <summary>
        /// <En>Call a repository method to get all devices from database.</En>
        /// <Fa>با صدا کردن یک تابع در ریپوزیتوری اطلاعات تمامی دستگاه ها را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        public List<AdminDeviceGroup> GetAdminDeviceGroupsByUserId(int userId)
        {
            return _adminDeviceRepository.GetAdminDeviceGroupsByUserId(userId);
        }

        public List<AdminDevice> GetAdminDevicesByUserId(int userId)
        {
            return _adminDeviceRepository.GetAdminDevicesByUserId(userId);
        }

        public ResultViewModel ModifyAdminDevice(string xml)
        {
            return _adminDeviceRepository.ModifyAdminDevice(xml);
        }
    }
}
