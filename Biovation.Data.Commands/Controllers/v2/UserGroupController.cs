using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class UserGroupController : ControllerBase
    {
        private readonly UserGroupRepository _userGroupRepository;

        public UserGroupController(UserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddUserGroup([FromBody] UserGroup userGroup = default)
        {
            return Task.Run(() => _userGroupRepository.ModifyUserGroup(userGroup));
        }

        [HttpPatch]
        [Authorize]
        [Route("{id:int}/Users")]
        public Task<ResultViewModel> ModifyUserGroupMembers([FromRoute] int id, [FromBody] List<UserGroupMember> members)
        {
            return Task.Run(() => _userGroupRepository.ModifyUserGroupMember(id, members));
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> ModifyUserGroup([FromBody] UserGroup userGroup = default)
        {
            return Task.Run(() => _userGroupRepository.ModifyUserGroup(userGroup));
        }

        [HttpDelete]
        [Authorize]
        [Route("{id:int}")]
        public Task<ResultViewModel> DeleteUserGroups(int id = default)
        {
            return Task.Run(() => _userGroupRepository.DeleteUserGroup(id));
        }
    }
}