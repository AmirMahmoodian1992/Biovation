using Biovation.Domain;
using Biovation.Repository.API.v2;
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

        public ResultViewModel<PagingResult<AdminDeviceGroup>> GetAdminDevicesByPersonId(int personId,
            int pageNumber = default, int pageSize = default)
        {
            return _adminDeviceRepository.GetAdminDevicesByPersonId(personId, pageNumber, pageSize);
        }

        public ResultViewModel ModifyAdminDevice(JObject adminDevice = default)
        {
            return _adminDeviceRepository.ModifyAdminDevice(adminDevice);
        }

    }
}
