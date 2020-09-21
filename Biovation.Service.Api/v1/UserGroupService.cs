using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v1
{
    public class UserGroupService
    {
        private readonly UserGroupRepository _userGroupRepository;

        public UserGroupService(UserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public List<UserGroup> UsersGroup(long userId = default, int userGroupId = default)
        {
            return _userGroupRepository.UsersGroup(userId, userGroupId)?.Data?.Data ?? new List<UserGroup>();
        }


        public List<UserGroup> GetAccessControlUserGroup(int id = default)
        {
            return _userGroupRepository.GetAccessControlUserGroup(id).Data;
        }

        public ResultViewModel SyncUserGroupMember(string lstUser = default)
        {
            return _userGroupRepository.SyncUserGroupMember(lstUser);
        }

        public ResultViewModel AddUserGroup(UserGroupMember userGroupMember = default)
        {
            return _userGroupRepository.AddUserGroup(userGroupMember);
        }

        public ResultViewModel ModifyUserGroup(UserGroup userGroup = default)
        {
            return _userGroupRepository.ModifyUserGroup(userGroup);
        }

        public ResultViewModel DeleteUserGroups(int groupId = default)
        {
            return _userGroupRepository.DeleteUserGroups(groupId);
        }
        public ResultViewModel ModifyUserGroupMember(List<UserGroupMember> member, int userGroupId)
        {
            return _userGroupRepository.ModifyUserGroupMember(member, userGroupId);
        }


    }
}
