using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class ServiceInstanceRepository
    {
        private readonly RestClient _restClient;

        public ServiceInstanceRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel AddServiceInstance(ServiceInstance serviceInstance)
        {
            var restRequest = new RestRequest("Commands/v2/ServiceInstance", Method.POST);

            restRequest.AddJsonBody(serviceInstance);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyServiceInstance(ServiceInstance serviceInstance)
        {

            var restRequest = new RestRequest("Commands/v2/ServiceInstance", Method.PUT);
            restRequest.AddJsonBody(serviceInstance);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<ServiceInstance> GetServiceInstance(string id)
        {
            var restRequest = new RestRequest("Queries/v2/ServiceInstance", Method.GET);

            restRequest.AddQueryParameter("id", id);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<ServiceInstance>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel DeleteServiceInstance(string id)
        {
            var restRequest = new RestRequest("Commands/v2/ServiceInstance", Method.DELETE);

            restRequest.AddQueryParameter("id", id);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
    }
}
