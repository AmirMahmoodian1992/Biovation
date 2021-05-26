using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v2
{
    public class AccessGroupService
    {
        private readonly AccessGroupRepository _accessGroupRepository;

        public AccessGroupService(AccessGroupRepository accessGroupRepository)
        {
            _accessGroupRepository = accessGroupRepository;
        }

        public ResultViewModel<PagingResult<AccessGroup>> GetAccessGroups(long userId = default,
            int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default, int pageNumber = default, int pageSize = default, int nestingDepthLevel = 5, string token = default)
        {
            return _accessGroupRepository.GetAccessGroups(userId, userGroupId, id, deviceId, deviceGroupId,
                pageNumber, pageSize, nestingDepthLevel, token);
        }

        public ResultViewModel<AccessGroup> GetAccessGroup(int id = default, int nestingDepthLevel = 4, string token = default)
        {
            return _accessGroupRepository.GetAccessGroup(id, nestingDepthLevel, token);
        }

        public ResultViewModel<PagingResult<DeviceBasicInfo>> GetDeviceOfAccessGroup(int accessGroupId = default,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return _accessGroupRepository.GetDeviceOfAccessGroup(accessGroupId, pageNumber, pageSize, token);
        }

        public ResultViewModel<PagingResult<ServerSideIdentificationCacheModel>>
            GetServerSideIdentificationCacheOfAccessGroup(int accessGroupId = default, string brandCode = default, long userId = default,
                int pageNumber = default, int pageSize = default, string token = default)
        {
            return _accessGroupRepository.GetServerSideIdentificationCacheOfAccessGroup(accessGroupId, brandCode,
                userId, pageNumber, pageSize, token);
        }

        public ResultViewModel<List<User>> GetAdminUserOfAccessGroup(long id = default, int accessGroupId = default, string token = default)
        {
            return _accessGroupRepository.GetAdminUserOfAccessGroup(id, accessGroupId, token);
        }

        public ResultViewModel AddAccessGroup(AccessGroup accessGroup = default, string token = default)
        {
            return _accessGroupRepository.AddAccessGroup(accessGroup, token);
        }

        public ResultViewModel ModifyAccessGroup(AccessGroup accessGroup = default, string token = default)
        {
            return _accessGroupRepository.ModifyAccessGroup(accessGroup, token);
        }

        public ResultViewModel ModifyAccessGroupAdminUsers(string xmlAdminUsers = default, int accessGroupId = default, string token = default)
        {
            return _accessGroupRepository.ModifyAccessGroupAdminUsers(xmlAdminUsers, accessGroupId, token);
        }

        public ResultViewModel ModifyAccessGroupDeviceGroup(string xmlDeviceGroup = default, int accessGroupId = default, string token = default)
        {
            return _accessGroupRepository.ModifyAccessGroupDeviceGroup(xmlDeviceGroup, accessGroupId, token);
        }

        public ResultViewModel ModifyAccessGroupUserGroup(string xmlUserGroup = default, int accessGroupId = default, string token = default)
        {
            return _accessGroupRepository.ModifyAccessGroupUserGroup(xmlUserGroup, accessGroupId, token);
        }
        public ResultViewModel DeleteAccessGroup(int id = default, string token = default)
        {
            return _accessGroupRepository.DeleteAccessGroup(id, token);
        }

        public void SendAccessGroupToDevice(DeviceBasicInfo device, int id, string token = default)
        {
            _accessGroupRepository.SendAccessGroupToDevice(device, id, token);
        }
    }
}
