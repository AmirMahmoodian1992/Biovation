using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<ResultViewModel<PagingResult<Relay>>> GetRelay(int id = 0,
           string name = null, int nodeNumber = 0, int relayHubId = 0, int entranceId = 0, string description = null,
           int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4, List<Scheduling> schedulings = null, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Relay", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<Relay>>>(restRequest);
            return requestResult.Data;
        }
    }
}
