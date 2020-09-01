using System;
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

        


    }
}