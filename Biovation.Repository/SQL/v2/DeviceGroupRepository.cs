using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using Newtonsoft.Json;

namespace Biovation.Repository.SQL.v2
{

    public class DeviceGroupRepository
    {
        private readonly GenericRepository _repository;

        public DeviceGroupRepository(GenericRepository repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک گروه را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="deviceGroupId">کد گروه</param>
        /// <returns></returns>
        public ResultViewModel<PagingResult<DeviceGroup>> GetDeviceGroups(int? deviceGroupId, long userId, int pageNumber = 0, int PageSize = 0, int nestingDepthLevel = 4)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@Id", SqlDbType.Int) { Value = deviceGroupId },
                new SqlParameter("@AdminUserId", SqlDbType.BigInt) { Value = userId },
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = PageSize},
            };
            return _repository.ToResultList< PagingResult<DeviceGroup>>("SelectDeviceGroupById", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
            
        }


        public ResultViewModel ModifyDeviceGroup(DeviceGroup deviceGroup)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@Id",deviceGroup.Id),
                new SqlParameter("@Name",deviceGroup.Name),
                new SqlParameter("@Description",deviceGroup.Description),
                new SqlParameter("@Devices", JsonConvert.SerializeObject(deviceGroup.Devices.Select(device => new {device.DeviceId} )))
                };

            return _repository.ToResultList<ResultViewModel>("InsertDeviceGroup", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel ModifyDeviceGroupMember(string node, int groupId)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@DeviceGroupMember",node),
                new SqlParameter("@DeviceGroupId",groupId)
            };
            return _repository.ToResultList<ResultViewModel>("InsertDeviceGroupMember", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteDeviceGroup(int id)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@GroupId",id)
                };

            return _repository.ToResultList<ResultViewModel>("DeleteGroupDevice", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteDeviceGroupMember(uint id)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@GroupId",id)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteGroupDeviceMemeber", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel<PagingResult<DeviceGroup>> GetAccessControlDeviceGroup(int id, int pageNumber = 0, int PageSize = 0, int nestingDepthLevel = 4)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@AccessControlId", id),
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = PageSize}
            };

            return _repository.ToResultList< PagingResult<DeviceGroup>>("SelectAccessControlDeviceGroup", parameters).FetchFromResultList();
        }

        public ResultViewModel<PagingResult<DeviceGroup>> GetDeviceGroupsByAccessGroup(int accessGroupId, int pageNumber = 0, int PageSize = 0, int nestingDepthLevel = 4)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@AccessGroupId", accessGroupId),
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
            new SqlParameter("@PageSize", SqlDbType.Int) {Value = PageSize}
            };
            return _repository.ToResultList<PagingResult<DeviceGroup>>("SelectDeviceGroupsByAccessGroupId", parameters, fetchCompositions: true).FetchFromResultList();
        }
    }
}
