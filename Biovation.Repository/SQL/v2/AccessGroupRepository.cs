using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.SQL.v2
{
    public class AccessGroupRepository
    {
        private readonly GenericRepository _repository;



        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        public ResultViewModel<PagingResult<AccessGroup>> GetAccessGroupsByFilter(int adminUserId = 0, int userGroupId = 0, int id = 0, int deviceId = 0, int userId = 0, int pageNumber = default, int pageSize = default, int nestingDepthLevel = 4)
        {
            var parameters = new List<SqlParameter>
            {

                new SqlParameter("@Id", id),
                new SqlParameter("@DeviceId ", deviceId),
                new SqlParameter("@UserId", userId ),
                new SqlParameter("@adminUserId", adminUserId),
                new SqlParameter("@UserGroupId", userGroupId),
                new SqlParameter("@PageNumber", pageNumber),
                new SqlParameter("@PageSize",pageSize)
            };
            return _repository.ToResultList<PagingResult<AccessGroup>>("SelectAccessGroupsByFilter", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public AccessGroupRepository(GenericRepository repository)
        {
            _repository = repository;
        }
        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="accessGroup"></param>
        /// <returns></returns>
        public ResultViewModel ModifyAccessGroup(AccessGroup accessGroup)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", accessGroup.Id),
                new SqlParameter("@Name", accessGroup.Name),
                new SqlParameter("@Description", accessGroup.Description),
                new SqlParameter("@TimeZoneId", accessGroup.TimeZone != null ? accessGroup.TimeZone.Id : 0)
            };

            return _repository.ToResultList<ResultViewModel>("ModifyAccessGroup", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel AddAccessGroup(AccessGroup accessGroup)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", accessGroup.Id),
                new SqlParameter("@Name", accessGroup.Name),
                new SqlParameter("@Description", accessGroup.Description),
                new SqlParameter("@TimeZoneId", accessGroup.TimeZone != null ? accessGroup.TimeZone.Id : 0)
            };

            return _repository.ToResultList<ResultViewModel>("InsertAccessGroup", parameters).Data.FirstOrDefault();
        }

        /// <summary>
        /// <En></En>
        /// <Fa>ذخیره جدول کاربران گروه دسترسی</Fa>
        /// </summary>
        /// <param name="xmlUserGroup">رشته آبجکت</param>
        /// <param name="accessGroupId">شناهس گرئه دسترسی</param>
        /// <returns></returns>
        public ResultViewModel ModifyAccessGroupUserGroup(string xmlUserGroup, int accessGroupId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Xml", xmlUserGroup),
                new SqlParameter("@AccessGroupId", accessGroupId)
            };
            return _repository.ToResultList<ResultViewModel>("ModifyAccessGroupUserGroup", parameters).Data.FirstOrDefault();
        }

        /// <summary>
        /// <En></En>
        /// <Fa>ذخیره ادمین های گروه دسترسی</Fa>
        /// </summary>
        /// <param name="xmlAdminUsers">رشته آبجکت</param>
        /// <param name="accessGroupId">شناهس گرئه دسترسی</param>
        /// <returns></returns>
        public ResultViewModel ModifyAccessGroupAdminUsers(string xmlAdminUsers, int accessGroupId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Xml", xmlAdminUsers),
                new SqlParameter("@AccessGroupId", accessGroupId)
            };
            return _repository.ToResultList<ResultViewModel>("ModifyAccessGroupAdminUser", parameters).Data.FirstOrDefault();
        }

        /// <summary>
        /// <En></En>
        /// <Fa>ذخیره جدول دستگاه گروه دسترسی</Fa>
        /// </summary>
        /// <param name="xmlDeviceGroup">رشته آبجکت</param>
        /// <param name="accessGroupId">شناسه گروه دسترسی</param>
        /// <returns></returns>
        public ResultViewModel ModifyAccessGroupDeviceGroup(string xmlDeviceGroup, int accessGroupId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Xml", xmlDeviceGroup),
                new SqlParameter("@AccessGroupId", accessGroupId)
            };
            return _repository.ToResultList<ResultViewModel>("ModifyAccessGroupDeviceGroup", parameters).Data.FirstOrDefault();
        }



        public ResultViewModel<PagingResult<AccessGroup>> GetAccessGroupsOfUser(long userId, int nestingDepthLevel)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };
            return _repository.ToResultList<PagingResult<AccessGroup>>($"SelectAccessGroupsByUserId{(nestingDepthLevel == 0 ? "" : "NestedProperties")}", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel<PagingResult<AccessGroup>> GetAccessGroupsOfDevice(uint deviceId, int nestingDepthLevel)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@DeviceId", SqlDbType.Int) {Value = deviceId}
            };
            return _repository.ToResultList<PagingResult<AccessGroup>>($"SelectAccessGroupsByDeviceId{(nestingDepthLevel == 0 ? "" : "NestedProperties")}", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel<PagingResult<AccessGroup>> GetAccessGroupsOfUserGroup(int userGroupId, int nestingDepthLevel)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserGroupId", SqlDbType.Int) {Value = userGroupId}
            };
            return _repository.ToResultList<PagingResult<AccessGroup>>($"SelectAccessGroupsByUserGroupId{(nestingDepthLevel == 0 ? "" : "NestedProperties")}", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }


        /// <summary>
        /// <En></En>
        /// <Fa></Fa>
        /// </summary>
        /// <param name="accessGroupId"></param>
        /// <param name="nestingDepthLevel"></param>
        /// <returns></returns>
        public ResultViewModel<AccessGroup> GetAccessGroup(int accessGroupId, int nestingDepthLevel)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", accessGroupId)
            };

            return _repository.ToResultList<AccessGroup>("SelectAccessGroupByID", parameters, fetchCompositions: true, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();

        }

        /// <summary>
        /// <En></En>
        /// <Fa></Fa>
        /// </summary>
        /// <param name="accessGroupId"></param>
        /// <param name="deviceGroupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResultViewModel<PagingResult<AccessGroup>> SearchAccessGroup(int accessGroupId, int deviceGroupId, int userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", accessGroupId),
                new SqlParameter("@DeviceGroupId", deviceGroupId),
                new SqlParameter("@UserId", userId)
            };
            return _repository.ToResultList<PagingResult<AccessGroup>>("SelectSearchAccessGroup", parameters, fetchCompositions: true).FetchFromResultList();
        }

        /// <summary>
        /// <En></En>
        /// <Fa></Fa>
        /// </summary>
        /// <param name="accessGroupId"></param>
        /// <returns></returns>
        public ResultViewModel DeleteAccessGroup(int accessGroupId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", accessGroupId)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteAccessGroupByID", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel<PagingResult<DeviceBasicInfo>> GetDeviceOfAccessGroup(int accessGroupId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", accessGroupId)
            };

            return _repository.ToResultList<PagingResult<DeviceBasicInfo>>("SelectDeviceOfAccessGroup", parameters, fetchCompositions: true).FetchFromResultList();
        }

        public ResultViewModel<PagingResult<ServerSideIdentificationCacheModel>> GetServerSideIdentificationCacheNoTemplate(long userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@adminUserId", userId)
            };

            return _repository.ToResultList<PagingResult<ServerSideIdentificationCacheModel>>("SelectServerSideIdentificationCacheNoTemplate", parameters, fetchCompositions: true).FetchFromResultList();
        }

        public ResultViewModel<PagingResult<ServerSideIdentificationCacheModel>> GetServerSideIdentificationCacheOfAccessGroup(int accessGroupId, string brandCode, long userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@adminUserId", userId),
                new SqlParameter("@brandCode", brandCode),
                new SqlParameter("@accessGroupId", accessGroupId)
            };

            return _repository
                .ToResultList<PagingResult<ServerSideIdentificationCacheModel>>(
                    "SelectServerSideIdentificationCacheOfAccessGroup", parameters, fetchCompositions: true,
                    compositionDepthLevel: 4).FetchFromResultList();
        }
    }
}

