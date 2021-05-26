using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ResultViewModel<List<User>> RetrieveUsersOfDevice(DeviceBasicInfo device, List<User> users, string token = default)
        {
            var usersResult =  _deviceRepository.RetrieveUsersOfDevice(device, token);

            var joinResult = (from r in usersResult?.Data
                join u in users on r.Code equals u.Code
                    into ps
                from u in ps.DefaultIfEmpty()
                select new User
                {
                    Type = u == null ? 0 : 1,
                    IsActive = r.IsActive,
                    Id = r.Id,
                    Code = r.Code,
                    FullName = u != null ? u.FirstName + " " + u.SurName : r.UserName,
                    StartDate = u?.StartDate ?? new DateTime(1990, 1, 1),
                    EndDate = u?.EndDate ?? new DateTime(2050, 1, 1)
                }).ToList();

            var lastResult = new ResultViewModel<List<User>>
            {
                Data = joinResult
            };
            return lastResult;
        }

        // TODO - Verify method.
        public IRestResponse<ResultViewModel> ReadOfflineOfDevice(DeviceBasicInfo device, string fromDate, string toDate, string token = default)
        {
            return _deviceRepository.ReadOfflineOfDevice(device, fromDate, toDate, token);
        }

        public ResultViewModel RemoveUserFromDevice(DeviceBasicInfo device, string token = default)
        {
            return _deviceRepository.RemoveUserFromDevice(device, token);
        }


        // TODO - Verify method.
        public ResultViewModel RemoveUserFromDeviceById(DeviceBasicInfo device, int userId, string token = default)
        {
            return _deviceRepository.RemoveUserFromDeviceById(device, userId, token);
        }

        // TODO - Verify the method.
        public List<DeviceBasicInfo> GetOnlineDevices(string token = default)
        {
            return _deviceRepository.GetOnlineDevices(token);
        }

        // TODO - Verify the method.
        public IRestResponse<ResultViewModel> ClearLogOfDevice(DeviceBasicInfo device, string fromDate, string toDate, string token)
        {
            return _deviceRepository.ClearLogsOfDevice(device, fromDate, toDate, token);
        }

        // TODO - Verify the method.
        public IRestResponse<List<ResultViewModel>> RetrieveUsers(DeviceBasicInfo device, JArray userId = default, string token = default)
        {
            return _deviceRepository.RetrieveUsers(device, userId, token);
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
