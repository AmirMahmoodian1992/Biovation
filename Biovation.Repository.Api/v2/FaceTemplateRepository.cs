using System.Threading.Tasks;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class FaceTemplateRepository
    {
        private readonly RestClient _restClient;
        public FaceTemplateRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<FaceTemplate>> FaceTemplates(string fingerTemplateTypeCode = default,
            long userId = 0, int index = 0, int pageNumber = default,
            int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/FaceTemplate", Method.GET);
            restRequest.AddQueryParameter("fingerTemplateTypeCode", fingerTemplateTypeCode);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("index", index.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("PageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<FaceTemplate>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyFaceTemplate(FaceTemplate faceTemplate)
        {
            var restRequest = new RestRequest($"Commands/v2/FaceTemplate/ModifyFaceTemplate", Method.PUT);
            restRequest.AddJsonBody(faceTemplate);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteFaceTemplate(long userId = 0, int index = 0)
        {
            var restRequest = new RestRequest($"Commands/v2/FaceTemplate/DeleteFaceTemplate", Method.DELETE);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("index", index.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

    }
}