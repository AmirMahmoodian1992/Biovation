using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System;
using System.Collections.Generic;

namespace Biovation.Service.Api.v1
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
        public List<DeviceBasicInfo> GetDevices(long adminUserId = default,
            int deviceGroupId = default, uint code = default, string brandId = default, string deviceName = null,
            int deviceModelId = default, int typeId = default, int pageNumber = default, int pageSize = default, string token = default)
        {
            return _deviceRepository.GetDevices(adminUserId, deviceGroupId, code, brandId, deviceName, deviceModelId,
                typeId, pageNumber, pageSize,token)?.Data?.Data ?? new List<DeviceBasicInfo>();
        }

        public DeviceBasicInfo GetDevice(long id = default, long adminUserId = default, string token = default)
        {
            return _deviceRepository.GetDevice(id, token).Result?.Data ?? new DeviceBasicInfo();
        }

        public List<DeviceModel> GetDeviceModels(long id = default, string brandId = default,
            string deviceName = null, int pageNumber = default, int pageSize = default, string token = default)
        {
            return _deviceRepository.GetDeviceModels(id, brandId, deviceName, pageNumber, pageSize)?.Data?.Data ?? new List<DeviceModel>();
        }

        public ResultViewModel<AuthModeMap> GetBioAuthModeWithDeviceId(int id, int authMode, string token = default)
        {
            return _deviceRepository.GetBioAuthModeWithDeviceId(id, authMode);
        }

        public ResultViewModel<DateTime> GetLastConnectedTime(int id, string token = default)
        {
            return _deviceRepository.GetLastConnectedTime((uint)id);
        }

        public List<Lookup> GetDeviceBrands(int code = default, string name = default,
            int pageNumber = default, int pageSize = default, string token = default)

        {
            return _deviceRepository.GetDeviceBrands(code, name, pageNumber, pageSize, token)?.Data?.Data ?? new List<Lookup>();
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

        public List<User> GetAuthorizedUsersOfDevice(int id, string token = default)
        {
            return _deviceRepository.GetAuthorizedUsersOfDevice(id, token)?.Data?.Data ?? new List<User>();
        }
    }
}
