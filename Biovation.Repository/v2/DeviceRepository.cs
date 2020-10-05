using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.Sql.v2
{
    public class DeviceRepository
    {
        private readonly GenericRepository _repository;


        public DeviceRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// <En>Get all devices from database.</En>
        /// <Fa>اطلاعات تمامی دستگاه ها را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        /// 

        public ResultViewModel<DeviceBasicInfo> GetDevice(long id, int adminUserId = 0, int nestingDepthLevel = 4)
        {
            var sqlParameter = new List<SqlParameter>
                {
                new SqlParameter("@Id", SqlDbType.Int) {Value = id }  ,
                new SqlParameter("@AdminUserId", SqlDbType.Int) {Value = adminUserId }
                };
            //return _repository.ToResultList<ResultViewModel<DeviceBasicInfo>>("SelectDeviceBasicInfoById", sqlParameter,
            //     fetchCompositions: true).Data.FirstOrDefault();

            return _repository.ToResultList<DeviceBasicInfo>("SelectDeviceBasicInfoById", sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();

        }

        public ResultViewModel<PagingResult<DeviceBasicInfo>> GetDevices(long adminUserId = 0, int groupId = 0, uint code = 0,
            int brandId = 0, string name = null, int modelId = 0, int deviceIoTypeId = 0, int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4)
        {

            var sqlParameter = new List<SqlParameter>
                {
                new SqlParameter("@AdminUserId", SqlDbType.Int) {Value = adminUserId },
                new SqlParameter("@GroupId", SqlDbType.Int) {Value = groupId },
                new SqlParameter("@DeviceCode", SqlDbType.Int) {Value = code},
                new SqlParameter("@DeviceBrandId", SqlDbType.Int) {Value = brandId},
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = name},
                new SqlParameter("@DeviceModelId", SqlDbType.Int) {Value = modelId},
                new SqlParameter("@DeviceTypeId", SqlDbType.Int) {Value = deviceIoTypeId},
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize},
                };
            return _repository.ToResultList<PagingResult<DeviceBasicInfo>>($"SelectDevicesByFilter", sqlParameter, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel AddDevice(DeviceBasicInfo device)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = device.DeviceId },
                new SqlParameter("@Code", SqlDbType.Int) { Value = device.Code },
                new SqlParameter("@DeviceModelID", SqlDbType.Int) { Value = device.Model?.Id ?? 0 },
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value = device.Name },
                new SqlParameter("@Active", SqlDbType.Bit) { Value = device.Active },
                new SqlParameter("@IPAddress", SqlDbType.NVarChar) { Value = device.IpAddress },
                new SqlParameter("@Port", SqlDbType.Int) { Value = device.Port },
                new SqlParameter("@MacAddress", SqlDbType.NVarChar) { Value = device.MacAddress },
                new SqlParameter("@SSL", SqlDbType.Bit) { Value = device.SSL },
                new SqlParameter("@TimeSync", SqlDbType.Bit) { Value = device.TimeSync },
                new SqlParameter("@DeviceTypeId", SqlDbType.Int) { Value = device.DeviceTypeId }
            };

            if (device.HardwareVersion != null)
            {
                parameters.Add(new SqlParameter("@HardwareVersion", SqlDbType.NVarChar) { Value = device.HardwareVersion });
            }

            if (device.FirmwareVersion != null)
            {
                parameters.Add(new SqlParameter("@FirmwareVersion", SqlDbType.NVarChar) { Value = device.FirmwareVersion });
            }

            if (device.DeviceLockPassword != null)
            {
                parameters.Add(new SqlParameter("@DeviceLockPassword", SqlDbType.NVarChar) { Value = device.DeviceLockPassword });
            }

            if (device.SerialNumber != null)
            {
                parameters.Add(new SqlParameter("@SerialNumber", SqlDbType.NVarChar) { Value = device.SerialNumber });
            }

            return _repository.ToResultList<ResultViewModel>("InsertDevice", parameters).Data.FirstOrDefault();

        }

        public ResultViewModel<PagingResult<Lookup>> GetDeviceBrands(int code = default, string name = default, int pageNumber = default, int PageSize = default, int nestingDepthLevel = 4)
        {
            var Parameter = new List<SqlParameter>
                {

                new SqlParameter("@Code", SqlDbType.Int) {Value = code},
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = name},
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = PageSize}
                };
            return _repository.ToResultList<PagingResult<Lookup>>("SelectDeviceBrandsByFilter", Parameter,
                     fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }
        public ResultViewModel<PagingResult<DeviceModel>> GetDeviceModels(long id = 0, string brandId = default, string name = default, int pageNumber = default, int PageSize = default)
        {

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) {Value = id },
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name },
                new SqlParameter("@DeviceBrandId ", SqlDbType.Int) { Value = brandId ?? 0.ToString() },
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = PageSize}
            };

            return _repository.ToResultList<PagingResult<DeviceModel>>("SelectDeviceModelsByFilter", parameters,
                fetchCompositions: true, compositionDepthLevel: 4).FetchFromResultList();
        }

        public ResultViewModel DeleteDevice(uint deviceId)
        {
            var parameters = new List<SqlParameter> { new SqlParameter("@ID", SqlDbType.Int) { Value = deviceId } };

            return _repository.ToResultList<ResultViewModel>("DeleteDeviceBasicInfoByID", parameters).Data.FirstOrDefault();
        }


        public ResultViewModel DeleteDevices(List<uint> deviceIds)
        {

            var parameters = new List<SqlParameter> { new SqlParameter("@json", SqlDbType.VarChar) { Value = JsonSerializer.Serialize(deviceIds) } };

            return _repository.ToResultList<ResultViewModel>("DeleteDevices", parameters).Data.FirstOrDefault();

        }

        public ResultViewModel AddDeviceModel(DeviceModel deviceModel)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = deviceModel.Id },
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value = deviceModel.Name },
                new SqlParameter("@brandId", SqlDbType.Int) { Value = deviceModel.Brand.Code },
                new SqlParameter("@ManufactureCode", SqlDbType.Int) { Value = deviceModel.ManufactureCode },
                new SqlParameter("@Description", SqlDbType.NVarChar) { Value = deviceModel.Description },
                new SqlParameter("@DefaultPortNumber", SqlDbType.NVarChar) { Value = deviceModel.DefaultPortNumber },

            };

            return _repository.ToResultList<ResultViewModel>("InsertDeviceModel", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel ModifyDeviceBasicInfo(DeviceBasicInfo device)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = device.DeviceId },
                new SqlParameter("@Code", SqlDbType.Int) { Value = device.Code },
                new SqlParameter("@DeviceModelID", SqlDbType.Int) { Value = device.Model?.Id ?? 0 },
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value = device.Name },
                new SqlParameter("@Active", SqlDbType.Bit) { Value = device.Active },
                new SqlParameter("@IPAddress", SqlDbType.NVarChar) { Value = device.IpAddress },
                new SqlParameter("@Port", SqlDbType.Int) { Value = device.Port },
                new SqlParameter("@MacAddress", SqlDbType.NVarChar) { Value = device.MacAddress },
                //new SqlParameter("@RegisterDate", SqlDbType.SmallDateTime) { Value = device.RegisterDate },
                new SqlParameter("@SSL", SqlDbType.Bit) { Value = device.SSL },
                new SqlParameter("@TimeSync", SqlDbType.Bit) { Value = device.TimeSync },
                new SqlParameter("@DeviceTypeId", SqlDbType.Int) { Value = device.DeviceTypeId }
            };

            if (device.HardwareVersion != null)
            {
                parameters.Add(new SqlParameter("@HardwareVersion", SqlDbType.NVarChar) { Value = device.HardwareVersion });
            }

            if (device.FirmwareVersion != null)
            {
                parameters.Add(new SqlParameter("@FirmwareVersion", SqlDbType.NVarChar) { Value = device.FirmwareVersion });
            }

            if (device.DeviceLockPassword != null)
            {
                parameters.Add(new SqlParameter("@DeviceLockPassword", SqlDbType.NVarChar) { Value = device.DeviceLockPassword });
            }

            if (device.SerialNumber != null)
            {
                parameters.Add(new SqlParameter("@SerialNumber", SqlDbType.NVarChar) { Value = device.SerialNumber });
            }

            return _repository.ToResultList<ResultViewModel>("ModifyDeviceBasicInfo", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel<AuthModeMap> GetBioAuthModeWithDeviceId(int deviceId, int authMode)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@DeviceId", SqlDbType.Int) { Value = deviceId },
                new SqlParameter("@AuthMode", SqlDbType.Int) { Value = authMode },
            };
            return _repository.ToResultList<AuthModeMap>("SelectBioAuthModeByDeviceId", parameters).FetchFromResultList();
        }

        public ResultViewModel<DateTime> GetLastConnectedTime(uint deviceId)
        {
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@deviceId", SqlDbType.Int) { Value = deviceId }
                };

                return _repository.ToResultList<DateTime>("SelectLastConnectionTime", parameters).FetchFromResultList();
                //var result = _repository.ToResultList<string>("SelectLastConnectionTime", parameters).Data.FirstOrDefault();
                //return DateTime.Parse(result);
            }
        }
        public ResultViewModel<PagingResult<User>> GetAuthorizedUsersOfDevice(int deviceId, int nestingDepthLevel = 4)
        {

            var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@DeviceId", deviceId)
                };

            return _repository.ToResultList<PagingResult<User>>("SelectAuthorizedUsersOfDevice", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();

        }

        public ResultViewModel AddNetworkConnectionLog(DeviceBasicInfo device)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@deviceId", SqlDbType.Int) { Value = device.DeviceId },
                new SqlParameter("@UpdateDate", SqlDbType.DateTime) { Value = DateTime.Now },
                new SqlParameter("@@IPAddress", SqlDbType.NVarChar) { Value = device.IpAddress }
            };

            return _repository.ToResultList<ResultViewModel>("InsertNetworkConnectionLog", parameters).Data.FirstOrDefault();
        }


    }
}