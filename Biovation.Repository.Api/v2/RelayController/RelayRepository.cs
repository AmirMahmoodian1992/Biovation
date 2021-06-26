using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;

namespace Biovation.Repository.Api.v2.RelayController
{
    public class RelayRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public RelayRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public async Task<ResultViewModel<PagingResult<Relay>>> GetRelay(int id = 0, int adminUserId = 0,
           string name = null, int nodeNumber = 0, int relayHubId = 0, string description = null,
           int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4, int schedulingId = default, int deviceId = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Relay", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("adminUserId", adminUserId.ToString());
            restRequest.AddQueryParameter("name", name ?? string.Empty);
            restRequest.AddQueryParameter("description", description ?? string.Empty);
            restRequest.AddQueryParameter("nodeNumber", nodeNumber.ToString());
            restRequest.AddQueryParameter("relayHubId", relayHubId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            restRequest.AddQueryParameter("nestingDepthLevel", nestingDepthLevel.ToString());
            restRequest.AddQueryParameter("schedulingId", schedulingId.ToString());
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<Relay>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> CreateRelay(Relay relay, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Relay", Method.POST);
            restRequest.AddJsonBody(relay);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> UpdateRelay(Relay relay, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Relay", Method.PUT);
            restRequest.AddJsonBody(relay);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
        public async Task<ResultViewModel> DeleteRelay(int id = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Relay", Method.DELETE);
            restRequest.AddQueryParameter("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
    }
}
