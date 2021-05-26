using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class ServiceInstanceRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public ServiceInstanceRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public ResultViewModel AddServiceInstance(ServiceInstance serviceInstance, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/ServiceInstance", Method.POST);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddJsonBody(serviceInstance);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyServiceInstance(ServiceInstance serviceInstance, string token = default)
        {

            var restRequest = new RestRequest("Commands/v2/ServiceInstance", Method.PUT);
            restRequest.AddJsonBody(serviceInstance);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<ServiceInstance> GetServiceInstance(string id, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/ServiceInstance", Method.GET);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddQueryParameter("id", id);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<ServiceInstance>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel DeleteServiceInstance(string id, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/ServiceInstance", Method.DELETE);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddQueryParameter("id", id);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
    }
}
