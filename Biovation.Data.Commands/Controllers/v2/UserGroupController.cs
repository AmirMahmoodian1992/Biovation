using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/commands/v2/[controller]")]

    public class UserGroupController : Controller
    {

        private readonly UserGroupRepository _userGroupRepository;

        public UserGroupController(UserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }



        //todo:add UserGroup
        /*[HttpPost]
        public Task<ResultViewModel> AddUserGroup([FromBody]UserGroup userGroup = default)
        {
            /*return Task.Run(() => _userGroupRepository.(userGroup));*/
        /*  throw null;
       }*/

        [HttpPatch]
        [Route("UserGroupMember/userGroupId")]
        [Authorize]

        public Task<ResultViewModel> ModifyUserGroupMember([FromBody]List<UserGroupMember> member, int userGroupId)
        {
            return Task.Run(() => _userGroupRepository.ModifyUserGroupMember(member, userGroupId));
        }
  

        [HttpPut]
        [Authorize]

        public Task<ResultViewModel> ModifyUserGroup([FromBody] UserGroup userGroup = default)
        {
            return Task.Run(() => _userGroupRepository.ModifyUserGroup(userGroup));
        }

        [HttpDelete]
        [Route("{groupId}")]
        [Authorize]

        public Task<ResultViewModel> DeleteUserGroups(int groupId = default)
        {
            return Task.Run(() => _userGroupRepository.DeleteUserGroup(groupId));
        }

        
    }
}