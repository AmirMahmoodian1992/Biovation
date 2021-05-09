using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Biovation.Repository.Api.v2
{
    public class LogRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public LogRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public async Task<ResultViewModel<PagingResult<Log>>> Logs(int id = default, int deviceId = default, int userId = default,
            DateTime? fromDate = null,
            DateTime? toDate = null, int pageNumber = default, int pageSize = default, string where = default,
            string order = default, bool? successTransfer = null, string token = default)
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
            restRequest.AddQueryParameter("where", where ?? string.Empty);
            restRequest.AddQueryParameter("order", order ?? string.Empty);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<Log>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<Log>>> LogImage(long id = default)
        {
            var restRequest = new RestRequest("Queries/v2/Log/LogImage/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<Log>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> AddLog(Log log, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Log/AddLog", Method.POST);
            restRequest.AddJsonBody(log);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        //public Task<ResultViewModel> AddLog(DataTable logs)
        //{
        //    return Task.Run(() =>
        //    {

        //        var restRequest = new RestRequest("Commands/v2/Log/AddLogBulk", Method.POST);
        //        restRequest.AddJsonBody(logs);
        //        var requestResult = _restClient.ExecuteAsync<Task<ResultViewModel>>(restRequest);
        //        return requestResult.Result.Data;
        //    });
        //}

        public async Task<ResultViewModel> AddLog(List<Log> logs, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Log/AddLogBulk", Method.POST);
            restRequest.AddJsonBody(logs);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> UpdateLog(List<Log> logs, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Log/UpdateLog", Method.POST);
            restRequest.AddJsonBody(logs);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> AddLogImage(Log log, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Log/AddLogImage", Method.PATCH);
            restRequest.AddJsonBody(log);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> UpdateLog(Log log, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Log/UpdateLog", Method.PUT);
            restRequest.AddJsonBody(log);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<List<Log>> CheckLogInsertion(List<Log> logs, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Log/CheckLogInsertion", Method.POST);
            restRequest.AddJsonBody(logs);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<List<Log>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> BroadcastLogs(List<Log> logs, string token = default)
        {
            return await Task.Run(() =>
            {
                var logsCount = logs.Count;
                var loopUpperBound = logsCount / 1000;
                loopUpperBound = loopUpperBound == 0 ? 1 : loopUpperBound;
                loopUpperBound = logsCount % 1000 <= 0 ? loopUpperBound : loopUpperBound + 1;

                ResultViewModel requestResult = null;
                Parallel.For(0, loopUpperBound, async index =>
                {
                    var shortenedLogList = logs.Skip(index * 1000).Take(1000).ToList();
                    var restRequest = new RestRequest("Commands/v2/Log/BroadcastLogs", Method.POST);
                    restRequest.AddJsonBody(shortenedLogList);
                    token ??= _biovationConfigurationManager.DefaultToken;
                    restRequest.AddHeader("Authorization", token);
                    var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                    if (requestResult != null)
                        lock (requestResult)
                        {
                            if (result is null || !result.IsSuccessful || result.StatusCode != HttpStatusCode.OK)
                                requestResult = result?.Data ?? new ResultViewModel
                                { Success = false, Message = "An error occured" };
                            else
                                requestResult.Success = requestResult.Success && result.IsSuccessful &&
                                                        result.StatusCode == HttpStatusCode.OK;
                        }
                });

                return requestResult ?? new ResultViewModel
                {
                    Success = true,
                    Message = "There are no logs to transfer"
                };
            });
        }
    }
}
