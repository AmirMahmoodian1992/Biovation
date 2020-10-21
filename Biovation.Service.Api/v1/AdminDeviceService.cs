using Biovation.Domain;
using Biovation.Repository.Api.v2;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return _adminDeviceRepository.GetAdminDevicesByUserId(personId, pageNumber, pageSize, token)?.Data?.Data ?? new List<AdminDevice>();
        }
        public List<AdminDeviceGroup> GetAdminDeviceGroupsByUserId(int personId,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return _adminDeviceRepository.GetAdminDeviceGroupsByUserId(personId, pageNumber, pageSize, token)?.Data?.Data ?? new List<AdminDeviceGroup>();
        }

        public ResultViewModel ModifyAdminDevice(JObject adminDevice = default, string token = default)
        {
            return _adminDeviceRepository.ModifyAdminDevice(adminDevice, token);
        }
    }
}
