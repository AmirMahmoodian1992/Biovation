using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.Api.v1
{
    public class DeviceGroupService
    {
        readonly DeviceGroupRepository _deviceGroupRepository;

        public DeviceGroupService(DeviceGroupRepository deviceGroupRepository)
        {
            _deviceGroupRepository = deviceGroupRepository;
        }

        public List<DeviceGroup> GetDeviceGroups(int deviceGroupId = default, long userId=default,
            int pageNumber = default, int pageSize = default)
        {
            return _deviceGroupRepository.GetDeviceGroups(deviceGroupId, userId, pageNumber, pageSize).Data.Data;
        }

        public List<DeviceGroup> GetAccessControlDeviceGroup(int id =default,
            int pageNumber = default, int pageSize = default)
        {
            return _deviceGroupRepository.GetAccessControlDeviceGroup(id, pageNumber, pageSize).Data.Data;
        }

        public ResultViewModel ModifyDeviceGroup(DeviceGroup deviceGroup =default)
        {
            return _deviceGroupRepository.ModifyDeviceGroup(deviceGroup);
        }

        public ResultViewModel ModifyDeviceGroupMember(string node = default, int groupId =default)
        {
            return _deviceGroupRepository.ModifyDeviceGroupMember(node, groupId);
        }

        public ResultViewModel DeleteDeviceGroup(int id =default)
        {
            return _deviceGroupRepository.DeleteDeviceGroup(id);
        }

        public ResultViewModel DeleteDeviceGroupMember(uint id = default)
        {
            return _deviceGroupRepository.DeleteDeviceGroupMember(id);
        }


    }
}
