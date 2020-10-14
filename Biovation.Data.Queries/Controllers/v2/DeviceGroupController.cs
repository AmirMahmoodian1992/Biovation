using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{


    //[ApiVersion("1.0")]
    [Route("biovation/api/v2/[controller]")]
    public class DeviceGroupController : Controller
    {
        private readonly DeviceGroupRepository _deviceGroupRepository;

        public DeviceGroupController(DeviceGroupRepository deviceGroupRepository)
        {
            _deviceGroupRepository = deviceGroupRepository;
        }


        [HttpGet]
        [Route("GetDeviceGroups")]
        [Authorize]

        public Task<ResultViewModel<PagingResult<DeviceGroup>>> GetDeviceGroups(int deviceGroupId, long userId,
            int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _deviceGroupRepository.GetDeviceGroups(deviceGroupId, userId, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("GetAccessControlDeviceGroup")]
        [Authorize]

        public Task<ResultViewModel<PagingResult<DeviceGroup>>> GetAccessControlDeviceGroup(int id,
            int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _deviceGroupRepository.GetAccessControlDeviceGroup(id, pageNumber, pageSize));
        }


        [HttpGet]
        [Route("GetDeviceGroupsByAccessGroup")]
        [Authorize]

        public Task<ResultViewModel<PagingResult<DeviceGroup>>> GetDeviceGroupsByAccessGroup(int accessGroupId,
            int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _deviceGroupRepository.GetDeviceGroupsByAccessGroup(accessGroupId, pageNumber, pageSize));

        }

    }
}
