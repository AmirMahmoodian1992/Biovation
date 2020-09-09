using Biovation.Domain;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class AdminDeviceRepository
    {
        private readonly RestClient _restClient;
        public AdminDeviceRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<AdminDeviceGroup>> GetAdminDevicesByPersonId(int personId,
            int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/AdminDevice/GetAdminDevicesByPersonId/{personId}", Method.GET);
            restRequest.AddQueryParameter("personId", personId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
           
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<AdminDeviceGroup>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyAdminDevice(JObject adminDevice)
        {
            var restRequest = new RestRequest($"Commands/v2/AdminDevice", Method.GET);
            restRequest.AddJsonBody(adminDevice);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
        }


    }
}