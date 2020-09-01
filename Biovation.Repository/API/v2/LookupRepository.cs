using System;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class LookupRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public LookupRepository(GenericRepository repository, RestClient restClient)
        {
            _repository = repository;
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<Lookup>> GetLookups(string code = default, string name = default,
            int lookupCategoryId = default, string codePrefix = default, int pageNumber = default,
            int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/Lookup", Method.GET);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<Lookup>>>(restRequest);
            restRequest.AddQueryParameter("code", code.ToString());
            restRequest.AddQueryParameter("name", name.ToString());
            restRequest.AddQueryParameter("lookupCategoryId", lookupCategoryId.ToString());
            restRequest.AddQueryParameter("codePrefix", codePrefix.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            return requestResult.Result.Data;
        }


    }
}