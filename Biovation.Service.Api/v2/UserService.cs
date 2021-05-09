using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<ResultViewModel<PagingResult<User>>> GetUsers(int from = default,
            int size = default, bool getTemplatesData = default, long userId = default, long code = default, string filterText = default,
            int type = default, bool withPicture = default, bool isAdmin = default, int pageNumber = default,
            int pageSize = default, string token = default)
        {
            return await _userRepository.GetUsers(from, size, getTemplatesData, userId, code, filterText, type,
                withPicture, isAdmin, pageNumber, pageSize, token);
        }

        public ResultViewModel<int> GetUsersCount(string token = default)
        {
            return _userRepository.GetUsersCount(token);
        }

        public ResultViewModel<List<DeviceBasicInfo>> GetAuthorizedDevicesOfUser(int userId, string token = default)
        {
            return _userRepository.GetAuthorizedDevicesOfUser(userId,token);
        }

        public ResultViewModel ModifyUser(User user = default, string token = default)
        {
            return _userRepository.ModifyUser(user, token);
        }

        public ResultViewModel DeleteUser(int id = default, string token = default)
        {
            return _userRepository.DeleteUser(id, token);
        }

        public ResultViewModel DeleteUsers(List<int> ids = default, string token = default)
        {
            return _userRepository.DeleteUsers(ids, token);
        }

        public ResultViewModel DeleteUserGroupsOfUser(int userId = default, int userTypeId = 1, string token = default)
        {
            return _userRepository.DeleteUserGroupsOfUser(userId, userTypeId, token);
        }

        public ResultViewModel DeleteUserGroupOfUser(int userId =default, int userGroupId = default, int userTypeId = 1, string token = default)
        {
            return _userRepository.DeleteUserGroupOfUser(userId, userGroupId, userTypeId, token);
        }

        public ResultViewModel ModifyPassword(int id = default, string password = default, string token = default)
        {
            return _userRepository.ModifyPassword(id, password, token);
        }
    }
}
