﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v2
{
    public class AccessGroupService
    {
        private readonly AccessGroupRepository _accessGroupRepository;
        private readonly DeviceService _deviceService;

        public AccessGroupService(AccessGroupRepository accessGroupRepository)
        {
            _accessGroupRepository = accessGroupRepository;
        }

        public async Task<ResultViewModel<PagingResult<AccessGroup>>> GetAccessGroups(long userId = default,
            int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default, int pageNumber = default, int pageSize = default, int nestingDepthLevel = 5, string token = default)
        {
            return await _accessGroupRepository.GetAccessGroups(userId, userGroupId, id, deviceId, deviceGroupId,
                pageNumber, pageSize, nestingDepthLevel, token);
        }

        public async Task<ResultViewModel<AccessGroup>> GetAccessGroup(int id = default, int nestingDepthLevel = 4, string token = default)
        {
            return await _accessGroupRepository.GetAccessGroup(id, nestingDepthLevel, token);
        }

        public async Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> GetDeviceOfAccessGroup(int accessGroupId = default,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return await _accessGroupRepository.GetDeviceOfAccessGroup(accessGroupId, pageNumber, pageSize, token);
        }

        public ResultViewModel<PagingResult<ServerSideIdentificationCacheModel>>
            GetServerSideIdentificationCacheOfAccessGroup(int accessGroupId = default, string brandCode = default, long userId = default,
                int pageNumber = default, int pageSize = default, string token = default)
        {
            return _accessGroupRepository.GetServerSideIdentificationCacheOfAccessGroup(accessGroupId, brandCode,
                userId, pageNumber, pageSize, token);
        }

        public async Task<ResultViewModel<List<User>>> GetAdminUserOfAccessGroup(long id = default, int accessGroupId = default, string token = default)
        {
            return await _accessGroupRepository.GetAdminUserOfAccessGroup(id, accessGroupId, token);
        }

        public async Task<ResultViewModel> AddAccessGroup(AccessGroup accessGroup = default, string token = default)
        {
            return await _accessGroupRepository.AddAccessGroup(accessGroup, token);
        }

        public async Task<ResultViewModel> ModifyAccessGroup(AccessGroup accessGroup = default, string token = default)
        {
            return await _accessGroupRepository.ModifyAccessGroup(accessGroup, token);
        }

        public async Task<ResultViewModel> ModifyAccessGroupAdminUsers(string xmlAdminUsers = default, int accessGroupId = default, string token = default)
        {
            return await _accessGroupRepository.ModifyAccessGroupAdminUsers(xmlAdminUsers, accessGroupId, token);
        }

        public async Task<ResultViewModel> ModifyAccessGroupDeviceGroup(string xmlDeviceGroup = default, int accessGroupId = default, string token = default)
        {
            return await _accessGroupRepository.ModifyAccessGroupDeviceGroup(xmlDeviceGroup, accessGroupId, token);
        }

        public async Task<ResultViewModel> ModifyAccessGroupUserGroup(string xmlUserGroup = default, int accessGroupId = default, string token = default)
        {
            return await _accessGroupRepository.ModifyAccessGroupUserGroup(xmlUserGroup, accessGroupId, token);
        }
        public async Task<ResultViewModel> DeleteAccessGroup(int id = default, string token = default)
        {
            return await _accessGroupRepository.DeleteAccessGroup(id, token);
        }

        // TODO - Verify the method.
        public void SendAccessGroupToDevice(DeviceBasicInfo device, int id, string token = default)
        {
            _accessGroupRepository.SendAccessGroupToDevice(device, id, token);
        }

        // TODO - Verify the method.
        public void SendUserToDevice(Lookup deviceBrand, DeviceBasicInfo device, string userIds, string token = default)
        {
            _accessGroupRepository.SendUserToDevice(deviceBrand, device, userIds, token);
        }

        // TODO - Verify the method.
        public void ModifyAccessGroup(string token = default)
        {
            var deviceBrands =  _deviceService.GetDeviceBrands().GetAwaiter().GetResult()?.Data?.Data;
            if (deviceBrands != null)
            {
                _accessGroupRepository.ModifyAccessGroup(deviceBrands, token);
            }
        }
    }
}
