using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.API.v2;

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
            int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default, int pageNumber = default, int pageSize = default)
        {
            return _accessGroupRepository.GetAccessGroups(userId, adminUserId, userGroupId, id, deviceId, deviceGroupId,
                pageNumber, pageSize).Data.Data;
        }

        public AccessGroup GetAccessGroup(int id = default, int nestingDepthLevel = default)
        {
            return _accessGroupRepository.GetAccessGroup(id, nestingDepthLevel).Data;
        }

        public List<DeviceBasicInfo> GetDeviceOfAccessGroup(int accessGroupId = default,
            int pageNumber = default, int pageSize = default)
        {
            return _accessGroupRepository.GetDeviceOfAccessGroup(accessGroupId, pageNumber, pageSize).Data.Data;
        }

        public ResultViewModel<PagingResult<ServerSideIdentificationCacheModel>>
            GetServerSideIdentificationCacheOfAccessGroup(int accessGroupId = default, string brandCode = default, long userId = default,
                int pageNumber = default, int pageSize = default)
        {
            return _accessGroupRepository.GetServerSideIdentificationCacheOfAccessGroup(accessGroupId, brandCode,
                userId, pageNumber, pageSize);
        }


        public ResultViewModel AddAccessGroup(AccessGroup accessGroup = default)
        {
            return _accessGroupRepository.AddAccessGroup(accessGroup);
        }

        public ResultViewModel ModifyAccessGroup(AccessGroup accessGroup = default)
        {
            return _accessGroupRepository.ModifyAccessGroup(accessGroup);
        }

        public ResultViewModel ModifyAccessGroupAdminUsers(string xmlAdminUsers = default, int accessGroupId = default)
        {
            return _accessGroupRepository.ModifyAccessGroupAdminUsers(xmlAdminUsers, accessGroupId);
        }

        public ResultViewModel ModifyAccessGroupDeviceGroup(string xmlDeviceGroup = default, int accessGroupId = default)
        {
            return _accessGroupRepository.ModifyAccessGroupDeviceGroup(xmlDeviceGroup, accessGroupId);
        }

        public ResultViewModel ModifyAccessGroupUserGroup(string xmlUserGroup = default, int accessGroupId = default)
        {
            return _accessGroupRepository.ModifyAccessGroupUserGroup(xmlUserGroup, accessGroupId);
        }
        public ResultViewModel DeleteAccessGroup(int id = default)
        {
            return _accessGroupRepository.DeleteAccessGroup(id);
        }



    }
}
