using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v1
{
    public class UserGroupService
    {
        private readonly UserGroupRepository _userGroupRepository;

        public UserGroupService(UserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public List<UserGroup> UsersGroup(int userGroupId = default, string token = default)
        {
            return _userGroupRepository.UserGroups(userGroupId, token).Result?.Data?.Data ?? new List<UserGroup>();
        }


        public List<UserGroup> GetAccessControlUserGroup(int id = default, string token = default)
        {
            return _userGroupRepository.GetAccessControlUserGroup(id, token)?.Data ?? new List<UserGroup>();
        }

        public ResultViewModel SyncUserGroupMember(string lstUser = default, string token = default)
        {
            return _userGroupRepository.SyncUserGroupMember(lstUser, token).Result ;
        }

        public ResultViewModel AddUserGroup(UserGroupMember userGroupMember = default, string token = default)
        {
            return _userGroupRepository.AddUserGroup(userGroupMember, token).Result;
        }

        public ResultViewModel ModifyUserGroup(UserGroup userGroup = default, string token = default)
        {
            return _userGroupRepository.ModifyUserGroup(userGroup, token).Result;
        }

        public async Task<ResultViewModel> DeleteUserGroup(int groupId = default, string token = default)
        {
            return await _userGroupRepository.DeleteUserGroup(groupId, token);
        }
        public ResultViewModel ModifyUserGroupMember(List<UserGroupMember> member, int userGroupId, string token = default)
        {
            return _userGroupRepository.ModifyUserGroupMember(member, userGroupId, token);
        }
    }
}
