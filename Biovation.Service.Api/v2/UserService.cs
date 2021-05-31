using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using RestSharp;

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

        public async Task<ResultViewModel> ModifyUser(User user = default, string token = default)
        {
            return await _userRepository.ModifyUser(user, token);
        }

        public async Task<ResultViewModel> DeleteUser(int id = default, string token = default)
        {
            return await _userRepository.DeleteUser(id, token);
        }

        public async Task<ResultViewModel> DeleteUsers(List<int> ids = default, string token = default)
        {
            return await _userRepository.DeleteUsers(ids, token);
        }

        public ResultViewModel DeleteUserGroupsOfUser(int userId = default, int userTypeId = 1, string token = default)
        {
            return _userRepository.DeleteUserGroupsOfUser(userId, userTypeId, token);
        }

        public async Task<ResultViewModel> DeleteUserGroupOfUser(int userId =default, int userGroupId = default, int userTypeId = 1, string token = default)
        {
            return await _userRepository.DeleteUserGroupOfUser(userId, userGroupId, userTypeId, token);
        }

        public async Task<ResultViewModel> ModifyPassword(int id = default, string password = default, string token = default)
        {
            return await _userRepository.ModifyPassword(id, password, token);
        }

        // TODO - Verify method.
        public void DeleteUserFromAllTerminal(List<Lookup> deviceBrands, long[] usersToSync = default, string token = default)
        {
            _userRepository.DeleteUserFromAllTerminal(deviceBrands, usersToSync, token);
        }

        // TODO - Verify method.
        public void AddUser(List<Lookup> deviceBrands, User user = default, string token = default)
        {
            _userRepository.AddUser(deviceBrands, user, token);
        }

        // TODO - Verify method.
        public void ModifyUser(List<Lookup> deviceBrands, User user = default, string token = default)
        {
            _userRepository.ModifyUser(deviceBrands, user, token);
        }

        // TODO - Verify method.
        public void Sync(Lookup deviceBrand, DeviceBasicInfo device, UserGroupMember userGroupMember, string token = default)
        {
            _userRepository.Sync(deviceBrand, device, userGroupMember, token);
        }

        // TODO - Verify method.
        public Task<IRestResponse<ResultViewModel>> EnrollFaceTemplate(DeviceBasicInfo device, int id = default, int deviceId = default, string token = default)
        {
            return _userRepository.EnrollFaceTemplate(device, id, deviceId, token);
        }

        // TODO - Verify method.
        public Task<IRestResponse> UpdateUserGroupsOfUser(Lookup deviceBrand, DeviceBasicInfo device, int userId, string token = default)
        {
            return _userRepository.UpdateUserGroupsOfUser(deviceBrand, device, userId, token);
        }

        // TODO - Verify method.
        public Task<IRestResponse> DeleteUserFromDevice(Lookup deviceBrand, DeviceBasicInfo device, List<int> listOfUserId, string token = default)
        {
            return _userRepository.DeleteUserFromDevice(deviceBrand, device, listOfUserId, token);
        }

        // TODO - Verify method.
        public List<ResultViewModel> SendUserToAllDevices(Lookup deviceBrand, long userId, User user = default)
        {
            return _userRepository.SendUserToAllDevices(deviceBrand, userId, user);
        }
    }
}
