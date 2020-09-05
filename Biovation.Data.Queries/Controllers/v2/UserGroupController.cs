using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        [Route("UsersGroup")]
        public Task<ResultViewModel<PagingResult<UserGroup>>> UsersGroup(int id, long userId, int accessGroupId, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _userGroupRepository.GetUserGroups(id,userId,accessGroupId,pageNumber,pageSize));
        }

        [HttpGet]
        [Route("AccessControlUserGroup/{id}")]
        public Task<ResultViewModel<List<UserGroup>>> GetAccessControlUserGroup(int id = default)
        {
            return Task.Run(() => _userGroupRepository.GetAccessControlUserGroup(id));
        }

        [HttpGet]
        [Route("SyncUserGroupMember")]
        public Task<ResultViewModel> SyncUserGroupMember(string lstUser)
        {
            return Task.Run(() => _userGroupRepository.SyncUserGroupMember(lstUser));
        }
    }
}