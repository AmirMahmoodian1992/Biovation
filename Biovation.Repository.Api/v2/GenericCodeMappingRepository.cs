using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class GenericCodeMappingRepository
    {
        private readonly RestClient _restClient;

        public GenericCodeMappingRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<GenericCodeMapping>> GetGenericCodeMappings(int categoryId = default,
            string brandCode = default, int manufactureCode = default, int genericCode = default,
            int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/GenericCodeMapping/GenericCodeMappings", Method.GET);
            restRequest.AddQueryParameter("categoryId", categoryId.ToString());
            restRequest.AddQueryParameter("brandCode", brandCode);
            restRequest.AddQueryParameter("manufactureCode", manufactureCode.ToString());
            restRequest.AddQueryParameter("genericCode", genericCode.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<GenericCodeMapping>>>(restRequest);
            return requestResult.Result.Data;
        }
    }
}
