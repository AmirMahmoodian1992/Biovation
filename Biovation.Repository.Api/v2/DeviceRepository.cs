using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System;
using System.Collections.Generic;

namespace Biovation.Repository.Api.v2
{
    public class DeviceRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public DeviceRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
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
            if (id != 0) restRequest.AddUrlSegment("id", id.ToString());
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
            if (id != 0) restRequest.AddUrlSegment("id", id.ToString());
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
    }
}