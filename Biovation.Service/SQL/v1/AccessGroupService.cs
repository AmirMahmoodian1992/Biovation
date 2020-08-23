using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository;

namespace Biovation.Service
{
    public class AccessGroupService
    {
        private readonly AccessGroupRepository _accessGroupRepository;

        public AccessGroupService(AccessGroupRepository accessGroupRepository)
        {
            _accessGroupRepository = accessGroupRepository;
        }

        public ResultViewModel ModifyAccessGroup(AccessGroup accessGroup, string deviceGroup, string userGroup, string adminUsers)
        {
            var saved = _accessGroupRepository.ModifyAccessGroup(accessGroup);
            if (saved.Validate != 1)
                return new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };

            var dsaved = _accessGroupRepository.ModifyAccessGroupDeviceGroup(deviceGroup, (int)saved.Id);
            if (dsaved.Validate != 1)
                return new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };

            var asaved = _accessGroupRepository.ModifyAccessGroupAdminUsers(adminUsers, (int)saved.Id);
            if (asaved.Validate != 1)
                return new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };

            var gsaved = _accessGroupRepository.ModifyAccessGroupUserGroup(userGroup, (int)saved.Id);
            return gsaved;


        }

        public List<AccessGroup> GetAllAccessGroups(long userId = 0, int getNestingLevel = 3)
        {
            return _accessGroupRepository.GetAccessGroups(userId, getNestingLevel);
        }

        public List<AccessGroup> GetAccessGroupsByFilter(int adminUserId = 0, int userGroupId = 0, int id = 0, int deviceId = 0, int userId = 0)
        {
            return _accessGroupRepository.GetAccessGroupsByFilter(adminUserId,userGroupId,id,deviceId,userId);

        }

        public List<AccessGroup> SearchAccessGroups(int accessGroupId, int deviceGroupId, int userId)
        {
            return _accessGroupRepository.SearchAccessGroup(accessGroupId, deviceGroupId, userId);
        }

        public AccessGroup GetAccessGroupById(int accessGroupId, int getNestingLevel = 3)
        {
            //var deviceGroupRepository = new DeviceGroupRepository();
            //var userGroupRepository = new UserGroupRepository();

            var accessGroup = _accessGroupRepository.GetAccessGroup(accessGroupId, getNestingLevel);

            //if (accessGroup != null)
            //{
            //    var deviceGroup = deviceGroupRepository.GetDeviceGroupsByAccessGroup(accessGroup.Id);
            //    accessGroup.DeviceGroup = deviceGroup;
            //    var userGroup = userGroupRepository.GetUserGroupsByAccessGroup(accessGroup.Id);
            //    accessGroup.UserGroup = userGroup;                
            //}

            return accessGroup;
        }

        public List<AccessGroup> GetAccessGroupsOfUser(long userId, int getNestingLevel = 3)
        {
            //var deviceGroupRepository = new DeviceGroupRepository();
            //var userGroupRepository = new UserGroupRepository();

            var accessGroups = _accessGroupRepository.GetAccessGroupsOfUser(userId, getNestingLevel);

            //foreach (var accessGroup in accessGroups)
            //{
            //    var deviceGroup = deviceGroupRepository.GetDeviceGroupsByAccessGroup(accessGroup.Id);
            //    accessGroup.DeviceGroup = deviceGroup;
            //    var userGroup = userGroupRepository.GetUserGroupsByAccessGroup(accessGroup.Id);
            //    accessGroup.UserGroup = userGroup;
            //}

            return accessGroups;
        }

        public List<AccessGroup> GetAccessGroupsOfDevice(uint deviceId, int getNestingLevel = 3)
        {
            //var deviceGroupRepository = new DeviceGroupRepository();
            //var userGroupRepository = new UserGroupRepository();

            var accessGroups = _accessGroupRepository.GetAccessGroupsOfDevice(deviceId, getNestingLevel);

            //foreach (var accessGroup in accessGroups)
            //{
            //    var deviceGroup = deviceGroupRepository.GetDeviceGroupsByAccessGroup(accessGroup.Id);
            //    accessGroup.DeviceGroup = deviceGroup;
            //    var userGroup = userGroupRepository.GetUserGroupsByAccessGroup(accessGroup.Id);
            //    accessGroup.UserGroup = userGroup;
            //}

            return accessGroups;
        }

        public List<AccessGroup> GetAccessGroupsOfUserGroup(int userGroupId, int getNestingLevel = 3)
        {
            //var deviceGroupRepository = new DeviceGroupRepository();
            //var userGroupRepository = new UserGroupRepository();

            var accessGroups = _accessGroupRepository.GetAccessGroupsOfUserGroup(userGroupId, getNestingLevel);

            //foreach (var accessGroup in accessGroups)
            //{
            //    var deviceGroup = deviceGroupRepository.GetDeviceGroupsByAccessGroup(accessGroup.Id);
            //    accessGroup.DeviceGroup = deviceGroup;
            //    var userGroup = userGroupRepository.GetUserGroupsByAccessGroup(accessGroup.Id);
            //    accessGroup.UserGroup = userGroup;
            //}

            return accessGroups;
        }

        public ResultViewModel DeleteAccessGroupById(int accessGroupId)
        {
            return _accessGroupRepository.DeleteAccessGroup(accessGroupId);
        }

        public List<DeviceBasicInfo> GetDeviceOfAccessGroup(int accessGroupId)
        {
            return _accessGroupRepository.GetDeviceOfAccessGroup(accessGroupId);
        }

        public List<ServerSideIdentificationCacheModel> GetServerSideIdentificationCacheNoTemplate(long userId = 0)
        {
            return _accessGroupRepository.GetServerSideIdentificationCacheNoTemplate(userId);
        }

        public List<ServerSideIdentificationCacheModel> GetServerSideIdentificationCacheOfAccessGroup(int accessGroupId, string brandCode, long userId = 0)
        {
            return _accessGroupRepository.GetServerSideIdentificationCacheOfAccessGroup(accessGroupId, brandCode, userId);
        }
    }
}
