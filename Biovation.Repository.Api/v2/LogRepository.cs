using System;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class LogRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public LogRepository(GenericRepository repository, RestClient restClient)
        {
            _repository = repository;
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<Domain.Log>> Logs(int id = default, int deviceId = default,
            int userId = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
            int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/Log", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("fromDate", fromDate.ToString());
            restRequest.AddQueryParameter("toDate", toDate.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<Domain.Log>>>(restRequest);
            return requestResult.Result.Data;
        }




    }
}