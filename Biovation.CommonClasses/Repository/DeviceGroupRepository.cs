using Biovation.CommonClasses.Models;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;

namespace Biovation.CommonClasses.Repository
{

    public class DeviceGroupRepository
    {
        private readonly GenericRepository _repository;

        public DeviceGroupRepository()
        {
            _repository = new GenericRepository();
        }


        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک گروه را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="deviceGroupId">کد گروه</param>
        /// <returns></returns>
        public List<DeviceGroup> GetDeviceGroups(int? deviceGroupId, long userId)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@Id", SqlDbType.Int) { Value = deviceGroupId },
                new SqlParameter("@AdminUserId", SqlDbType.BigInt) { Value = userId },

            };

            return _repository.ToResultList<DeviceGroup>("SelectDeviceGroupById", parameters,
                    fetchCompositions: true).Data;
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

        public List<DeviceGroup> GetAccessControlDeviceGroup(int id)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@AccessControlId",id)
                };

            return _repository.ToResultList<DeviceGroup>("SelectAccessControlDeviceGroup", parameters).Data;
        }

        public List<DeviceGroup> GetDeviceGroupsByAccessGroup(int accessGroupId)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@AccessGroupId", accessGroupId)
                };

            return _repository.ToResultList<DeviceGroup>("SelectDeviceGroupsByAccessGroupId", parameters, fetchCompositions: true).Data;
        }
    }
}
