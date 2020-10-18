using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class FingerTemplateRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public FingerTemplateRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public ResultViewModel<PagingResult<UserTemplateCount>> GetTemplateCount(string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/FingerTemplate/TemplateCount", Method.GET);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<UserTemplateCount>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<FingerTemplate>> FingerTemplates(int userId, int templateIndex,
            Lookup fingerTemplateType, int from = 0, int size = 0, int pageNumber = default,
            int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/FingerTemplate", Method.GET);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("templateIndex", templateIndex.ToString());
            restRequest.AddQueryParameter("from", from.ToString());
            restRequest.AddQueryParameter("size", size.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<FingerTemplate>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<Lookup>> GetFingerTemplateTypes(string brandId, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/FingerTemplate/FingerTemplateTypes", Method.GET);
            restRequest.AddQueryParameter("brandId", brandId);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<Lookup>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<int> GetFingerTemplatesCountByFingerTemplateType(Lookup fingerTemplateType, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/FingerTemplate", Method.GET);
            restRequest.AddQueryParameter("fingerTemplateType", fingerTemplateType.ToString() ?? string.Empty);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<int>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyFingerTemplate(FingerTemplate fingerTemplate, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/FingerTemplate", Method.PATCH);
            restRequest.AddJsonBody(fingerTemplate);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            return requestResult.Result.Data;
        }

        public ResultViewModel DeleteFingerTemplate(int userId, int fingerIndex, string token = default)
        {
            var restRequest = new RestRequest($"Commands/v2/FingerTemplate/{userId}", Method.PATCH);
            restRequest.AddQueryParameter("fingerIndex", fingerIndex.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            return requestResult.Result.Data;
        }


    }
}