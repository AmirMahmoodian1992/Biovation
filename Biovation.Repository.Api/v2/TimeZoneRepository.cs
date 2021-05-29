using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System.Collections.Generic;
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

        public async Task<ResultViewModel<PagingResult<TimeZone>>> GetTimeZones(string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/TimeZone", Method.GET);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<TimeZone>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> ModifyTimeZone(TimeZone timeZone, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/TimeZone", Method.PUT);
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

        public List<ResultViewModel> SendTimeZoneDevice(int id, DeviceBasicInfo device, string token = default)
        {
            var restRequest =
                new RestRequest(
                    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}TimeZone/SendTimeZoneToDevice",
                    Method.GET);
            restRequest.AddParameter("code", device.Code);
            restRequest.AddParameter("timeZoneId", id);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);

            return new List<ResultViewModel>(requestResult.Result.Data);
        }
    }
}