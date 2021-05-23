using System;
using System.Collections.Generic;
using System.Linq;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class BlackListRepository : ControllerBase
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public BlackListRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public ResultViewModel<PagingResult<BlackList>> GetBlacklist(int id = default, int userId = default,
            int deviceId = 0, DateTime? startDate = null, DateTime? endDate = null, bool isDeleted = default,
            int pageNumber = default, int pageSize = default, string token =default)
        {
            var restRequest = new RestRequest("Queries/v2/BlackList/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            restRequest.AddQueryParameter("startDate", startDate.ToString());
            restRequest.AddQueryParameter("endDate", endDate.ToString());
            restRequest.AddQueryParameter("isDeleted", isDeleted.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<BlackList>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel CreateBlackList( BlackList blackList, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/BlackList", Method.POST);
            restRequest.AddJsonBody(blackList);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
        }
        public ResultViewModel DeleteBlackList(int id = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/BlackList/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
        }
        public ResultViewModel DeleteBlackLists(List<uint> ids, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/BlackList/DeleteBlackLists", Method.DELETE);
            restRequest.AddJsonBody(ids);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
        }
        public ResultViewModel ChangeBlackList(BlackList blackList, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/BlackList", Method.PUT);
            restRequest.AddJsonBody(blackList);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
        }
    }
}