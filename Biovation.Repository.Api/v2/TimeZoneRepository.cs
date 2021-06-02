using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System.Threading.Tasks;

namespace Biovation.Repository.Api.v2
{
    public class TimeZoneRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public TimeZoneRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public async Task<ResultViewModel<TimeZone>> TimeZones(int id = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/TimeZone/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<TimeZone>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<TimeZone>>> GetTimeZones(int id = default, int accessGroupId = default, string name = default, int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/TimeZone", Method.GET);
            restRequest.AddQueryParameter(nameof(id), id.ToString());
            restRequest.AddQueryParameter(nameof(accessGroupId), accessGroupId.ToString());
            restRequest.AddQueryParameter(nameof(name), name ?? string.Empty);
            restRequest.AddQueryParameter(nameof(pageNumber), pageNumber.ToString());
            restRequest.AddQueryParameter(nameof(pageSize), pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<TimeZone>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> AddTimeZone(TimeZone timeZone, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/TimeZone", Method.POST);
            restRequest.AddJsonBody(timeZone);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> ModifyTimeZone(int id, TimeZone timeZone, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/TimeZone/{id}", Method.PUT);
            restRequest.AddUrlSegment("id", id);
            restRequest.AddJsonBody(timeZone);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> DeleteTimeZone(int id, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/TimeZone/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
    }
}