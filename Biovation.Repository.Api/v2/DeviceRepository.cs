using Biovation.Domain;
using RestSharp;
using System;
using System.Collections.Generic;

namespace Biovation.Repository.Api.v2
{
    public class DeviceRepository
    {
        private readonly RestClient _restClient;
        public DeviceRepository(RestClient restClient)
        {
            _restClient = restClient;
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
            int brandId = 0, string name = null, int modelId = 0, int deviceIoTypeId = 0, int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest("Queries/v2/Device", Method.GET);
            restRequest.AddQueryParameter("adminUserId", adminUserId.ToString());
            restRequest.AddQueryParameter("groupId", groupId.ToString());
            restRequest.AddQueryParameter("code", code.ToString());
            restRequest.AddQueryParameter("brandId", brandId.ToString());
            restRequest.AddQueryParameter("name", name ?? string.Empty);
            restRequest.AddQueryParameter("modelId", modelId.ToString());
            restRequest.AddQueryParameter("deviceIoTypeId", deviceIoTypeId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceBasicInfo>>>(restRequest);

            return requestResult.Result.Data;
        }

        public ResultViewModel<DeviceBasicInfo> GetDevice(long id = 0, int adminUserId = 0)
        {
            var restRequest = new RestRequest("Queries/v2/Device/{id}", Method.GET);
            if (id != 0) restRequest.AddUrlSegment("id", id.ToString());
            restRequest.AddQueryParameter("adminUserId", adminUserId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<DeviceBasicInfo>>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel<PagingResult<DeviceModel>> GetDeviceModels(long id = 0, string brandId = default,
            string name = default, int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest("Queries/v2/Device/DeviceModels/{id}", Method.GET);
            if (id != 0) restRequest.AddUrlSegment("id", id.ToString());
            if (brandId != null) restRequest.AddQueryParameter("brandId", brandId);
            restRequest.AddQueryParameter("name", name ?? string.Empty);
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceModel>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<AuthModeMap> GetBioAuthModeWithDeviceId(int id, int authMode)
        {
            var restRequest = new RestRequest("Queries/v2/Device/GetBioAuthModeWithDeviceId", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("authMode", authMode.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<AuthModeMap>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<DateTime> GetLastConnectedTime(uint id)
        {
            var restRequest = new RestRequest("Queries/v2/Device/LastConnectedTime", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<DateTime>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<Lookup>> GetDeviceBrands(int code = default, string name = default,
            int pageNumber = default, int pageSize = default)

        {
            var restRequest = new RestRequest("Queries/v2/Device/DeviceBrands", Method.GET);
            restRequest.AddQueryParameter("code", code.ToString());
            restRequest.AddQueryParameter("name", name ?? string.Empty);
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<Lookup>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel AddDevice(DeviceBasicInfo device)
        {
            var restRequest = new RestRequest("Commands/v2/Device", Method.POST);
            restRequest.AddJsonBody(device);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel AddDeviceModel(DeviceModel deviceModel)
        {
            var restRequest = new RestRequest("Commands/v2/Device/DeviceModel", Method.POST);
            restRequest.AddJsonBody(deviceModel);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel DeleteDevice(uint id)
        {
            var restRequest = new RestRequest("Commands/v2/Device/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel DeleteDevices(List<uint> ids)
        {
            var restRequest = new RestRequest("Commands/v2/Device/DeleteDevices", Method.POST);
            restRequest.AddJsonBody(ids);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyDevice(DeviceBasicInfo device)
        {
            var restRequest = new RestRequest("Commands/v2/Device", Method.PUT);
            restRequest.AddJsonBody(device);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel AddNetworkConnectionLog(DeviceBasicInfo device)
        {
            var restRequest = new RestRequest("Commands/v2/Device/NetworkConnectionLog", Method.POST);
            restRequest.AddJsonBody(device);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<User>> GetAuthorizedUsersOfDevice(int id)
        {
            var restRequest = new RestRequest($"Queries/v2/Device/AuthorizedUsersOfDevice/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<User>>>(restRequest);
            return requestResult.Result.Data;
        }
    }
}