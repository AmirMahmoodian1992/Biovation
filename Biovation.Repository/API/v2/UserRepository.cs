using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class UserRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public UserRepository(GenericRepository repository, RestClient restClient)
        {
            _repository = repository;
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<User>> GetUsers(long onlineId = default, int from = default,
            int size = default, bool getTemplatesData = default, long userId = default, string filterText = default,
            int type = default, bool withPicture = default, bool isAdmin = default, int pageNumber = default,
            int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/User/GetUsers", Method.GET);
            restRequest.AddQueryParameter("onlineId", onlineId.ToString());
            restRequest.AddQueryParameter("from", from.ToString());
            restRequest.AddQueryParameter("size", size.ToString());
            restRequest.AddQueryParameter("getTemplatesData", getTemplatesData.ToString());
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("filterText", filterText.ToString());
            restRequest.AddQueryParameter("type", type.ToString());
            restRequest.AddQueryParameter("withPicture", withPicture.ToString());
            restRequest.AddQueryParameter("isAdmin", isAdmin.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<User>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<List<User>> GetAdminUserOfAccessGroup(long id = default, int accessGroupId = default)
        {

            var restRequest = new RestRequest($"Queries/v2/User/GetUsers", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest);
            return requestResult.Result.Data;
        }




    }
}