using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class AdminDeviceRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public AdminDeviceRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public ResultViewModel<PagingResult<AdminDeviceGroup>> GetAdminDeviceGroupsByUserId(int personId,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/AdminDevice/AdminDeviceGroupsByUserId/{personId}", Method.GET);
            restRequest.AddUrlSegment("personId", personId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);

            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<AdminDeviceGroup>>>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel<PagingResult<AdminDevice>> GetAdminDevicesByUserId(int personId,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/AdminDevice/AdminDevicesByUserId/{personId}", Method.GET);
            restRequest.AddUrlSegment("personId", personId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<AdminDevice>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyAdminDevice(JObject adminDevice, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/AdminDevice", Method.PUT);
            restRequest.AddJsonBody(adminDevice);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
        }
    }
}