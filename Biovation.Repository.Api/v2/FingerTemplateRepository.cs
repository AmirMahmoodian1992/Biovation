using System.Threading.Tasks;
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
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<UserTemplateCount>>>(restRequest);
            return requestResult.Result.Data;
        }

        public async Task<ResultViewModel<PagingResult<FingerTemplate>>> FingerTemplates(int userId, int templateIndex,
            string fingerTemplateType, int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/FingerTemplate", Method.GET);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("templateIndex", templateIndex.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<FingerTemplate>>>(restRequest);
            return requestResult.Data;
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

        public async Task<ResultViewModel> ModifyFingerTemplate(FingerTemplate fingerTemplate, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/FingerTemplate", Method.PATCH);
            restRequest.AddJsonBody(fingerTemplate);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> DeleteFingerTemplate(int userId, int fingerIndex, string token = default)
        {
            var restRequest = new RestRequest($"Commands/v2/FingerTemplate/{userId}", Method.PATCH);
            restRequest.AddQueryParameter("fingerIndex", fingerIndex.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }


    }
}