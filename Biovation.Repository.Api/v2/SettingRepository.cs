using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class SettingRepository
    {
        private readonly RestClient _restClient;

        public SettingRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public Task<ResultViewModel<List<Setting>>> GetSettings(string key = default)
        {
            return Task.Run(() =>
            {
                var restRequest = new RestRequest("Queries/v2/Setting/GetSettings", Method.GET);
                restRequest.AddQueryParameter("key", key);
                var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<Setting>>>(restRequest);
                return requestResult.Result.Data;
            });
        }

        public Task<ResultViewModel<string>> GetSetting(string key)
        {
            return Task.Run(() =>
            {
                var restRequest = new RestRequest("Queries/v2/Setting/GetSetting", Method.GET);
                restRequest.AddQueryParameter("key", key);
                var requestResult = _restClient.ExecuteAsync<ResultViewModel<string>>(restRequest);
                return requestResult.Result.Data;
            });
        }
    }
}
