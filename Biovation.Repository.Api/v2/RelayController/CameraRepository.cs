using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System.Threading.Tasks;

namespace Biovation.Repository.Api.v2.RelayController
{
    public class CameraRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public CameraRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public async Task<ResultViewModel<PagingResult<Camera>>> GetCamera(long id = default, uint code = default, string name = default, string ip = default, int port = default,
            string brandCode = default, int modelId = default, string filterText = default, int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Camera", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("name", name ?? string.Empty);
            restRequest.AddQueryParameter(nameof(ip), ip ?? string.Empty);
            restRequest.AddQueryParameter(nameof(port), port.ToString());
            restRequest.AddQueryParameter(nameof(brandCode), brandCode ?? string.Empty);
            restRequest.AddQueryParameter(nameof(modelId), modelId.ToString());
            restRequest.AddQueryParameter(nameof(filterText), filterText ?? string.Empty);
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            restRequest.AddQueryParameter("nestingDepthLevel", nestingDepthLevel.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<Camera>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<CameraModel>>> GetCameraModel(long id = default,
        uint manufactureCode = default, string name = default, string brandCode = default, int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Camera/CameraModel", Method.GET);
            restRequest.AddQueryParameter(nameof(id), id.ToString());
            restRequest.AddQueryParameter(nameof(brandCode), brandCode ?? string.Empty);
            restRequest.AddQueryParameter(nameof(manufactureCode), manufactureCode.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            restRequest.AddQueryParameter("nestingDepthLevel", nestingDepthLevel.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<CameraModel>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> CreateCamera(Camera camera, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Camera", Method.POST);
            restRequest.AddJsonBody(camera);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> UpdateCamera(Camera camera, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Camera", Method.PUT);
            restRequest.AddJsonBody(camera);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
        public async Task<ResultViewModel> DeleteCamera(int id = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Camera", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
    }
}
