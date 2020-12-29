using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Biovation.Repository.Api.v2.RelayController
{
    public class SchedulingRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public SchedulingRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public async Task<ResultViewModel> CreateScheduling(Scheduling scheduling, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Scheduling", Method.POST);
            restRequest.AddJsonBody(scheduling);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<Scheduling>>> GetScheduling(int id = 0,
            TimeSpan startTime = default, TimeSpan endTime = default, string mode = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Scheduling", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<Scheduling>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> UpdateScheduling(Scheduling scheduling, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Scheduling", Method.PUT);
            restRequest.AddJsonBody(scheduling);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> DeleteScheduling(int id, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Scheduling", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
    }
}
