using System;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class FingerTemplateRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public FingerTemplateRepository(GenericRepository repository, RestClient restClient)
        {
            _repository = repository;
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<UserTemplateCount>> GetTemplateCount()
        {
            var restRequest = new RestRequest($"Queries/v2/FingerTemplate/TemplateCount", Method.GET);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<UserTemplateCount>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<FingerTemplate>> FingerTemplates(int userId, int templateIndex,
            Lookup fingerTemplateType, int from = 0, int size = 0, int pageNumber = default,
            int PageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/FingerTemplate", Method.GET);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<FingerTemplate>>>(restRequest);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("templateIndex", templateIndex.ToString());
            restRequest.AddQueryParameter("from", from.ToString());
            restRequest.AddQueryParameter("size", size.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", PageSize.ToString());
            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<Lookup>> GetFingerTemplateTypes(string brandId)
        {
            var restRequest = new RestRequest($"Queries/v2/FingerTemplate/FingerTemplateTypes", Method.GET);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<Lookup>>>(restRequest);
            restRequest.AddQueryParameter("brandId", brandId.ToString());
            return requestResult.Result.Data;
        }


    }
}