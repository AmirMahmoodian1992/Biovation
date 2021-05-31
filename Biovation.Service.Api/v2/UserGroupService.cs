using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2
{
    public class UserGroupService
    {
        private readonly UserGroupRepository _userGroupRepository;
        private readonly DeviceService _deviceService;
        private readonly UserService _userService;
        private readonly SystemInfo _systemInfo;

        public UserGroupService(UserGroupRepository userGroupRepository, DeviceService deviceService, UserService userService, SystemInfo systemInfo)
        {
            _userGroupRepository = userGroupRepository;
            _deviceService = deviceService;
            _userService = userService;
            _systemInfo = systemInfo;
        }

        public async Task<ResultViewModel<PagingResult<UserGroup>>> UserGroups(int userGroupId = default, string token = default)
        {
            return await _userGroupRepository.UserGroups(userGroupId, token);
        }


        public async Task<ResultViewModel<List<UserGroup>>> GetAccessControlUserGroup(int id = default, string token = default)
        {
            return await _userGroupRepository.GetAccessControlUserGroup(id, token);
        }

        public async Task<ResultViewModel> SyncUserGroupMember(string lstUser = default, string token = default)
        {
            return await _userGroupRepository.SyncUserGroupMember(lstUser, token);
        }

        public async Task<ResultViewModel> AddUserGroup(UserGroupMember userGroupMember = default, string token = default)
        {
            return await _userGroupRepository.AddUserGroup(userGroupMember, token);
        }

        public async Task<ResultViewModel> ModifyUserGroup(UserGroup userGroup = default, string token = default)
        {
            return await _userGroupRepository.ModifyUserGroup(userGroup, token);
        }

        public async Task<ResultViewModel> DeleteUserGroup(int groupId = default, string token = default)
        {
            return await _userGroupRepository.DeleteUserGroup(groupId, token);
        }
        public ResultViewModel ModifyUserGroupMember(List<UserGroupMember> member, int userGroupId, string token = default)
        {
            return _userGroupRepository.ModifyUserGroupMember(member, userGroupId, token);
        }

        public async Task<ResultViewModel> SendUsersOfGroup(int userGroupId, string token = default)
        {
            var deviceBrands = (await _deviceService.GetDeviceBrands(token: token))?.Data?.Data;

            var userGroup = (await UserGroups(userGroupId: userGroupId, token))?.Data?.Data.FirstOrDefault();

            var serviceInstances = _systemInfo.Services;

            if (userGroup is null || deviceBrands is null)
            {
                return new ResultViewModel
                { Success = false, Validate = 0, Message = "Provided user group is wrong", Id = userGroupId };
            }

            foreach (var userGroupMember in userGroup.Users)
            {
                var user = (await _userService.GetUsers(code: userGroupMember.UserId, token: token))?.Data?.Data.FirstOrDefault();

                foreach (var deviceBrand in deviceBrands)
                {
                    _userGroupRepository.SendUsersOfGroup(serviceInstances, deviceBrand, user);
                }
            }
            return new ResultViewModel { Validate = 1, Id = userGroupId };
        }

        public ResultViewModel DeleteUserFromDevice(DeviceBasicInfo device, IEnumerable<User> usersToDeleteFromDevice, string token = default)
        {
            return _userGroupRepository.DeleteUserFromDevice(device, usersToDeleteFromDevice, token);
        }

        public List<ResultViewModel> SendUserToDevice(DeviceBasicInfo device, IEnumerable<User> usersToDeleteFromDevice, string token = default)
        {
            return _userGroupRepository.SendUserToDevice(device, usersToDeleteFromDevice, token);
        }

        public async Task<List<ResultViewModel>> ModifyUserGroupMember(string token)
        {
            var results = new List<ResultViewModel>();
            var deviceBrands = (await _deviceService.GetDeviceBrands(token: token))?.Data?.Data;
            var serviceInstances = _systemInfo.Services;

            if (deviceBrands == null)
                return results;

            foreach (var deviceBrand in deviceBrands)
            {
                var modifyResult = await _userGroupRepository.ModifyUserGroupMember(deviceBrand, serviceInstances, token);
                if (modifyResult != null)
                    results.AddRange(modifyResult);
            }

            return results;
        }
    }
}
