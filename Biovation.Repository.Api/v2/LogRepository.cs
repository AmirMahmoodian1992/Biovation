using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class LogRepository
    {
        private readonly RestClient _restClient;
        public LogRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<Log>> Logs(int id = default, int deviceId = default, int userId = default, DateTime? fromDate = null, 
            DateTime? toDate = null, int pageNumber = default, int pageSize = default, string where = default,
            string order = default, long onlineUserId = default, bool? successTransfer = default)
        {
            var restRequest = new RestRequest("Queries/v2/Log", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("successTransfer", successTransfer?.ToString());
            restRequest.AddQueryParameter("fromDate", fromDate.ToString());
            restRequest.AddQueryParameter("toDate", toDate.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            restRequest.AddQueryParameter("where", @where ?? string.Empty);
            restRequest.AddQueryParameter("order", order ?? string.Empty);
            restRequest.AddQueryParameter("onlineUserId", onlineUserId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<Log>>>(restRequest);
            return requestResult.Result.Data;
        }

        public Task<ResultViewModel> AddLog(Log log)
        {
            return Task.Run(() =>
            {

                var restRequest = new RestRequest("Queries/v2/Log/AddLog", Method.POST);
                restRequest.AddJsonBody(log);
                var requestResult = _restClient.ExecuteAsync<Task<ResultViewModel>>(restRequest);
                return requestResult.Result.Data;
            });
        }
        public Task<ResultViewModel> AddLog(DataTable logs)
        {
            return Task.Run(() =>
            {

                var restRequest = new RestRequest("Commands/v2/Log/AddLogBulk", Method.POST);
                restRequest.AddJsonBody(logs);
                var requestResult = _restClient.ExecuteAsync<Task<ResultViewModel>>(restRequest);
                return requestResult.Result.Data;
            });
        }
        
        public Task<ResultViewModel> UpdateLog(DataTable logs)
        {
            return Task.Run(() =>
            {

                var restRequest = new RestRequest("Commands/v2/Log/UpdateLog", Method.PUT);
                restRequest.AddJsonBody(logs);
                var requestResult = _restClient.ExecuteAsync<Task<ResultViewModel>>(restRequest);
                return requestResult.Result.Data;
            });
        }
        public Task<ResultViewModel> AddLogImage(Log log)
        {
            return Task.Run(() =>
            {

                var restRequest = new RestRequest("Commands/v2/Log/AddLogImage", Method.PATCH);
                restRequest.AddJsonBody(log);
                var requestResult = _restClient.ExecuteAsync<Task<ResultViewModel>>(restRequest);
                return requestResult.Result.Data;
            });
        }
        
        public Task<ResultViewModel> UpdateLog(Log log)
        {
            return Task.Run(() =>
            {

                var restRequest = new RestRequest("Commands/v2/Log/UpdateLog", Method.PUT);
                restRequest.AddJsonBody(log);
                var requestResult = _restClient.ExecuteAsync<Task<ResultViewModel>>(restRequest);
                return requestResult.Result.Data;
            });
        }
        public Task<List<Log>> CheckLogInsertion(List<Log> logs)
        {
            return Task.Run(() =>
            {

                var restRequest = new RestRequest("Commands/v2/Log/CheckLogInsertion", Method.PUT);
                restRequest.AddJsonBody(logs);
                var requestResult = _restClient.ExecuteAsync<Task<List<Log>>>(restRequest);
                return requestResult.Result.Data;
            });
        }





    }
}