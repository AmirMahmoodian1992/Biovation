using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using RestSharp;

namespace Biovation.Service.Api.v2
{
    public class UserGroupService
    {
        private readonly UserGroupRepository _userGroupRepository;
        private readonly DeviceService _deviceService;
        private readonly UserGroupService _userGroupService;
        private readonly UserService _userService;
        private readonly SystemInfo _systemInfo;

        public UserGroupService(UserGroupRepository userGroupRepository, DeviceService deviceService, UserGroupService userGroupService, UserService userService, SystemInfo systemInfo)
        {
            _userGroupRepository = userGroupRepository;
            _deviceService = deviceService;
            _userGroupService = userGroupService;
            _userService = userService;
            _systemInfo = systemInfo;
        }

        public ResultViewModel<PagingResult<UserGroup>> UserGroups(int userGroupId = default, string token = default)
        {
            return _userGroupRepository.UserGroups(userGroupId, token);
        }


        public ResultViewModel<List<UserGroup>> GetAccessControlUserGroup(int id = default, string token = default)
        {
            return _userGroupRepository.GetAccessControlUserGroup(id, token);
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

        public ResultViewModel DeleteUserGroup(int groupId = default, string token = default)
        {
            return _userGroupRepository.DeleteUserGroup(groupId, token);
        }
        public ResultViewModel ModifyUserGroupMember(List<UserGroupMember> member, int userGroupId, string token = default)
        {
            return _userGroupRepository.ModifyUserGroupMember(member, userGroupId, token);
        }

        public ResultViewModel SendUsersOfGroup(int userGroupId, string token = default)
        {
            var deviceBrands = _deviceService.GetDeviceBrands(token: token)?.Data?.Data;

            var userGroup = _userGroupService.UserGroups(userGroupId: userGroupId, token: token)?.Data?.Data.FirstOrDefault();

            var serviceInstances = _systemInfo.Services;

            if (userGroup is null || deviceBrands is null)
            {
                return new ResultViewModel
                    { Success = false, Validate = 0, Message = "Provided user group is wrong", Id = userGroupId };
            }
            
            foreach (var userGroupMember in userGroup.Users)
            {
                var user = _userService.GetUsers(code: userGroupMember.UserId, token: token)?.Data?.Data.FirstOrDefault();

                foreach (var deviceBrand in deviceBrands)
                {
                    _userGroupRepository.SendUsersOfGroup(serviceInstances, deviceBrand, user);
                }
            }
            return new ResultViewModel { Validate = 1, Id = userGroupId };
        }

        public ResultViewModel DeleteUserFromDevice(DeviceBasicInfo device, IEnumerable<User> usersToDeleteFromDevice, string token = default)
        {
            return _userGroupRepository.DeleteUserFromDevice(device, usersToDeleteFromDevice,token);
        }

        public List<ResultViewModel> SendUserToDevice(DeviceBasicInfo device, IEnumerable<User> usersToDeleteFromDevice, string token = default)
        {
            return _userGroupRepository.SendUserToDevice(device, usersToDeleteFromDevice,token);
        }

        public List<ResultViewModel> ModifyUserGroupMember(string token)
        {
            var deviceBrands = _deviceService.GetDeviceBrands(token: token)?.Data?.Data;

            if (deviceBrands == null)
            {
                return new List<ResultViewModel>();
            }

            foreach (var deviceBrand in deviceBrands)
            {
                return _userGroupRepository.ModifyUserGroupMember(deviceBrand);
            }

            return new List<ResultViewModel>();
        }
    }
}
