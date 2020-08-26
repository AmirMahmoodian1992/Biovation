using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository
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
        public List<DeviceBasicInfo> GetAllDevicesBasicInfos(long adminUserId = 0)
        {
            var sqlParameter = new List<SqlParameter>
                {
            new SqlParameter("@AdminUserId", SqlDbType.Int) {Value = adminUserId }
                };
            return _repository.ToResultList<DeviceBasicInfo>("SelectDeviceBasicInfos", sqlParameter,
                    fetchCompositions: true).Data;
        }
        public List<DeviceBasicInfo> GetAllDevicesBasicInfosByFilter(long adminUserId = 0, int deviceGroupId = 0, uint Code = 0, int deviceId = 0, int brandId = 0, string deviceName = null, int deviceModelId = 0)
        {
            var sqlParameter = new List<SqlParameter>
            {
                new SqlParameter("@GroupId", SqlDbType.Int) {Value = deviceGroupId},
                new SqlParameter("@AdminUserId", SqlDbType.Int) {Value = adminUserId },
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = deviceName },
                new SqlParameter("@Code", SqlDbType.Int) { Value = Code },
                new SqlParameter("@Id", SqlDbType.Int) { Value = deviceId },
                new SqlParameter("@DeviceBrandId", SqlDbType.Int) { Value = brandId },
                new SqlParameter("@DeviceModelId", SqlDbType.Int) {Value = deviceModelId }
            };
            return _repository.ToResultList<DeviceBasicInfo>("SelectDeviceBasicInfosByFilter", sqlParameter,
                fetchCompositions: true).Data;
        }


        public List<DeviceBasicInfo> GetDevicesByFilter(/*long id = 0,*/ long adminUserId = 0, int groupId = 0, uint code = 0,
            int brandId = 0, string name = null, int modelId = 0, int typeId = 0)
        {
            var sqlParameter = new List<SqlParameter>
            {
                /*new SqlParameter("@Id", SqlDbType.Int) {Value = id},*/
                new SqlParameter("@AdminUserId", SqlDbType.Int) {Value = adminUserId },
                new SqlParameter("@GroupId", SqlDbType.Int) {Value = groupId},
                new SqlParameter("@Code", SqlDbType.Int) { Value = code },
                new SqlParameter("@DeviceBrandId", SqlDbType.Int) { Value = brandId },
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = name },
                new SqlParameter("@DeviceModelId", SqlDbType.Int) {Value = modelId },
                new SqlParameter("@DeviceTypeId", SqlDbType.Int) {Value = typeId }
            };
            return _repository.ToResultList<DeviceBasicInfo>("SelectDevicesByFilter", sqlParameter,
                fetchCompositions: true).Data;
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

            return _repository.ToResultList<ResultViewModel>("AddDevice", parameters).Data.FirstOrDefault();

        }


        public List<DeviceBasicInfo> GetDevicesBasicInfosByFilter(string deviceName, int deviceModelId, int deviceTypeId, long adminUserId = 0)
        {
            var sqlParameter = new List<SqlParameter>
            {
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = deviceName },
                new SqlParameter("@deviceModelId", SqlDbType.Int) {Value = deviceModelId },
                new SqlParameter("@deviceTypeId", SqlDbType.Int) {Value = deviceTypeId },
                new SqlParameter("@AdminUserId", SqlDbType.Int) {Value = adminUserId }
            };
            return _repository.ToResultList<DeviceBasicInfo>("selectSearchedDevice", sqlParameter,
                fetchCompositions: true).Data;
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک دستگاه را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="deviceId">کد دستگاه</param>
        /// <param name="adminUserId"></param>
        /// <returns></returns>
        public DeviceBasicInfo GetDeviceBasicInfo(int deviceId, int adminUserId = 0)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@ID", SqlDbType.Int) { Value = deviceId },
                new SqlParameter("@AdminUserId", SqlDbType.Int) {Value = adminUserId }
            };

            return _repository.ToResultList<DeviceBasicInfo>("SelectDeviceBasicInfoByID", parameters,
                    fetchCompositions: true).Data.FirstOrDefault();
        }

        public List<Lookup> GetDeviceBrands()
        {
            return _repository.ToResultList<Lookup>("SelectDeviceBrands", null,
                    fetchCompositions: true).Data;
        }

        public Lookup GetDeviceBrandById(string brandId)
        {
            var parameters = new List<SqlParameter> { new SqlParameter("@Id", SqlDbType.Int) { Value = brandId } };

            return _repository.ToResultList<Lookup>("SelectDeviceBrandById", parameters,
                    fetchCompositions: true).Data.FirstOrDefault();
        }

        public List<DeviceBasicInfo> GetDevicesBasicInfosByName(string name, int adminUserId = 0)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name },
            new SqlParameter("@AdminUserId", SqlDbType.Int) {Value = adminUserId }
            };

            return _repository.ToResultList<DeviceBasicInfo>("SelectDeviceBasicInfosLikeName", parameters,
                    fetchCompositions: true).Data;
        }

        public List<DeviceBasicInfo> GetAllDevicesBasicInfosByBrandId(string brandId, int adminUserId = 0)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@DeviceBrandId", brandId),
                new SqlParameter("@AdminUserId", SqlDbType.Int) { Value = adminUserId }

            };

            return _repository.ToResultList<DeviceBasicInfo>("SelectDeviceBasicInfosByBrandId", parameters,
                    fetchCompositions: true).Data;
        }

        public List<DeviceBasicInfo> GetAllDevicesBasicInfosByDeviceModelId(int modelId, int adminUserId = 0)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@DeviceModelID", SqlDbType.Int) { Value = modelId },
                new SqlParameter("@AdminUserId", SqlDbType.Int) { Value = adminUserId }
            };

            return _repository.ToResultList<DeviceBasicInfo>("SelectDeviceBasicInfosByDeviceModelID", parameters,
                    fetchCompositions: true).Data;
        }

        public DeviceBasicInfo GetDeviceBasicInfoByIdAndBrandId(int deviceId, string brandId, int adminUserId = 0)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = deviceId },
                new SqlParameter("@deviceBrandId", SqlDbType.Int) { Value = brandId },
                new SqlParameter("@AdminUserId", SqlDbType.Int) { Value = adminUserId }

            };

            return _repository.ToResultList<DeviceBasicInfo>("SelectDeviceBasicInfoByIdAndBrandId", parameters,
                    fetchCompositions: true).Data.FirstOrDefault();
        }

        public DeviceBasicInfo GetDeviceBasicInfoWithCode(uint code, string brandId, int adminUserId = 0)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Code", SqlDbType.Int) { Value = code },
                new SqlParameter("@deviceBrandId", SqlDbType.Int) { Value = brandId },
               new SqlParameter("@AdminUserId", SqlDbType.Int) { Value = adminUserId }

            };

            return _repository.ToResultList<DeviceBasicInfo>("SelectDeviceBasicInfoWithCode", parameters,
                    fetchCompositions: true).Data.FirstOrDefault();
        }

        public DeviceBasicInfo GetDeviceBasicInfoByIdAndDeviceModelId(int deviceId, int modelId, int adminUserId = 0)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = deviceId },
                new SqlParameter("@DeviceModelID", SqlDbType.Int) { Value = modelId },
                new SqlParameter("@AdminUserId", SqlDbType.Int) { Value = adminUserId }

            };

            return _repository.ToResultList<DeviceBasicInfo>("SelectDeviceBasicInfoByIdAndDeviceModelId", parameters,
                    fetchCompositions: true).Data.FirstOrDefault();
        }

        public DeviceModel GetDeviceModelByName(string name)
        {
            var parameters = new List<SqlParameter> { new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name } };

            return _repository.ToResultList<DeviceModel>("SelectDeviceModelByName", parameters,
                    fetchCompositions: true).Data.FirstOrDefault();
        }

        public List<DeviceModel> GetDeviceModelsByBrandId(string brandId)
        {
            var parameters = new List<SqlParameter> { new SqlParameter("@Id", SqlDbType.Int) { Value = brandId ?? 0.ToString() } };

            return _repository.ToResultList<DeviceModel>("SelectDeviceModelsByBrandId", parameters,
                    fetchCompositions: true).Data;
        }
        public List<DeviceModel> GetDeviceModelsByFilter(string brandId = default, string name = default)
        {

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name },
                new SqlParameter("@DeviceBrandId ", SqlDbType.Int) { Value = brandId ?? 0.ToString() }
            };

            return _repository.ToResultList<DeviceModel>("SelectDeviceModelsByFilter", parameters,
                fetchCompositions: true).Data;
        }

        public ResultViewModel DeleteDevice(uint deviceId)
        {
            var parameters = new List<SqlParameter> { new SqlParameter("@ID", SqlDbType.Int) { Value = deviceId } };

            return _repository.ToResultList<ResultViewModel>("DeleteDeviceBasicInfoByID", parameters).Data.FirstOrDefault();
        }

        //public ResultViewModel AddDevice(DeviceBasicInfo device)
        //{
        //    var parameters = new List<SqlParameter>
        //    {
        //        new SqlParameter("@ID", SqlDbType.Int) { Value = device.DeviceId },
        //        new SqlParameter("@Code", SqlDbType.Int) { Value = device.Code },
        //        new SqlParameter("@DeviceModelID", SqlDbType.Int) { Value = device.Model.Id },
        //        new SqlParameter("@Name", SqlDbType.NVarChar) { Value = device.Name },
        //        new SqlParameter("@Active", SqlDbType.Bit) { Value = device.Active },
        //        new SqlParameter("@IPAddress", SqlDbType.NVarChar) { Value = device.IpAddress },
        //        new SqlParameter("@Port", SqlDbType.Int) { Value = device.Port },
        //        new SqlParameter("@RegisterDate", SqlDbType.SmallDateTime) { Value = device.RegisterDate },
        //        new SqlParameter("@SSL", SqlDbType.Bit) { Value = device.SSL },
        //        new SqlParameter("@TimeSync", SqlDbType.Bit) { Value = device.TimeSync },
        //        new SqlParameter("@DeviceTypeId", SqlDbType.Int) { Value = device.DeviceTypeId }
        //    };

        //    if (device.MacAddress != null)
        //    {
        //        parameters.Add(new SqlParameter("@MacAddress", SqlDbType.NVarChar) { Value = device.MacAddress });
        //    }

        //    if (device.HardwareVersion != null)
        //    {
        //        parameters.Add(new SqlParameter("@HardwareVersion", SqlDbType.NVarChar) { Value = device.HardwareVersion });
        //    }

        //    if (device.FirmwareVersion != null)
        //    {
        //        parameters.Add(new SqlParameter("@FirmwareVersion", SqlDbType.NVarChar) { Value = device.FirmwareVersion });
        //    }

        //    if (device.DeviceLockPassword != null)
        //    {
        //        parameters.Add(new SqlParameter("@DeviceLockPassword", SqlDbType.NVarChar) { Value = device.DeviceLockPassword });
        //    }

        //    if (device.SerialNumber != null)
        //    {
        //        parameters.Add(new SqlParameter("@SerialNumber", SqlDbType.NVarChar) { Value = device.SerialNumber });
        //    }

        //    return _repository.ToResultList<ResultViewModel>("InsertDeviceBasicInfo", parameters).Data.FirstOrDefault();
        //}

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

        public ResultViewModel ModifyDeviceBasicInfoByID(DeviceBasicInfo device)
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

        public AuthModeMap GetBioAuthModeWithDeviceId(int deviceId, int authMode)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@DeviceId", SqlDbType.Int) { Value = deviceId },
                new SqlParameter("@AuthMode", SqlDbType.Int) { Value = authMode },
            };
            return _repository.ToResultList<AuthModeMap>("SelectBioAuthModeByDeviceId", parameters).Data.FirstOrDefault();
        }

        public DateTime GetLastConnectedTime(uint deviceId)
        {
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@deviceId", SqlDbType.Int) { Value = deviceId }
                };

                return _repository.ToResultList<DateTime>("SelectLastConnectionTime", parameters).Data.FirstOrDefault();
                //var result = _repository.ToResultList<string>("SelectLastConnectionTime", parameters).Data.FirstOrDefault();
                //return DateTime.Parse(result);
            }
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

        public Task<List<User>> GetAuthorizedUsersOfDevice(int deviceId)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@DeviceId", deviceId)
                };

                return _repository.ToResultList<User>("SelectAuthorizedUsersOfDevice", parameters, fetchCompositions: true).Data;
            });
        }
    }
}
