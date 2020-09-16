using System.Collections.Generic;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class TimeZoneRepository
    {
        private readonly RestClient _restClient;
        public TimeZoneRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel<TimeZone> TimeZones(int id = default)
        {
            var restRequest = new RestRequest("Queries/v2/TimeZone/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<TimeZone>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<List<TimeZone>> GetTimeZones()
        {
            var restRequest = new RestRequest("Queries/v2/TimeZone", Method.GET);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<TimeZone>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyTimeZone(TimeZone timeZone)
        {
            var restRequest = new RestRequest("Commands/v2/TimeZone", Method.PUT);
            restRequest.AddJsonBody(timeZone);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteTimeZone(int id)
        {
            var restRequest = new RestRequest("Commands/v2/TimeZone/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
    }
}