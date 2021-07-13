using System;
using System.Collections.Generic;
using System.Text;
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
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
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

        public ResultViewModel<List<ServiceInstance>> GetServiceInstance(string id = default,string token = default)
        {
            RestRequest restRequest = null;

            if (id != default)
            {
                restRequest = new RestRequest("Queries/v2/ServiceInstance/{id}", Method.GET);
                restRequest.AddUrlSegment("id", id);
            }
            else
            {
                restRequest = new RestRequest("Queries/v2/ServiceInstance", Method.GET);
            }
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<ServiceInstance>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel DeleteServiceInstance(string id = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/ServiceInstance/{id}", Method.DELETE);

            restRequest.AddQueryParameter("id", id ?? string.Empty);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
    }
}
