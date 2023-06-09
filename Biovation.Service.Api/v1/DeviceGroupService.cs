﻿using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v1
{
    public class DeviceGroupService
    {
        private readonly DeviceGroupRepository _deviceGroupRepository;

        public DeviceGroupService(DeviceGroupRepository deviceGroupRepository)
        {
            _deviceGroupRepository = deviceGroupRepository;
        }

        public List<DeviceGroup> GetDeviceGroups(int deviceGroupId = default,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return _deviceGroupRepository.GetDeviceGroups(deviceGroupId, pageNumber, pageSize, token)?.Data?.Data ?? new List<DeviceGroup>();
        }

        public List<DeviceGroup> GetAccessControlDeviceGroup(int id = default,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return _deviceGroupRepository.GetAccessControlDeviceGroup(id, pageNumber, pageSize, token).Result?.Data?.Data ?? new List<DeviceGroup>();
        }

        public async Task<ResultViewModel> ModifyDeviceGroup(DeviceGroup deviceGroup = default, string token = default)
        {
            return await _deviceGroupRepository.ModifyDeviceGroup(deviceGroup, token);
        }

        public ResultViewModel ModifyDeviceGroupMember(string node = default, int groupId = default, string token = default)
        {
            return _deviceGroupRepository.ModifyDeviceGroupMember(node, groupId, token).Result;
        }

        public ResultViewModel DeleteDeviceGroup(int id = default, string token = default)
        {
            return _deviceGroupRepository.DeleteDeviceGroup(id, token).Result;
        }

        public ResultViewModel DeleteDeviceGroupMember(uint id = default, string token = default)
        {
            return _deviceGroupRepository.DeleteDeviceGroupMember(id, token).Result;
        }
    }
}
