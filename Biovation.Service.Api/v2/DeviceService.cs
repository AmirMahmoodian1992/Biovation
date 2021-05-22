using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Biovation.Service.Api.v2
{
    public class DeviceService
    {
        private readonly DeviceRepository _deviceRepository;

        public DeviceService(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        /// <summary>
        /// <En>Call a repository method to get all devices from database.</En>
        /// <Fa>با صدا کردن یک تابع در ریپوزیتوری اطلاعات تمامی دستگاه ها را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        //public List<DeviceBasicInfo> GetDevice(long adminUserId = default)
        //{
        //    return _deviceRepository.GetDevice(adminUserId);
        //}
        public ResultViewModel<PagingResult<DeviceBasicInfo>> GetDevices(long adminUserId = default,
            int deviceGroupId = default, uint code = default, string brandId = default, string deviceName = null,
            int deviceModelId = default, int deviceIoTypeId = default, int pageNumber = default, int pageSize = default, string token = default)
        {
            return _deviceRepository.GetDevices(adminUserId, deviceGroupId, code, brandId, deviceName, deviceModelId,
                deviceIoTypeId, pageNumber, pageSize);
        }

        public ResultViewModel<DeviceBasicInfo> GetDevice(long id = default, long adminUserId = default, string token = default)
        {
            return _deviceRepository.GetDevice(id, (int)adminUserId, token);
        }

        public ResultViewModel<PagingResult<DeviceModel>> GetDeviceModels(long id = default, int brandId = default,
            string name = null, int pageNumber = default, int pageSize = default, string token = default)
        {
            return _deviceRepository.GetDeviceModels(id, brandId.ToString(), name, pageNumber, pageSize, token);
        }

        public ResultViewModel<AuthModeMap> GetBioAuthModeWithDeviceId(int id, int authMode, string token)
        {
            return _deviceRepository.GetBioAuthModeWithDeviceId(id, authMode, token);
        }

        public ResultViewModel<DateTime> GetLastConnectedTime(int id, string token)
        {
            return _deviceRepository.GetLastConnectedTime((uint)id, token);
        }

        public ResultViewModel<PagingResult<Lookup>> GetDeviceBrands(int code = default, string name = default,
            int pageNumber = default, int pageSize = default, string token = default)

        {
            return _deviceRepository.GetDeviceBrands(code, name, pageNumber, pageSize, token);
        }

        // TODO - Verify the method
        public ResultViewModel<List<User>> RetrieveUsersOfDevice(DeviceBasicInfo device)
        {
            return _deviceRepository.RetrieveUsersOfDevice(device);
        }

        // TODO - Verify method.
        public IRestResponse<ResultViewModel> ReadOfflineOfDevice(DeviceBasicInfo device, string fromDate, string toDate)
        {
            return _deviceRepository.ReadOfflineOfDevice(device, fromDate, toDate);
        }

        public ResultViewModel RemoveUserFromDevice(DeviceBasicInfo device)
        {
            return _deviceRepository.RemoveUserFromDevice(device);
        }


        // TODO - Verify method.
        public ResultViewModel RemoveUserFromDeviceById(DeviceBasicInfo device, int userId)
        {
            return _deviceRepository.RemoveUserFromDeviceById(device, userId);
        }

        // TODO - Verify the method.
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            return _deviceRepository.GetOnlineDevices();
        }

        // TODO - Verify the method.
        public IRestResponse<ResultViewModel> ClearLogOfDevice(DeviceBasicInfo device, string fromDate, string toDate)
        {
            return _deviceRepository.ClearLogsOfDevice(device, fromDate, toDate);
        }

        // TODO - Verify the method.
        public IRestResponse<List<ResultViewModel>> RetrieveUsers(DeviceBasicInfo device, JArray userId = default)
        {
            return _deviceRepository.RetrieveUsers(device, userId);
        }

        public ResultViewModel AddDevice(DeviceBasicInfo device = default, string token = default)
        {
            return _deviceRepository.AddDevice(device, token);
        }

        public ResultViewModel AddDeviceModel(DeviceModel deviceModel = default, string token = default)
        {
            return _deviceRepository.AddDeviceModel(deviceModel, token);
        }

        public ResultViewModel DeleteDevice(uint id, string token = default)
        {
            return _deviceRepository.DeleteDevice(id, token);
        }

        public ResultViewModel DeleteDevices(List<uint> ids, string token = default)
        {
            return _deviceRepository.DeleteDevices(ids, token);
        }

        public ResultViewModel ModifyDevice(DeviceBasicInfo device, string token = default)
        {
            return _deviceRepository.ModifyDevice(device, token);
        }

        public ResultViewModel AddNetworkConnectionLog(DeviceBasicInfo device, string token = default)
        {
            return _deviceRepository.AddNetworkConnectionLog(device, token);
        }

        public ResultViewModel<PagingResult<User>> GetAuthorizedUsersOfDevice(int id, string token = default)
        {
            return _deviceRepository.GetAuthorizedUsersOfDevice(id, token);
        }
    }
}
