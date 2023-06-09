﻿using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System;
using System.Collections.Generic;
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
