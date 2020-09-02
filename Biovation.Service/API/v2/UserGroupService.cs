using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.API.v2
{
    public class UserGroupService
    {
        private readonly UserGroupRepository _userGroupRepository;

        public UserGroupService(UserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public ResultViewModel<List<UserGroup>> UsersGroup(long userId, int userGroupId)
        {
            return _userGroupRepository.UsersGroup(userId, userGroupId);
        }


        public ResultViewModel<List<UserGroup>> GetAccessControlUserGroup(int id = default)
        {
            return _userGroupRepository.GetAccessControlUserGroup(id);
        }

    }
}
