using System;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class BlackListRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public BlackListRepository(GenericRepository repository, RestClient restClient)
        {
            _repository = repository;
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<BlackList>> GetBlacklist(int id = default, int userId = default,
            int deviceId = 0, DateTime? startDate = null, DateTime? endDate = null, bool isDeleted = default,
            int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/BlackList/{id}", Method.GET);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<BlackList>>>(restRequest);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            restRequest.AddQueryParameter("startDate", startDate.ToString());
            restRequest.AddQueryParameter("endDate", endDate.ToString());
            restRequest.AddQueryParameter("isDeleted", isDeleted.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            return requestResult.Result.Data;
        }




    }
}