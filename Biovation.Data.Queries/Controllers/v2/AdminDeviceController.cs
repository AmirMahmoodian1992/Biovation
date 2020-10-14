using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biovation.Data.Queries.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class AdminDeviceController : Controller
    {
        //private readonly CommunicationManager<DeviceBasicInfo> _communicationManager = new CommunicationManager<DeviceBasicInfo>();

        private readonly AdminDeviceRepository _adminDeviceRepository;

        public AdminDeviceController(AdminDeviceRepository adminDeviceRepository)
        {
            _adminDeviceRepository = adminDeviceRepository;
        }

        [HttpGet]
        [Route("AdminDeviceGroupsByUserId/{personId}")]
        [Authorize]

        public Task<ResultViewModel<PagingResult<AdminDeviceGroup>>> GetAdminDeviceGroupsByUserId(int personId, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _adminDeviceRepository.GetAdminDeviceGroupsByUserId(personId, pageNumber, pageSize));
        }
        [HttpGet]
        [Route("AdminDevicesByUserId/{personId}")]
        [Authorize]

        public Task<ResultViewModel<PagingResult<AdminDevice>>> GetAdminDevicesByUserId(int personId, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _adminDeviceRepository.GetAdminDevicesByUserId(personId, pageNumber, pageSize));
        }

    }
}