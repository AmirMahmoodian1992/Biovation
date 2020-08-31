using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.v2;
using Microsoft.AspNetCore.Mvc;

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
        public Task<ResultViewModel<PagingResult<AccessGroup>>> GetAccessGroupsByFilter(int adminUserId = 0, int userGroupId = 0, int id = 0, int deviceId = 0, int userId = 0, int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _accessGroupRepository.GetAccessGroupsByFilter(adminUserId, userGroupId, id, deviceId, userId,
                 pageNumber, PageSize));
        }

        [HttpGet]
        [Route("GetAccessGroupsOfUser")]
        public Task<ResultViewModel<PagingResult<AccessGroup>> GetAccessGroupsOfUser(long userId, int nestingDepthLevel, int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _accessGroupRepository.GetAccessGroupsOfUser(userId,pageNumber, PageSize));
        }

        [HttpGet]
        [Route("GetAccessGroupsOfDevice")]
        public List<AccessGroup> GetAccessGroupsOfDevice(uint deviceId, int nestingDepthLevel)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@DeviceId", SqlDbType.Int) {Value = deviceId}
            };
            return _repository.ToResultList<AccessGroup>($"SelectAccessGroupsByDeviceId{(nestingDepthLevel == 0 ? "" : "NestedProperties")}", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).Data;
        }

        [HttpGet]
        [Route("GetAccessGroupsOfUserGroup")]
        public List<AccessGroup> GetAccessGroupsOfUserGroup(int userGroupId, int nestingDepthLevel)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserGroupId", SqlDbType.Int) {Value = userGroupId}
            };
            return _repository.ToResultList<AccessGroup>($"SelectAccessGroupsByUserGroupId{(nestingDepthLevel == 0 ? "" : "NestedProperties")}", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).Data;
        }


        [HttpGet]
        [Route("GetAccessGroup")]
        /// <summary>
        /// <En></En>
        /// <Fa></Fa>
        /// </summary>
        /// <param name="accessGroupId"></param>
        /// <param name="nestingDepthLevel"></param>
        /// <returns></returns>
        public AccessGroup GetAccessGroup(int accessGroupId, int nestingDepthLevel)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", accessGroupId)
            };

            return _repository.ToResultList<AccessGroup>("SelectAccessGroupByID", parameters, fetchCompositions: true, compositionDepthLevel: nestingDepthLevel).Data.FirstOrDefault();
        }


        [HttpGet]
        [Route("SearchAccessGroup")]
        /// <summary>
        /// <En></En>
        /// <Fa></Fa>
        /// </summary>
        /// <param name="accessGroupId"></param>
        /// <param name="deviceGroupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<AccessGroup> SearchAccessGroup(int accessGroupId, int deviceGroupId, int userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", accessGroupId),
                new SqlParameter("@DeviceGroupId", deviceGroupId),
                new SqlParameter("@UserId", userId)
            };

            return _repository.ToResultList<AccessGroup>("SelectSearchAccessGroup", parameters, fetchCompositions: true).Data;
        }


        [HttpGet]
        [Route("GetDeviceOfAccessGroup")]
        /// <summary>
        /// <En></En>
        /// <Fa></Fa>
        /// </summary>
        /// <param name="accessGroupId"></param>
        /// <returns></returns>
        public List<DeviceBasicInfo> GetDeviceOfAccessGroup(int accessGroupId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", accessGroupId)
            };

            return _repository.ToResultList<DeviceBasicInfo>("SelectDeviceOfAccessGroup", parameters, fetchCompositions: true).Data;
        }

        [HttpGet]
        [Route("GetServerSideIdentificationCacheNoTemplate")]
        public List<ServerSideIdentificationCacheModel> GetServerSideIdentificationCacheNoTemplate(long userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@adminUserId", userId)
            };

            return _repository.ToResultList<ServerSideIdentificationCacheModel>("SelectServerSideIdentificationCacheNoTemplate", parameters).Data;
        }


        [HttpGet]
        [Route("GetServerSideIdentificationCacheOfAccessGroup")]
        public List<ServerSideIdentificationCacheModel> GetServerSideIdentificationCacheOfAccessGroup(int accessGroupId, string brandCode, long userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@adminUserId", userId),
                new SqlParameter("@brandCode", brandCode),
                new SqlParameter("@accessGroupId", accessGroupId)
            };

            return _repository.ToResultList<ServerSideIdentificationCacheModel>("SelectServerSideIdentificationCacheOfAccessGroup", parameters, fetchCompositions: true, compositionDepthLevel: 4).Data;
        }
    }
}

