using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;

namespace Biovation.Service.Api.v1
{
    public class UserGroupService
    {
        private readonly UserGroupRepository _userGroupRepository;

        public UserGroupService(UserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public List<UserGroup> UsersGroup(long userId = default, int userGroupId = default, string token = default)
        {
            return _userGroupRepository.UsersGroup(userId, userGroupId, token)?.Data?.Data ?? new List<UserGroup>();
        }


        public List<UserGroup> GetAccessControlUserGroup(int id = default, string token = default)
        {
            return _userGroupRepository.GetAccessControlUserGroup(id, token)?.Data ?? new List<UserGroup>();
        }

        public ResultViewModel SyncUserGroupMember(string lstUser = default, string token = default)
        {
            return _userGroupRepository.SyncUserGroupMember(lstUser, token);
        }

        public ResultViewModel AddUserGroup(UserGroupMember userGroupMember = default, string token = default)
        {
            return _userGroupRepository.AddUserGroup(userGroupMember, token);
        }

        public ResultViewModel ModifyUserGroup(UserGroup userGroup = default, string token = default)
        {
            return _userGroupRepository.ModifyUserGroup(userGroup, token);
        }

        public ResultViewModel DeleteUserGroups(int groupId = default, string token = default)
        {
            return _userGroupRepository.DeleteUserGroups(groupId, token);
        }
        public ResultViewModel ModifyUserGroupMember(List<UserGroupMember> member, int userGroupId, string token = default)
        {
            return _userGroupRepository.ModifyUserGroupMember(member, userGroupId, token);
        }
    }
}
