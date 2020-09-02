using System;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class AdminDeviceRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public AdminDeviceRepository(GenericRepository repository, RestClient restClient)
        {
            _repository = repository;
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<AdminDeviceGroup>> GetAdminDevicesByPersonId(int personId,
            int pageNumber = default, int PageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/AdminDevice/GetAdminDevicesByPersonId/{personId}", Method.GET);
            restRequest.AddQueryParameter("personId", personId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", PageSize.ToString());
           
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<AdminDeviceGroup>>>(restRequest);
            return requestResult.Result.Data;
        }


    }
}