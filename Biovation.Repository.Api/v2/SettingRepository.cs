using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Repository.Api.v2
{
    public class SettingRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public SettingRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public Task<ResultViewModel<List<Setting>>> GetSettings(string key = default, string token = default)
        {
            return Task.Run(() =>
            {
                var restRequest = new RestRequest("Queries/v2/Setting/GetSettings", Method.GET);
                restRequest.AddQueryParameter("key", key);
                token ??= _biovationConfigurationManager.DefaultToken;
                restRequest.AddHeader("Authorization", token);
                var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<Setting>>>(restRequest);
                return requestResult.Result.Data;
            });
        }

        public Task<ResultViewModel<string>> GetSetting(string key, string token = default)
        {
            return Task.Run(() =>
            {
                var restRequest = new RestRequest("Queries/v2/Setting/GetSetting", Method.GET);
                restRequest.AddQueryParameter("key", key);
                token ??= _biovationConfigurationManager.DefaultToken;
                restRequest.AddHeader("Authorization", token);
                var requestResult = _restClient.ExecuteAsync<ResultViewModel<string>>(restRequest);
                return requestResult.Result.Data;
            });
        }
    }
}
