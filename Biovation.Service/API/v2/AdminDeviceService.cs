using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.API.v2
{
    public class AdminDeviceService
    {
        private readonly AdminDeviceRepository _adminDeviceRepository;

        public AdminDeviceService(AdminDeviceRepository adminDeviceRepository)
        {
            _adminDeviceRepository = adminDeviceRepository;
        }

        public ResultViewModel<PagingResult<AdminDeviceGroup>> GetAdminDevicesByPersonId(int personId,
            int pageNumber = default, int PageSize = default)
        {
            return _adminDeviceRepository.GetAdminDevicesByPersonId(personId, pageNumber, PageSize);
        }


    }
}
