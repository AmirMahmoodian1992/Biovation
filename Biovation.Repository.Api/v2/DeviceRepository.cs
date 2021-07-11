using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Biovation.Repository.Api.v2
{
    public class DeviceRepository
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
        public async Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> GetDevices(int groupId = 0, uint code = 0,
            string brandId = default, string name = null, int modelId = 0, int deviceIoTypeId = 0, int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Device", Method.GET);
            restRequest.AddQueryParameter("groupId", groupId.ToString());
            restRequest.AddQueryParameter("code", code.ToString());
            if (brandId != null)
                restRequest.AddQueryParameter("brandId", brandId);
            if (name != null)
                restRequest.AddQueryParameter("name", name);
            restRequest.AddQueryParameter("modelId", modelId.ToString());
            restRequest.AddQueryParameter("deviceIoTypeId", deviceIoTypeId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceBasicInfo>>>(restRequest);

            return requestResult.Data;
        }

        public async Task<ResultViewModel<DeviceBasicInfo>> GetDevice(long id = 0, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Device/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<DeviceBasicInfo>>(restRequest);
            return requestResult.Data;
        }
        public async Task<ResultViewModel<PagingResult<DeviceModel>>> GetDeviceModels(long id = 0, string brandId = default,
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
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceModel>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<AuthModeMap>> GetBioAuthModeWithDeviceId(int id, int authMode, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Device/GetBioAuthModeWithDeviceId", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("authMode", authMode.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<AuthModeMap>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<DateTime>> GetLastConnectedTime(uint id, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Device/LastConnectedTime", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<DateTime>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<Lookup>>> GetDeviceBrands(int code = default, string name = default,
            int pageNumber = default, int pageSize = default, string token = default)

        {
            var restRequest = new RestRequest("Queries/v2/Device/DeviceBrands", Method.GET);
            restRequest.AddQueryParameter("code", code.ToString());
            restRequest.AddQueryParameter("name", name ?? string.Empty);
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<Lookup>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> AddDevice(DeviceBasicInfo device, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Device", Method.POST);
            restRequest.AddJsonBody(device);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> AddDeviceModel(DeviceModel deviceModel, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Device/DeviceModel", Method.POST);
            restRequest.AddJsonBody(deviceModel);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> DeleteDevice(uint id, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Device/{id}", Method.DELETE);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddUrlSegment("id", id.ToString());
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> DeleteDevices(List<uint> ids, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Device/DeleteDevices", Method.POST);
            restRequest.AddJsonBody(ids);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> ModifyDevice(DeviceBasicInfo device, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Device", Method.PUT);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddJsonBody(device);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        // TODO - Verify method.
        public async void ModifyDeviceInfo(DeviceBasicInfo device, string token = default)
        {
            var restRequest = new RestRequest($"{device.ServiceInstance.Id}/Device/ModifyDevice",
                Method.POST);
            restRequest.AddJsonBody(device);
            restRequest.AddHeader("Authorization", token!);
            await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
        }

        public async Task<ResultViewModel> AddNetworkConnectionLog(DeviceBasicInfo device, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Device/NetworkConnectionLog", Method.POST);
            restRequest.AddJsonBody(device);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<User>>> GetAuthorizedUsersOfDevice(int id, string token = default)
        {
            var restRequest = new RestRequest($"Queries/v2/Device/AuthorizedUsersOfDevice/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<User>>>(restRequest);
            return requestResult.Data;
        }

        // TODO - Ask if the foreach is correct.
        public async Task<List<DeviceBasicInfo>> GetOnlineDevices(string token)
        {
            var resultList = new List<DeviceBasicInfo>();

            var deviceBrands = (await GetDeviceBrands())?.Data.Data;

            var serviceInstances = _systemInfo.Services;

            if (deviceBrands == null) return resultList;

            Parallel.ForEach(serviceInstances, serviceInstance =>
            {
                var restRequest = new RestRequest($"{serviceInstance.Id}/Device/GetOnlineDevices");
                token ??= _biovationConfigurationManager.DefaultToken;
                restRequest.AddHeader("Authorization", token);
                var result = _restClient.Execute<List<DeviceBasicInfo>>(restRequest);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    resultList.AddRange(result.Data);
                }

            });

            return resultList;
        }

        public IRestResponse<ResultViewModel> ClearLogsOfDevice(DeviceBasicInfo device, DateTime? fromDate, DateTime? toDate, string token)
        {
            var restRequest = new RestRequest($"{device.ServiceInstance.Id}/Log/ClearLog", Method.POST);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            if (fromDate.HasValue) restRequest.AddQueryParameter("fromDate", fromDate.Value.ToString(CultureInfo.InvariantCulture));
            if (toDate.HasValue) restRequest.AddQueryParameter("toDate", toDate.Value.ToString(CultureInfo.InvariantCulture));
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);

            var restAwaiter = _restClient.ExecuteAsync<ResultViewModel>(restRequest).GetAwaiter();

            return restAwaiter.GetResult();
        }

        public IRestResponse<List<ResultViewModel>> RetrieveUsers(DeviceBasicInfo device, List<uint> userId = default, string token = default)
        {
            var restRequest = new RestRequest($"{device.ServiceInstance.Id}/Device/RetrieveUserFromDevice", Method.POST);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            if (userId != null)
            {
                restRequest.AddJsonBody(userId);
            }
            var restResult = _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest).GetAwaiter();
            return restResult.GetResult();
        }

        public ResultViewModel<List<User>> RetrieveUsersOfDevice(DeviceBasicInfo device, string token = default)
        {
            var restRequest =
                new RestRequest(
                    $"{device.ServiceInstance.Id}/Device/RetrieveUsersListFromDevice");
            restRequest.AddQueryParameter("code", device.Code.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var restAwaiter = _restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest).GetAwaiter();
            return restAwaiter.GetResult().Data;
        }

        public IRestResponse<ResultViewModel> ReadOfflineOfDevice(DeviceBasicInfo device, DateTime? fromDate, DateTime? toDate, string token = default)
        {
            var restRequest =
                new RestRequest(
                    $"{device.ServiceInstance.Id}/Device/ReadOfflineOfDevice");
            restRequest.AddQueryParameter("code", device.Code.ToString());
            if (fromDate.HasValue) restRequest.AddQueryParameter("fromDate", fromDate.Value.ToString(CultureInfo.InvariantCulture));
            if (toDate.HasValue) restRequest.AddQueryParameter("toDate", toDate.Value.ToString(CultureInfo.InvariantCulture));
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var restAwaiter = _restClient.ExecuteAsync<ResultViewModel>(restRequest).GetAwaiter();
            return restAwaiter.GetResult();
        }

        public ResultViewModel RemoveUserFromDeviceById(DeviceBasicInfo device, long userId, string token = default)
        {
            var restRequest =
                new RestRequest(
                    $"{device.ServiceInstance.Id}/Device/DeleteUserFromDevice",
                    Method.POST);

            restRequest.AddJsonBody(userId);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
        }

        public ResultViewModel RemoveUserFromDevice(DeviceBasicInfo device, string token = default)
        {
            var restRequest =
                new RestRequest(
                    $"{device.ServiceInstance.Id}/Device/DeleteUserFromDevice", Method.POST);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var restAwaiter = _restClient.ExecuteAsync<ResultViewModel>(restRequest).GetAwaiter();
            return restAwaiter.GetResult().Data;
        }

        // TODO - Verify the method.
        public ResultViewModel DeleteUserFromDevice(DeviceBasicInfo device, List<long> userIds, string token = default)
        {
            var restRequest = new RestRequest($"{device.ServiceInstance.Id}/Device/DeleteUserFromDevice", Method.POST);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            restRequest.AddJsonBody(userIds);
            restRequest.AddHeader("Authorization", token!);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).GetAwaiter().GetResult()?.Data;
        }

        // TODO - Verify the method.
        public Task<IRestResponse<List<ResultViewModel>>> SendUserToDevice(DeviceBasicInfo device, List<long> userIds, string token = default)
        {
            var restRequest =
                new RestRequest(
                    $"/{device.ServiceInstance.Id}/User/SendUserToDevice",
                    Method.GET);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            restRequest.AddQueryParameter("userId", JsonSerializer.Serialize(userIds));
            restRequest.AddHeader("Authorization", token!);
            return _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
        }

        // TODO - Verify the method.
        public Task<IRestResponse<ResultViewModel>> SendUsersOfDevice(DeviceBasicInfo device, string token = default)
        {
            var restRequest = new RestRequest($"{device.ServiceInstance.Id}/Device/SendUsersOfDevice", Method.POST);
            restRequest.AddJsonBody(device);
            restRequest.AddHeader("Authorization", token!);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest);
        }

        // TODO - Verify the method.
        public Task<IRestResponse<Dictionary<string, string>>> GetAdditionalData(DeviceBasicInfo device, string token = default)
        {
            var restRequest = new RestRequest($"{device.ServiceInstance.Id}/Device/GetAdditionalData");
            restRequest.AddQueryParameter("code", device.Code.ToString());
            restRequest.AddHeader("Authorization", token!);
            return _restClient.ExecuteAsync<Dictionary<string, string>>(restRequest);
        }
    }
}