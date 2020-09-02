using System;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class UserCardRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public UserCardRepository(GenericRepository repository, RestClient restClient)
        {
            _repository = repository;
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<UserCard>> GetCardsByFilter(long userId, bool isActive,
            int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/UserCard", Method.GET);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("isActive", isActive.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<UserCard>>>(restRequest);
            return requestResult.Result.Data;
        }

        public int ReadCardNumber(string brandName = default, int deviceId = default)
        {
            //call virdi API
            return 0;
        }



    }
}