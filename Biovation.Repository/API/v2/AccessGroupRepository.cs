using System;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class AccessGroupRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public AccessGroupRepository(GenericRepository repository, RestClient restClient)
        {
            _repository = repository;
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<AccessGroup>> GetAccessGroups(int userId = default, int adminUserId = default,
            int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default, int pageNumber = default,
            int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/AccessGroup/", Method.GET);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<AccessGroup>>>(restRequest);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("adminUserId", adminUserId.ToString());
            restRequest.AddQueryParameter("userGroupId", userGroupId.ToString());
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            restRequest.AddQueryParameter("deviceGroupId", deviceGroupId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            return requestResult.Result.Data;
        }

        public ResultViewModel<AccessGroup> GetAccessGroup(int id = default, int nestingDepthLevel = default)
        {
            var restRequest = new RestRequest($"Queries/v2/AccessGroup/{id}", Method.GET);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<AccessGroup>>(restRequest);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("nestingDepthLevel", nestingDepthLevel.ToString());
            return requestResult.Result.Data;
        }



    }
}