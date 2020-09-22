using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;

namespace Biovation.Service.Api.v1
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<User> GetUsers(long userId = default, bool withPicture = default, long onlineUserId = default, int from = default,
            int size = default, bool getTemplatesData = default, string filterText = default,
            int type = default, bool isAdmin = default, int pageNumber = default,
            int pageSize = default)
        {
            return _userRepository.GetUsers(onlineUserId, from, size, getTemplatesData, userId, filterText, type,
                withPicture, isAdmin, pageNumber, pageSize)?.Data?.Data ?? new List<User>();
        }

        public List<User> GetAdminUser(long userId = 0)
        {
            return _userRepository.GetAdminUser(userId)?.Data?.Data ?? new List<User>();
        }

        public List<User> GetAdminUserOfAccessGroup(long userId = default, int accessGroupId = default)
        {
            return _userRepository.GetAdminUserOfAccessGroup(userId, accessGroupId)?.Data ?? new List<User>();
        }

        public int GetUsersCount()
        {
            return _userRepository.GetUsersCount()?.Data ?? default;
        }

        public List<DeviceBasicInfo> GetAuthorizedDevicesOfUser(long userId)
        {
            return _userRepository.GetAuthorizedDevicesOfUser((int)userId)?.Data ?? new List<DeviceBasicInfo>();
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

        public ResultViewModel DeleteUserGroupOfUser(int userId = default, int userGroupId = default, int userTypeId = 1)
        {
            return _userRepository.DeleteUserGroupOfUser(userId, userGroupId, userTypeId);
        }

        public ResultViewModel ModifyPassword(int id = default, string password = default)
        {
            return _userRepository.ModifyPassword(id, password);
        }
    }
}
