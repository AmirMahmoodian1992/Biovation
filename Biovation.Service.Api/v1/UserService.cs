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

        public List<User> GetUsers(long userId = default, bool withPicture = default,/* long onlineUserId = default, */int from = default,
            int size = default, bool getTemplatesData = default, string filterText = default,
            int type = default, bool isAdmin = default, int pageNumber = default,
            int pageSize = default, string token = default, long code = default)
        {
            return _userRepository. GetUsers(from, size, getTemplatesData, userId, filterText: filterText, type: type,
                withPicture: withPicture, isAdmin: isAdmin, pageNumber: pageNumber, pageSize: pageSize, token: token, code: code).Result?.Data?.Data ?? new List<User>();
        }

        public List<User> GetAdminUser(long userId = 0, string token = default)
        {
            return _userRepository.GetAdminUser(userId, token)?.Data?.Data ?? new List<User>();
        }

        public int GetUsersCount()
        {
            return _userRepository.GetUsersCount()?.Data ?? default;
        }

        public List<DeviceBasicInfo> GetAuthorizedDevicesOfUser(long userId, string token = default)
        {
            return _userRepository.GetAuthorizedDevicesOfUser((int)userId, token)?.Data ?? new List<DeviceBasicInfo>();
        }

        public ResultViewModel ModifyUser(User user = default, string token = default)
        {
            return _userRepository.ModifyUser(user, token);
        }

        public ResultViewModel DeleteUser(long id = default, string token = default)
        {
            return _userRepository.DeleteUser(id, token).Result;
        }

        public ResultViewModel DeleteUsers(List<int> ids = default, string token = default)
        {
            return _userRepository.DeleteUsers(ids, token).Result;
        }

        public ResultViewModel DeleteUserGroupsOfUser(int userId = default, int userTypeId = 1, string token = default)
        {
            return _userRepository.DeleteUserGroupsOfUser(userId, userTypeId, token);
        }

        public ResultViewModel DeleteUserGroupOfUser(int userId = default, int userGroupId = default, int userTypeId = 1, string token = default)
        {
            return _userRepository.DeleteUserGroupOfUser(userId, userGroupId, userTypeId, token);
        }

        public ResultViewModel ModifyPassword(int id = default, string password = default, string token = default)
        {
            return _userRepository.ModifyPassword(id, password, token);
        }
    }
}
