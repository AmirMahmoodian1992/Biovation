using System;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class DeviceGroupRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public DeviceGroupRepository(GenericRepository repository, RestClient restClient)
        {
            _repository = repository;
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<DeviceGroup>> GetDeviceGroups(int? deviceGroupId, long userId,
            int pageNumber = default, int PageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/DeviceGroupController/GetDeviceGroups", Method.GET);
            restRequest.AddQueryParameter("deviceGroupId", deviceGroupId.ToString());
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", PageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceGroup>>>(restRequest);

            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<DeviceGroup>> GetAccessControlDeviceGroup(int id,
            int pageNumber = default, int PageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/DeviceGroupController/GetAccessControlDeviceGroup", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", PageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceGroup>>>(restRequest);

            return requestResult.Result.Data;
        }




    }
}