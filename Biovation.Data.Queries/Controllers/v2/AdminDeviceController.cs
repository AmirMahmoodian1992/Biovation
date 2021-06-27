using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class AdminDeviceController : ControllerBase
    {
        private readonly AdminDeviceRepository _adminDeviceRepository;

        public AdminDeviceController(AdminDeviceRepository adminDeviceRepository)
        {
            _adminDeviceRepository = adminDeviceRepository;
        }

        [HttpGet]
        [Authorize]
        [Route("AdminDeviceGroupsByUserId/{personId}")]
        public Task<ResultViewModel<PagingResult<AdminDeviceGroup>>> GetAdminDeviceGroupsByUserId([FromRoute] int personId, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _adminDeviceRepository.GetAdminDeviceGroupsByUserId(personId, pageNumber, pageSize));
        }

        [HttpGet]
        [Authorize]
        [Route("AdminDevicesByUserId/{personId}")]
        public Task<ResultViewModel<PagingResult<AdminDevice>>> GetAdminDevicesByUserId([FromRoute] int personId, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _adminDeviceRepository.GetAdminDevicesByUserId(personId, pageNumber, pageSize));
        }

    }
}