using System;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class DeviceRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public DeviceRepository(GenericRepository repository, RestClient restClient)
        {
            _repository = repository;
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
            int brandId = 0, string name = null, int modelId = 0, int typeId = 0, int pageNumber = default, int PageSize = default)
        {
            var restRequest = new RestRequest("Queries/v2/Device", Method.GET);
            restRequest.AddQueryParameter("adminUserId", adminUserId.ToString());
            restRequest.AddQueryParameter("groupId", groupId.ToString());
            restRequest.AddQueryParameter("code", code.ToString());
            restRequest.AddQueryParameter("brandId", brandId.ToString());
            restRequest.AddQueryParameter("name", name);
            restRequest.AddQueryParameter("modelId", modelId.ToString());
            restRequest.AddQueryParameter("typeId", typeId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", PageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceBasicInfo>>>(restRequest);

            return requestResult.Result.Data;
        }


        public ResultViewModel<DeviceBasicInfo> GetDevice(long id = 0, int adminUserId = 0)
        {
            var restRequest = new RestRequest($"Queries/v2/Device/{id}", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("adminUserId", adminUserId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<DeviceBasicInfo>>(restRequest);
            return requestResult.Result.Data;
        }
        public PagingResult<DeviceModel> GetDeviceModels(long id = 0, string brandId = default,
            string name = default, int pageNumber = default, int PageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/Device/GetDeviceModels/{id}", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("brandId", brandId.ToString());
            restRequest.AddQueryParameter("name", name);
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", PageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<PagingResult<DeviceModel>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<AuthModeMap> GetBioAuthModeWithDeviceId(int id, int authMode)
        {
            var restRequest = new RestRequest($"Queries/v2/Device/GetBioAuthModeWithDeviceId", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("authMode", authMode.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<AuthModeMap>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<DateTime> GetLastConnectedTime(uint id)
        {
            var restRequest = new RestRequest($"Queries/v2/Device/GetBioAuthModeWithDeviceId", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<DateTime>>(restRequest);
            return requestResult.Result.Data;
        }


    }
}