using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class DeviceGroupRepository
    {

        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public DeviceGroupRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public ResultViewModel<PagingResult<DeviceGroup>> GetDeviceGroups(int? deviceGroupId, long userId,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/DeviceGroup/GetDeviceGroups", Method.GET);
            restRequest.AddQueryParameter("deviceGroupId", deviceGroupId.ToString());
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceGroup>>>(restRequest);

            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<DeviceGroup>> GetAccessControlDeviceGroup(int id,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/DeviceGroup/GetAccessControlDeviceGroup", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceGroup>>>(restRequest);

            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyDeviceGroup(DeviceGroup deviceGroup, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/DeviceGroup", Method.PUT);
            restRequest.AddJsonBody(deviceGroup);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyDeviceGroupMember(string node, int groupId, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/DeviceGroup/ModifyDeviceGroupMember", Method.PUT);
            restRequest.AddQueryParameter("node", node);
            restRequest.AddQueryParameter("groupId", groupId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);

            return requestResult.Result.Data;
        }

        public ResultViewModel DeleteDeviceGroup(int id, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/DeviceGroup/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);

            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteDeviceGroupMember(uint id, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/DeviceGroup/DeleteDeviceGroupMember/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
    }
}