using Biovation.CommonClasses.Extension;
using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("1.0")]
    public class AccessGroupController : Controller
    {
        private readonly AccessGroupRepository _accessGroupRepository;
        private readonly User _user;

        public AccessGroupController(AccessGroupRepository accessGroupRepository)
        {
            _accessGroupRepository = accessGroupRepository;
            _user = HttpContext.GetUser();
        }

        [HttpGet]
        [Authorize]

        public Task<ResultViewModel<PagingResult<AccessGroup>>> AccessGroups(int userId = 0, int userGroupId = 0, int id = 0, int deviceId = 0, int deviceGroupId = default, int pageNumber = default, int pageSize = default, int nestingDepthLevel = 5)
        {
            return Task.Run(() => _accessGroupRepository.AccessGroups(userId, (int)_user.Id, userGroupId, id, deviceId, deviceGroupId,
                 pageNumber, pageSize, nestingDepthLevel));
        }

        [HttpGet]
        [Authorize]

        [Route("{id}")]
        public Task<ResultViewModel<AccessGroup>> AccessGroup([FromRoute] int id, int nestingDepthLevel = 4)
        {
            return Task.Run(() => _accessGroupRepository.AccessGroup(id, nestingDepthLevel));
        }


        [HttpGet]
        [Route("DeviceOfAccessGroup")]
        [Authorize]

        public Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> GetDeviceOfAccessGroup(int accessGroupId, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _accessGroupRepository.GetDeviceOfAccessGroup(accessGroupId, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("ServerSideIdentificationCacheOfAccessGroup")]
        [Authorize]

        public Task<ResultViewModel<PagingResult<ServerSideIdentificationCacheModel>>> GetServerSideIdentificationCacheOfAccessGroup(int accessGroupId, string brandCode, long userId, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _accessGroupRepository.GetServerSideIdentificationCacheOfAccessGroup(accessGroupId, brandCode, userId, pageNumber, pageSize));
        }
    }
}
