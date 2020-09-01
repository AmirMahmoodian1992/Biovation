using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [Route("biovation/api/queries/v2/[controller]")]
    //[ApiVersion("1.0")]
    public class AccessGroupController : Controller
    {
        private readonly AccessGroupRepository _accessGroupRepository;


        public AccessGroupController(AccessGroupRepository accessGroupRepository)
        {
            _accessGroupRepository = accessGroupRepository;
        }


        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        [HttpGet]


        [HttpGet]
        [Route("{userId}")]
        //public Task<IActionResult> AccessGroups(long userId = default, int adminUserId = default, int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default)
        //{
            public Task<ResultViewModel<PagingResult<AccessGroup>>> AccessGroups(int userId = 0,int adminUserId = 0, int userGroupId = 0, int id = 0, int deviceId = 0, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _accessGroupRepository.GetAccessGroupsByFilter(adminUserId, userGroupId, id, deviceId, userId,
                 pageNumber, pageSize));
        }

        [HttpGet]
        [Route("GetAccessGroupsOfUser")]
        public Task<ResultViewModel<PagingResult<AccessGroup>>> GetAccessGroupsOfUser(long userId, int nestingDepthLevel, int pageNumber = default, int pageSize = default)
        {

            return Task.Run(() => _accessGroupRepository.GetAccessGroupsOfUser(userId, nestingDepthLevel, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("GetAccessGroupsOfDevice")]
        public Task<ResultViewModel<PagingResult<AccessGroup>>> GetAccessGroupsOfDevice(uint deviceId, int nestingDepthLevel, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _accessGroupRepository.GetAccessGroupsOfDevice(deviceId, nestingDepthLevel, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("GetAccessGroupsOfUserGroup")]
        public Task<ResultViewModel<PagingResult<AccessGroup>>> GetAccessGroupsOfUserGroup(int userGroupId, int nestingDepthLevel, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _accessGroupRepository.GetAccessGroupsOfUserGroup(userGroupId, nestingDepthLevel, pageNumber, pageSize));
        }


        [HttpGet]
        [Route("{id?}")]
        public Task<ResultViewModel<AccessGroup>> GetAccessGroup([FromRoute]int id, int nestingDepthLevel=default)
        {
            return Task.Run(() => _accessGroupRepository.GetAccessGroup(id, nestingDepthLevel));

        }


        [HttpGet]
        [Route("SearchAccessGroup")]
        public Task<ResultViewModel<PagingResult<AccessGroup>>> SearchAccessGroup(int accessGroupId, int deviceGroupId, int userId, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _accessGroupRepository.SearchAccessGroup(accessGroupId, deviceGroupId, userId, pageNumber, pageSize));
        }


        [HttpGet]
        [Route("GetDeviceOfAccessGroup")]
        public Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> GetDeviceOfAccessGroup(int accessGroupId, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _accessGroupRepository.GetDeviceOfAccessGroup(accessGroupId, pageNumber, pageSize));

        }

        [HttpGet]
        [Route("GetServerSideIdentificationCacheNoTemplate")]
        public Task<ResultViewModel<PagingResult<ServerSideIdentificationCacheModel>>> GetServerSideIdentificationCacheNoTemplate(long userId, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _accessGroupRepository.GetServerSideIdentificationCacheNoTemplate(userId, pageNumber, pageSize));

        }


        [HttpGet]
        [Route("GetServerSideIdentificationCacheOfAccessGroup")]
        public Task<ResultViewModel<PagingResult<ServerSideIdentificationCacheModel>>> GetServerSideIdentificationCacheOfAccessGroup(int accessGroupId, string brandCode, long userId, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _accessGroupRepository.GetServerSideIdentificationCacheOfAccessGroup(accessGroupId, brandCode, userId, pageNumber, pageSize));

        }
    }
}

