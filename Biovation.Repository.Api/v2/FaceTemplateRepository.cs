using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class FaceTemplateRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public FaceTemplateRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public ResultViewModel<PagingResult<FaceTemplate>> FaceTemplates(string fingerTemplateTypeCode = default,
            long userId = 0, int index = 0, int pageNumber = default,
            int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/FaceTemplate", Method.GET);
            restRequest.AddQueryParameter("fingerTemplateTypeCode", fingerTemplateTypeCode);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("index", index.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<FaceTemplate>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyFaceTemplate(FaceTemplate faceTemplate, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/FaceTemplate/ModifyFaceTemplate", Method.PUT);
            restRequest.AddJsonBody(faceTemplate);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteFaceTemplate(long userId = 0, int index = 0, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/FaceTemplate/DeleteFaceTemplate", Method.DELETE);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("index", index.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            return requestResult.Result.Data;
        }

    }
}