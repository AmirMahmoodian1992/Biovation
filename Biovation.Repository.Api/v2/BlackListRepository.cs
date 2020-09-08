using System;
using System.Collections.Generic;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class BlackListRepository
    {

        private readonly RestClient _restClient;
        public BlackListRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<BlackList>> GetBlacklist(int id = default, int userId = default,
            int deviceId = 0, DateTime? startDate = null, DateTime? endDate = null, bool isDeleted = default,
            int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/BlackList/{id}", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            restRequest.AddQueryParameter("startDate", startDate.ToString());
            restRequest.AddQueryParameter("endDate", endDate.ToString());
            restRequest.AddQueryParameter("isDeleted", isDeleted.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<BlackList>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel CreateBlackList( BlackList blackList)
        {
            var restRequest = new RestRequest($"Commands/v2/BlackList", Method.POST);
            restRequest.AddJsonBody(blackList);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
        }
        public ResultViewModel DeleteBlackList(int id = default)
        {
            var restRequest = new RestRequest($"Commands/v2/BlackList/{id}", Method.DELETE);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
        }
        public ResultViewModel DeleteBlackLists(List<uint> ids)
        {
            var restRequest = new RestRequest($"Commands/v2/BlackList/DeleteBlackLists", Method.DELETE);
            restRequest.AddJsonBody(ids);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
        }
        public ResultViewModel ChangeBlackList(BlackList blackList)
        {
            var restRequest = new RestRequest($"Commands/v2/BlackList", Method.PUT);
            restRequest.AddJsonBody(blackList);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
        }



    }
}