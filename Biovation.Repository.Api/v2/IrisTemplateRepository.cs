using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class IrisTemplateRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public IrisTemplateRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public ResultViewModel<PagingResult<IrisTemplate>> IrisTemplates(string irisTemplateTypeCode = default,
            long userId = 0, int index = 0, int pageNumber = default,
            int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/IrisTemplate", Method.GET);
            restRequest.AddQueryParameter("fingerTemplateTypeCode", irisTemplateTypeCode ?? string.Empty);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("index", index.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<IrisTemplate>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyIrisTemplate(IrisTemplate irisTemplate, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/IrisTemplate", Method.PUT);
            restRequest.AddJsonBody(irisTemplate);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel DeleteIrisTemplate(long userId = 0, int index = 0, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/IrisTemplate", Method.DELETE);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("index", index.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
    }
}