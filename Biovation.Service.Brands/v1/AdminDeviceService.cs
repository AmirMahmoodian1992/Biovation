﻿using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using Newtonsoft.Json.Linq;

namespace Biovation.Service.Api.v1
{
    public class AdminDeviceService
    {
        private readonly AdminDeviceRepository _adminDeviceRepository;

        public AdminDeviceService(AdminDeviceRepository adminDeviceRepository)
        {
            _adminDeviceRepository = adminDeviceRepository;
        }

        public List<AdminDevice> GetAdminDevicesByUserId(int personId,
            int pageNumber = default, int pageSize = default)
        {
            return _adminDeviceRepository.GetAdminDevicesByUserId(personId, pageNumber, pageSize).Data.Data;
        }
        public List<AdminDeviceGroup> GetAdminDeviceGroupsByUserId(int personId,
            int pageNumber = default, int pageSize = default)
        {
            return _adminDeviceRepository.GetAdminDeviceGroupsByUserId(personId, pageNumber, pageSize).Data.Data;
        }

        public ResultViewModel ModifyAdminDevice(JObject adminDevice = default)
        {
            return _adminDeviceRepository.ModifyAdminDevice(adminDevice);
        }

    }
}
