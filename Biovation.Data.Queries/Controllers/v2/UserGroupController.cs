using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]

    public class UserGroupController : Controller
    {

        private readonly UserGroupRepository _userGroupRepository;

        public UserGroupController(UserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        [HttpGet]
        [Route("GetUsersGroup")]
        public Task<ResultViewModel<List<UserGroup>>> UsersGroup(long userId, int userGroupId)
        {
            return Task.Run(() => _userGroupRepository.GetUserGroup(userId, userGroupId));
        }

        [HttpGet]
        [Route("AccessControlUserGroup/{id}")]
        public Task<ResultViewModel<List<UserGroup>>> GetAccessControlUserGroup(int id = default)
        {
            return Task.Run(() => _userGroupRepository.GetAccessControlUserGroup(id));
        }

    }
}