using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Threading.Tasks;

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
        public async Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> GetDevices(int deviceGroupId = default, uint code = default, string brandId = default, string deviceName = null,
            int deviceModelId = default, int deviceIoTypeId = default, int pageNumber = default, int pageSize = default, string token = default)
        {
            return await _deviceRepository.GetDevices(deviceGroupId, code, brandId, deviceName, deviceModelId,
                deviceIoTypeId, pageNumber, pageSize, token);
        }

        public async Task<ResultViewModel<DeviceBasicInfo>> GetDevice(long id = default, string token = default)
        {
            return await _deviceRepository.GetDevice(id, token);
        }

        public async Task<ResultViewModel<PagingResult<DeviceModel>>> GetDeviceModels(long id = default, int brandId = default,
            string name = null, int pageNumber = default, int pageSize = default, string token = default)
        {
            return await _deviceRepository.GetDeviceModels(id, brandId.ToString(), name, pageNumber, pageSize, token);
        }

        public async Task<ResultViewModel<AuthModeMap>> GetBioAuthModeWithDeviceId(int id, int authMode, string token)
        {
            return await _deviceRepository.GetBioAuthModeWithDeviceId(id, authMode, token);
        }

        public async Task<ResultViewModel<DateTime>> GetLastConnectedTime(int id, string token)
        {
            return await _deviceRepository.GetLastConnectedTime((uint)id, token);
        }

        public async Task<ResultViewModel<PagingResult<Lookup>>> GetDeviceBrands(int code = default, string name = default,
            int pageNumber = default, int pageSize = default, string token = default)

        {
            return await _deviceRepository.GetDeviceBrands(code, name, pageNumber, pageSize, token);
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
        public IRestResponse<ResultViewModel> ReadOfflineOfDevice(DeviceBasicInfo device, DateTime? fromDate, DateTime? toDate, string token = default)
        {
            return _deviceRepository.ReadOfflineOfDevice(device, fromDate, toDate, token);
        }

        public ResultViewModel RemoveUserFromDevice(DeviceBasicInfo device, string token = default)
        {
            return _deviceRepository.RemoveUserFromDevice(device, token);
        }


        // TODO - Verify method.
        public ResultViewModel RemoveUserFromDeviceById(DeviceBasicInfo device, long userId, string token = default)
        {
            return _deviceRepository.RemoveUserFromDeviceById(device, userId, token);
        }

        // TODO - Verify the method.
        public async Task<List<DeviceBasicInfo>> GetOnlineDevices(string token = default)
        {
            return await _deviceRepository.GetOnlineDevices(token);
        }

        // TODO - Verify the method.
        public IRestResponse<ResultViewModel> ClearLogOfDevice(DeviceBasicInfo device, DateTime? fromDate, DateTime? toDate, string token)
        {
            return _deviceRepository.ClearLogsOfDevice(device, fromDate, toDate, token);
        }

        // TODO - Verify the method.
        public IRestResponse<List<ResultViewModel>> RetrieveUsers(DeviceBasicInfo device, List<uint> userId = default, string token = default)
        {
            return _deviceRepository.RetrieveUsers(device, userId, token);
        }

        public async Task<ResultViewModel> AddDevice(DeviceBasicInfo device = default, string token = default)
        {
            return await _deviceRepository.AddDevice(device, token);
        }

        public async Task<ResultViewModel> AddDeviceModel(DeviceModel deviceModel = default, string token = default)
        {
            return await _deviceRepository.AddDeviceModel(deviceModel, token);
        }

        public async Task<ResultViewModel> DeleteDevice(uint id, string token = default)
        {
            return await _deviceRepository.DeleteDevice(id, token);
        }

        public async Task<ResultViewModel> DeleteDevices(List<uint> ids, string token = default)
        {
            return await _deviceRepository.DeleteDevices(ids, token);
        }

        public async Task<ResultViewModel> ModifyDevice(DeviceBasicInfo device, string token = default)
        {
            return await _deviceRepository.ModifyDevice(device, token);
        }

        public async Task<ResultViewModel> AddNetworkConnectionLog(DeviceBasicInfo device, string token = default)
        {
            return await _deviceRepository.AddNetworkConnectionLog(device, token);
        }

        public async Task<ResultViewModel<PagingResult<User>>> GetAuthorizedUsersOfDevice(int id, string token = default)
        {
            return await _deviceRepository.GetAuthorizedUsersOfDevice(id, token);
        }
    }
}
