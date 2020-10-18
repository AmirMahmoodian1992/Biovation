using System.Collections.Generic;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;

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

        public ResultViewModel<TimeZone> TimeZones(int id = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/TimeZone/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<TimeZone>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<List<TimeZone>> GetTimeZones(string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/TimeZone", Method.GET);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<TimeZone>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyTimeZone(TimeZone timeZone, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/TimeZone", Method.PUT);
            restRequest.AddJsonBody(timeZone);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteTimeZone(int id, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/TimeZone/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString()); token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
    }
}