using Biovation.Domain;
using RestSharp;
using System.Threading.Tasks;

namespace Biovation.Repository.Api.v2
{
    public class GenericCodeMappingRepository
    {
        private readonly RestClient _restClient;

        public GenericCodeMappingRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<ResultViewModel<PagingResult<GenericCodeMapping>>> GetGenericCodeMappings(int categoryId = default,
            string brandCode = default, int manufactureCode = default, int genericCode = default,
            int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest("Queries/v2/GenericCodeMapping/GenericCodeMappings", Method.GET);
            restRequest.AddQueryParameter("categoryId", categoryId.ToString());
            restRequest.AddQueryParameter("brandCode", brandCode);
            restRequest.AddQueryParameter("manufactureCode", manufactureCode.ToString());
            restRequest.AddQueryParameter("genericCode", genericCode.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<GenericCodeMapping>>>(restRequest);
            return requestResult.Data;
        }
    }
}
