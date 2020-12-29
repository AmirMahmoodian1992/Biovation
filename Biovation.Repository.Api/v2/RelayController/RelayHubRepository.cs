using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using RestSharp;
using System.Threading.Tasks;

namespace Biovation.Repository.Api.v2.RelayController
{
    public class RelayHubRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public RelayHubRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public async Task<ResultViewModel> CreateRelayHub(RelayHub relayHub, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/RelayHub", Method.POST);
            restRequest.AddJsonBody(relayHub);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<RelayHub>>> GetRelayHubs(int id = 0, string ipAddress = null, int port = 0,
            int capacity = 0, string relayHubModel = null, string description = null, int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/RelayHub", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<RelayHub>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> UpdateRelayHub(RelayHub relayHub, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/RelayHub", Method.PUT);
            restRequest.AddJsonBody(relayHub);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> DeleteRelayHub(int id, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/RelayHub", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
    }
}
