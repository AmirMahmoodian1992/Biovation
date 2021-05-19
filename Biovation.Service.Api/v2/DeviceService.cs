﻿using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Service.Api.v2
{
    public class DeviceService : ControllerBase
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
        public ResultViewModel<List<User>> RetrieveUsersOfDevice(RestClient restClient, DeviceBasicInfo device)
        {
            var restRequest =
                new RestRequest(
                    $"{device.Brand.Name}/{device.ServiceInstance.Id}/{device.Brand.Name}Device/RetrieveUsersListFromDevice");
            restRequest.AddQueryParameter("code", device.Code.ToString());
            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
            {
                restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
            }

            var restAwaiter = restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest).GetAwaiter();
            return restAwaiter.GetResult().Data;
        }

        // TODO - Verify method.
        public IRestResponse<ResultViewModel> ReadOfflineLog(RestClient restClient, DeviceBasicInfo device, string fromDate, string toDate)
        {
            var restRequest =
                new RestRequest(
                    $"{device.Brand.Name}/{device.ServiceInstance.Id}/{device.Brand.Name}Device/ReadOfflineOfDevice");
            restRequest.AddQueryParameter("code", device.Code.ToString());
            restRequest.AddQueryParameter("fromDate", fromDate);
            restRequest.AddQueryParameter("toDate", toDate);
            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
            {
                restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
            }

            var restAwaiter = restClient.ExecuteAsync<ResultViewModel>(restRequest).GetAwaiter();
            return restAwaiter.GetResult();
        }

        // TODO - Verify the method.
        public ResultViewModel<List<DeviceBasicInfo>> GetOnlineDevices(RestClient restClient, Lookup deviceBrand, ServiceInstance serviceInstance)
        {
            var restRequest =
                new RestRequest($"{deviceBrand.Name}/{serviceInstance.Id}/{deviceBrand.Name}Device/GetOnlineDevices");
            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
            {
                restRequest.AddHeader("Authorization",
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault());
            }

            var restAwaiter = restClient.ExecuteAsync<ResultViewModel<List<DeviceBasicInfo>>>(restRequest).GetAwaiter();
            return restAwaiter.GetResult().Data;
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
