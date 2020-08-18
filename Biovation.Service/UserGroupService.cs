using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository;

namespace Biovation.Service
{
    public class UserGroupService
    {
        private readonly UserGroupRepository _userGroupRepository;

        public UserGroupService(UserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public Task<ResultViewModel> ModifyUserGroup(UserGroup userGroup)
        {
            return _userGroupRepository.ModifyUserGroup(userGroup);
        }

        public ResultViewModel ModifyUserGroupMember(string node, int userGroupId)
        {
            return _userGroupRepository.ModifyUserGroupMember(node, userGroupId);
        }
        public ResultViewModel AddUserGroupMember(UserGroupMember userMember)
        {
            return _userGroupRepository.AddUserGroupMember(userMember);
        }

        public UserGroup GetUserGroup(int userGroupId)
        {
            return _userGroupRepository.GetUserGroup(userGroupId);
        }

        public List<UserGroup> GetUserGroups(long adminUserId)
        {
            return _userGroupRepository.GetUserGroups(adminUserId);
        }
        public List<UserGroup> GetUserGroupsOfUser(long userId)
        {
            return _userGroupRepository.GetUserGroupsOfUser(userId);
        }

        public ResultViewModel DeleteUserGroup(int userGroup)
        {
            return _userGroupRepository.DeleteUserGroup(userGroup);
        }

        public List<UserGroup> GetAccessControlUserGroup(int id)
        {
            return _userGroupRepository.GetAccessControlUserGroup(id);
        }

        public ResultViewModel SyncUserGroupMember(string lstUser)
        {
            return _userGroupRepository.SyncUserGroupMember(lstUser);
        }
    }
}
