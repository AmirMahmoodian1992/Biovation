using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class LookupRepository
    {
        private readonly RestClient _restClient;
        public LookupRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<Lookup>> GetLookups(string code = default, string name = default,
            int lookupCategoryId = default, string codePrefix = default, int pageNumber = default,
            int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/Lookup", Method.GET);
            if (code != null) restRequest.AddQueryParameter("code", code);
            restRequest.AddQueryParameter("name", name ?? string.Empty);
            restRequest.AddQueryParameter("lookupCategoryId", lookupCategoryId.ToString());
            restRequest.AddQueryParameter("codePrefix", codePrefix ?? string.Empty);
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<Lookup>>>(restRequest);
            return requestResult.Result.Data;
        }


    }
}