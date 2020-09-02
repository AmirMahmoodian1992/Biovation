using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class TimeZoneRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public TimeZoneRepository(GenericRepository repository, RestClient restClient)
        {
            _repository = repository;
            _restClient = restClient;
        }

        public ResultViewModel<Domain.TimeZone> TimeZones(int id = default)
        {
            var restRequest = new RestRequest($"Queries/v2/TimeZone/{id}", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<Domain.TimeZone>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<List<Domain.TimeZone>> GetTimeZones()
        {
            var restRequest = new RestRequest($"Queries/v2/TimeZone", Method.GET);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<Domain.TimeZone>>>(restRequest);
            return requestResult.Result.Data;
        }




    }
}