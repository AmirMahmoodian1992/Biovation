﻿using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using Newtonsoft.Json.Linq;

namespace Biovation.Service.Api.v2
{
    public class AdminDeviceService
    {
        private readonly AdminDeviceRepository _adminDeviceRepository;

        public AdminDeviceService(AdminDeviceRepository adminDeviceRepository)
        {
            _adminDeviceRepository = adminDeviceRepository;
        }

        public async Task<ResultViewModel<PagingResult<AdminDevice>>> GetAdminDevicesByUserId(int personId,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return await _adminDeviceRepository.GetAdminDevicesByUserId(personId, pageNumber, pageSize, token);
        }
        public async Task<ResultViewModel<PagingResult<AdminDeviceGroup>>> GetAdminDeviceGroupsByUserId(int personId,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return await _adminDeviceRepository.GetAdminDeviceGroupsByUserId(personId, pageNumber, pageSize, token);
        }

        public async Task<ResultViewModel> ModifyAdminDevice(JObject adminDevice = default, string token = default)
        {
            return await _adminDeviceRepository.ModifyAdminDevice(adminDevice, token);
        }
    }
}
