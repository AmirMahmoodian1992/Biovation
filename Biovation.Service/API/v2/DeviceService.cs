using System;
using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.API.v2
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
            int deviceGroupId = default, uint code = default, int brandId = default, string deviceName = null,
            int deviceModelId = default, int typeId = default, int pageNumber = default, int pageSize = default)
        {
            return _deviceRepository.GetDevices(adminUserId, deviceGroupId, code, brandId, deviceName, deviceModelId,
                typeId, pageNumber, pageSize);
        }

        public ResultViewModel<DeviceBasicInfo> GetDevice(long id = default, long adminUserId = default)
        {
            return _deviceRepository.GetDevice(id, (int) adminUserId);
        }

        public PagingResult<DeviceModel> GetDeviceModels(long id = default, int brandId = default,
            string deviceName = null, int pageNumber = default, int pageSize = default)
        {
            return _deviceRepository.GetDeviceModels(id, brandId.ToString(), deviceName, pageNumber, pageSize);
        }

        public ResultViewModel<AuthModeMap> GetBioAuthModeWithDeviceId(int id, int authMode)
        {
            return _deviceRepository.GetBioAuthModeWithDeviceId(id, authMode);
        }

        public ResultViewModel<DateTime> GetLastConnectedTime(int id)
        {
            return _deviceRepository.GetLastConnectedTime((uint) id);
        }

        public PagingResult<Lookup> GetDeviceBrands(int code = default, string name = default,
            int pageNumber = default, int pageSize = default)

        {
            return _deviceRepository.GetDeviceBrands(code, name, pageNumber, pageSize);
        }

        public ResultViewModel AddDevice(DeviceBasicInfo device = default)
        {
            return _deviceRepository.AddDevice(device);
        }

        public ResultViewModel AddDeviceModel(DeviceModel deviceModel = default)
        {
            return _deviceRepository.AddDeviceModel(deviceModel);
        }

        public ResultViewModel DeleteDevice(uint id)
        {
            return _deviceRepository.DeleteDevice(id);
        }

        public ResultViewModel DeleteDevices(List<uint> ids)
        {
            return _deviceRepository.DeleteDevices(ids);
        }

        public ResultViewModel ModifyDevice(DeviceBasicInfo device)
        {
            return _deviceRepository.ModifyDevice(device);
        }

        public ResultViewModel AddNetworkConnectionLog(DeviceBasicInfo device)
        {
            return _deviceRepository.AddNetworkConnectionLog(device);
        }
    }
}
