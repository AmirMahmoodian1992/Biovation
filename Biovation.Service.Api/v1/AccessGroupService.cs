using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;

namespace Biovation.Service.Api.v1
{
    public class AccessGroupService
    {
        private readonly AccessGroupRepository _accessGroupRepository;

        public AccessGroupService(AccessGroupRepository accessGroupRepository)
        {
            _accessGroupRepository = accessGroupRepository;
        }

        public List<AccessGroup> GetAccessGroups(long userId = default, int adminUserId = default,
            int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default, int pageNumber = default, int pageSize = default, int nestingDepthLevel = 5, string token = default)
        {
            return _accessGroupRepository.GetAccessGroups(userId, userGroupId, id, deviceId, deviceGroupId,
                pageNumber, pageSize, nestingDepthLevel).Result?.Data?.Data ?? new List<AccessGroup>();
        }

        public AccessGroup GetAccessGroup(int id = default, int nestingDepthLevel = 5, string token = default)
        {
            return _accessGroupRepository.GetAccessGroup(id, nestingDepthLevel).Result?.Data ?? new AccessGroup();
        }

        public List<User> GetAdminUserOfAccessGroup(long userId = default, int accessGroupId = default, string token = default)
        {
            return _accessGroupRepository.GetAdminUserOfAccessGroup(userId, accessGroupId, token).Result?.Data ?? new List<User>();
        }

        public List<DeviceBasicInfo> GetDeviceOfAccessGroup(int accessGroupId = default,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            return _accessGroupRepository.GetDeviceOfAccessGroup(accessGroupId, pageNumber, pageSize).Result?.Data?.Data ?? new List<DeviceBasicInfo>();
        }

        public List<ServerSideIdentificationCacheModel> GetServerSideIdentificationCacheOfAccessGroup(int accessGroupId = default, string brandCode = default, long userId = default,
                int pageNumber = default, int pageSize = default, string token = default)
        {
            return _accessGroupRepository.GetServerSideIdentificationCacheOfAccessGroup(accessGroupId, brandCode,
                userId, pageNumber, pageSize, token)?.Data?.Data ?? new List<ServerSideIdentificationCacheModel>();
        }


        public ResultViewModel AddAccessGroup(AccessGroup accessGroup = default, string token = default)
        {
            return _accessGroupRepository.AddAccessGroup(accessGroup, token).Result;
        }

        public ResultViewModel ModifyAccessGroup(AccessGroup accessGroup = default, string token = default)
        {
            return _accessGroupRepository.ModifyAccessGroup(accessGroup, token).Result;
        }

        public ResultViewModel ModifyAccessGroupAdminUsers(string xmlAdminUsers = default, int accessGroupId = default, string token = default)
        {
            return _accessGroupRepository.ModifyAccessGroupAdminUsers(xmlAdminUsers, accessGroupId, token).Result;
        }

        public ResultViewModel ModifyAccessGroupDeviceGroup(string xmlDeviceGroup = default, int accessGroupId = default, string token = default)
        {
            return _accessGroupRepository.ModifyAccessGroupDeviceGroup(xmlDeviceGroup, accessGroupId, token).Result;
        }

        public ResultViewModel ModifyAccessGroupUserGroup(string xmlUserGroup = default, int accessGroupId = default, string token = default)
        {
            return _accessGroupRepository.ModifyAccessGroupUserGroup(xmlUserGroup, accessGroupId, token).Result;
        }
        public ResultViewModel DeleteAccessGroup(int id = default, string token = default)
        {
            return _accessGroupRepository.DeleteAccessGroup(id, token).Result;
        }



    }
}
