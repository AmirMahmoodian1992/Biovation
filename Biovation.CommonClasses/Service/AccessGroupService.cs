using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;
using System.Collections.Generic;

namespace Biovation.CommonClasses.Service
{
    public class AccessGroupService
    {
        public ResultViewModel ModifyAccessGroup(AccessGroup accessGroup, string deviceGroup, string userGroup, string adminUsers)
        {
            var accessGroupRepository = new AccessGroupRepository();
            var saved = accessGroupRepository.ModifyAccessGroup(accessGroup);
            if (saved.Validate != 1)
                return new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };

            var dsaved = accessGroupRepository.ModifyAccessGroupDeviceGroup(deviceGroup, (int)saved.Id);
            if (dsaved.Validate != 1)
                return new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };

            var asaved = accessGroupRepository.ModifyAccessGroupAdminUsers(adminUsers, (int)saved.Id);
            if (asaved.Validate != 1)
                return new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };

            var gsaved = accessGroupRepository.ModifyAccessGroupUserGroup(userGroup, (int)saved.Id);
            return gsaved;


        }

        public List<AccessGroup> GetAllAccessGroups(long userId = 0, int getNestingLevel = 3)
        {
            var accessGroupRepository = new AccessGroupRepository();
            return accessGroupRepository.GetAccessGroups(userId, getNestingLevel);
        }

        public List<AccessGroup> GetAccessGroupsByFilter(int adminUserId = 0, int userGroupId = 0, int id = 0, int deviceId = 0, int userId = 0)
        {
            var accessGroupRepository = new AccessGroupRepository();
            return accessGroupRepository.GetAccessGroupsByFilter(adminUserId,userGroupId,id,deviceId,userId);

        }

        public List<AccessGroup> SearchAccessGroups(int accessGroupId, int deviceGroupId, int userId)
        {
            var accessGroupRepository = new AccessGroupRepository();
            return accessGroupRepository.SearchAccessGroup(accessGroupId, deviceGroupId, userId);
        }

        public AccessGroup GetAccessGroupById(int accessGroupId, int getNestingLevel = 3)
        {
            var accessGroupRepository = new AccessGroupRepository();
            //var deviceGroupRepository = new DeviceGroupRepository();
            //var userGroupRepository = new UserGroupRepository();

            var accessGroup = accessGroupRepository.GetAccessGroup(accessGroupId, getNestingLevel);

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
            var accessGroupRepository = new AccessGroupRepository();
            //var deviceGroupRepository = new DeviceGroupRepository();
            //var userGroupRepository = new UserGroupRepository();

            var accessGroups = accessGroupRepository.GetAccessGroupsOfUser(userId, getNestingLevel);

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
            var accessGroupRepository = new AccessGroupRepository();
            //var deviceGroupRepository = new DeviceGroupRepository();
            //var userGroupRepository = new UserGroupRepository();

            var accessGroups = accessGroupRepository.GetAccessGroupsOfDevice(deviceId, getNestingLevel);

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
            var accessGroupRepository = new AccessGroupRepository();
            //var deviceGroupRepository = new DeviceGroupRepository();
            //var userGroupRepository = new UserGroupRepository();

            var accessGroups = accessGroupRepository.GetAccessGroupsOfUserGroup(userGroupId, getNestingLevel);

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
            var accessGroupRepository = new AccessGroupRepository();
            return accessGroupRepository.DeleteAccessGroup(accessGroupId);
        }

        public List<DeviceBasicInfo> GetDeviceOfAccessGroup(int accessGroupId)
        {
            var accessGroupRepository = new AccessGroupRepository();
            return accessGroupRepository.GetDeviceOfAccessGroup(accessGroupId);
        }

        public List<ServerSideIdentificationCacheModel> GetServerSideIdentificationCacheNoTemplate(long userId = 0)
        {
            var accessGroupRepository = new AccessGroupRepository();
            return accessGroupRepository.GetServerSideIdentificationCacheNoTemplate(userId);
        }

        public List<ServerSideIdentificationCacheModel> GetServerSideIdentificationCacheOfAccessGroup(int accessGroupId, string brandCode, long userId = 0)
        {
            var accessGroupRepository = new AccessGroupRepository();
            return accessGroupRepository.GetServerSideIdentificationCacheOfAccessGroup(accessGroupId, brandCode, userId);
        }
    }
}
