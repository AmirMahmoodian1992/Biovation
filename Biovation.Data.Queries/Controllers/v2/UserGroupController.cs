using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Extension;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]

    public class UserGroupController : Controller
    {
        private readonly UserGroupRepository _userGroupRepository;

        public UserGroupController(UserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<UserGroup>>> UsersGroup(int id, long userId, int accessGroupId, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _userGroupRepository.GetUserGroups(id,HttpContext.GetUser().Id , accessGroupId, userId,pageNumber,pageSize));
        }

        [HttpGet]
        [Route("AccessControlUserGroup/{id}")]
        [Authorize]

        public Task<ResultViewModel<List<UserGroup>>> GetAccessControlUserGroup(int id = default)
        {
            return Task.Run(() => _userGroupRepository.GetAccessControlUserGroup(id));
        }

        [HttpGet]
        [Route("SyncUserGroupMember")]
        [Authorize]

        public Task<ResultViewModel> SyncUserGroupMember(string lstUser,int id, int deviceGroupId)
        {
            return Task.Run(() => _userGroupRepository.SyncUserGroupMember(lstUser,id,(int) HttpContext.GetUser().Id,deviceGroupId));
        }
    }
}