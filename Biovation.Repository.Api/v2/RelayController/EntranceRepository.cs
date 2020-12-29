using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Repository.Api.v2.RelayController
{
    public class EntranceRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public EntranceRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public async Task<ResultViewModel> CreateEntrance(Entrance entrance, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Entrance", Method.POST);
            restRequest.AddJsonBody(entrance);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<Entrance>>> GetEntrances(int id = 0, string name = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4, List<DeviceBasicInfo> devices = null, List<Scheduling> schedulings = null, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Entrance", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<Entrance>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> UpdateEntrance(Entrance entrance, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Entrance", Method.PUT);
            restRequest.AddJsonBody(entrance);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> DeleteEntrance(int id, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Entrance", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
    }
}
