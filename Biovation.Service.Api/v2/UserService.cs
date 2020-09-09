using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v2
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public ResultViewModel<PagingResult<User>> GetUsers(long onlineId = default, int from = default,
            int size = default, bool getTemplatesData = default, long userId = default, string filterText = default,
            int type = default, bool withPicture = default, bool isAdmin = default, int pageNumber = default,
            int pageSize = default)
        {
            return _userRepository.GetUsers(onlineId, from, size, getTemplatesData, userId, filterText, type,
                withPicture, isAdmin, pageNumber, pageSize);
        }


        public ResultViewModel<List<User>> GetAdminUserOfAccessGroup(long id = default, int accessGroupId = default)
        {
            return _userRepository.GetAdminUserOfAccessGroup(id, accessGroupId);
        }

        public ResultViewModel<int> GetUsersCount()
        {
            return _userRepository.GetUsersCount();
        }

        public ResultViewModel<List<DeviceBasicInfo>> GetAuthorizedDevicesOfUser(int userId)
        {
            return _userRepository.GetAuthorizedDevicesOfUser(userId);
        }

        public ResultViewModel ModifyUser(User user = default)
        {
            return _userRepository.ModifyUser(user);
        }

        public ResultViewModel DeleteUser(int id = default)
        {
            return _userRepository.DeleteUser(id);
        }

        public ResultViewModel DeleteUsers(List<int> ids = default)
        {
            return _userRepository.DeleteUsers(ids);
        }

        public ResultViewModel DeleteUserGroupsOfUser(int userId = default, int userTypeId = 1)
        {
            return _userRepository.DeleteUserGroupsOfUser(userId, userTypeId);
        }

        public ResultViewModel DeleteUserGroupOfUser(int userId =default, int userGroupId = default, int userTypeId = 1)
        {
            return _userRepository.DeleteUserGroupOfUser(userId, userGroupId, userTypeId);
        }

        public ResultViewModel ModifyPassword(int id = default, string password = default)
        {
            return _userRepository.ModifyPassword(id, password);
        }
    }
}
