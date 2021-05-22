using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Biovation.Repository.Api.v2
{
    public class DeviceRepository : ControllerBase
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        private readonly SystemInfo _systemInfo;
        public DeviceRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager, SystemInfo systemInfo)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
            _systemInfo = systemInfo;
        }

        /// <summary>
        /// <En>Get all devices from database.</En>
        /// <Fa>اطلاعات تمامی دستگاه ها را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        /// 

        //public ResultViewModel<DeviceBasicInfo> GetDevice(long id, int adminUserId = 0)
        //{

        //}
        public ResultViewModel<PagingResult<DeviceBasicInfo>> GetDevices(long adminUserId = 0, int groupId = 0, uint code = 0,
            string brandId = default, string name = null, int modelId = 0, int deviceIoTypeId = 0, int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Device", Method.GET);
            //restRequest.AddQueryParameter("adminUserId", adminUserId.ToString());
            restRequest.AddQueryParameter("groupId", groupId.ToString());
            restRequest.AddQueryParameter("code", code.ToString());
            restRequest.AddQueryParameter("brandId", brandId);
            restRequest.AddQueryParameter("name", name ?? string.Empty);
            restRequest.AddQueryParameter("modelId", modelId.ToString());
            restRequest.AddQueryParameter("deviceIoTypeId", deviceIoTypeId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token); 
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceBasicInfo>>>(restRequest);

            return requestResult.Result.Data;
        }

        public ResultViewModel<DeviceBasicInfo> GetDevice(long id = 0, int adminUserId = 0, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Device/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            //restRequest.AddQueryParameter("adminUserId", adminUserId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<DeviceBasicInfo>>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel<PagingResult<DeviceModel>> GetDeviceModels(long id = 0, string brandId = default,
            string name = default, int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Device/DeviceModels/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            if (brandId != null) restRequest.AddQueryParameter("brandId", brandId);
            restRequest.AddQueryParameter("name", name ?? string.Empty);
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceModel>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<AuthModeMap> GetBioAuthModeWithDeviceId(int id, int authMode,string token =default)
        {
            var restRequest = new RestRequest("Queries/v2/Device/GetBioAuthModeWithDeviceId", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("authMode", authMode.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<AuthModeMap>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<DateTime> GetLastConnectedTime(uint id, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Device/LastConnectedTime", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<DateTime>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<Lookup>> GetDeviceBrands(int code = default, string name = default,
            int pageNumber = default, int pageSize = default, string token = default)

        {
            var restRequest = new RestRequest("Queries/v2/Device/DeviceBrands", Method.GET);
            restRequest.AddQueryParameter("code", code.ToString());
            restRequest.AddQueryParameter("name", name ?? string.Empty);
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token); 
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<Lookup>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel AddDevice(DeviceBasicInfo device, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Device", Method.POST);
            restRequest.AddJsonBody(device);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel AddDeviceModel(DeviceModel deviceModel, string token=default)
        {
            var restRequest = new RestRequest("Commands/v2/Device/DeviceModel", Method.POST);
            restRequest.AddJsonBody(deviceModel);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel DeleteDevice(uint id, string token=default)
        {
            var restRequest = new RestRequest("Commands/v2/Device/{id}", Method.DELETE);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddUrlSegment("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel DeleteDevices(List<uint> ids, string token=default)
        {
            var restRequest = new RestRequest("Commands/v2/Device/DeleteDevices", Method.POST);
            restRequest.AddJsonBody(ids);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyDevice(DeviceBasicInfo device,string token =default)
        {
            var restRequest = new RestRequest("Commands/v2/Device", Method.PUT);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddJsonBody(device);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel AddNetworkConnectionLog(DeviceBasicInfo device,string token =default)
        {
            var restRequest = new RestRequest("Commands/v2/Device/NetworkConnectionLog", Method.POST);
            restRequest.AddJsonBody(device);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<User>> GetAuthorizedUsersOfDevice(int id, string token =default)
        {
            var restRequest = new RestRequest($"Queries/v2/Device/AuthorizedUsersOfDevice/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<User>>>(restRequest);
            return requestResult.Result.Data;
        }

        // TODO - Ask if the foreach is correct.
        public List<DeviceBasicInfo> GetOnlineDevices()
        {
            var resultList = new List<DeviceBasicInfo>();

            var deviceBrands = GetDeviceBrands()?.Data.Data;

            var serviceInstances = _systemInfo.Services;

            if (deviceBrands == null) return resultList;

            Parallel.ForEach(deviceBrands, deviceBrand =>
            {
                foreach (var restRequest in serviceInstances.Select(serviceInstance => 
                    new RestRequest($"{deviceBrand.Name}/{serviceInstance.Id}/{deviceBrand.Name}Device/GetOnlineDevices")))
                {
                    if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                    {
                        restRequest.AddHeader("Authorization",
                            HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                    }

                    var result = _restClient.Execute<List<DeviceBasicInfo>>(restRequest);

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        resultList.AddRange(result.Data);
                    }
                }
            });

            return resultList;
        }

        public IRestResponse<ResultViewModel> ClearLogsOfDevice(DeviceBasicInfo device, string fromDate, string toDate)
        {
            var restRequest = new RestRequest($"{device.Brand.Name}/{device.ServiceInstance.Id}/{device.Brand.Name}Log/ClearLog", Method.POST);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            restRequest.AddQueryParameter("fromDate", fromDate);
            restRequest.AddQueryParameter("toDate", toDate);
            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
            {
                restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
            }

            var restAwaiter = _restClient.ExecuteAsync<ResultViewModel>(restRequest).GetAwaiter();

            return restAwaiter.GetResult();
        }

        public IRestResponse<List<ResultViewModel>> RetrieveUsers(DeviceBasicInfo device, JArray userId = default)
        {
            var restRequest = new RestRequest($"{device.Brand.Name}/{device.ServiceInstance.Id}/{device.Brand.Name}Device/RetrieveUserFromDevice", Method.POST);
            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
            {
                restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
            }
            restRequest.AddQueryParameter("code", device.Code.ToString());

            restRequest.AddJsonBody(userId);
            var restResult = _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest).GetAwaiter();
            return restResult.GetResult();
        }

        public ResultViewModel<List<User>> RetrieveUsersOfDevice(DeviceBasicInfo device)
        {
            var restRequest =
                new RestRequest(
                    $"{device.Brand.Name}/{device.ServiceInstance.Id}/{device.Brand.Name}Device/RetrieveUsersListFromDevice");
            restRequest.AddQueryParameter("code", device.Code.ToString());
            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
            {
                restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
            }

            var restAwaiter = _restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest).GetAwaiter();
            return restAwaiter.GetResult().Data;
        }

        public IRestResponse<ResultViewModel> ReadOfflineOfDevice(DeviceBasicInfo device, string fromDate, string toDate)
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

            var restAwaiter = _restClient.ExecuteAsync<ResultViewModel>(restRequest).GetAwaiter();
            return restAwaiter.GetResult();
        }

        public ResultViewModel RemoveUserFromDeviceById(DeviceBasicInfo device, int userId)
        {
            var restRequest =
                new RestRequest(
                    $"{device.Brand?.Name}/{device.ServiceInstance.Id}/{device.Brand?.Name}Device/DeleteUserFromDevice",
                    Method.POST);

            restRequest.AddQueryParameter("code", device.Code.ToString());

            restRequest.AddJsonBody(userId);

            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
            {
                restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
            }

            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
        }

        public ResultViewModel RemoveUserFromDevice(DeviceBasicInfo device)
        {
            var restRequest =
                new RestRequest(
                    $"{device.Brand.Name}/{device.ServiceInstance.Id}/{device.Brand.Name}Device/DeleteUserFromDevice", Method.POST);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
            {
                restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
            }

            var restAwaiter = _restClient.ExecuteAsync<ResultViewModel>(restRequest).GetAwaiter();
            return restAwaiter.GetResult().Data;
        }
    }
}