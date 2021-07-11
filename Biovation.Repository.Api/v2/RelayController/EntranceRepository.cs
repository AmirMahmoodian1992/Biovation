using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain.RelayModels;

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

        public async Task<ResultViewModel<PagingResult<Entrance>>> GetEntrances(int id = 0, string name = null,int pageNumber = 0,
            int pageSize = 0, int nestingDepthLevel = 4, int cameraId = default, int deviceId = default, int schedulingId = default, string filterText = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Entrance", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("name", name ?? string.Empty);
            restRequest.AddQueryParameter(nameof(cameraId), cameraId.ToString());
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            restRequest.AddQueryParameter("schedulingId", schedulingId.ToString());
            restRequest.AddQueryParameter(nameof(filterText), filterText ?? string.Empty);
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
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
