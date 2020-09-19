using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class DeviceGroupRepository
    {

        private readonly RestClient _restClient;
        public DeviceGroupRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<DeviceGroup>> GetDeviceGroups(int? deviceGroupId, long userId,
            int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/DeviceGroup/GetDeviceGroups", Method.GET);
            restRequest.AddQueryParameter("deviceGroupId", deviceGroupId.ToString());
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceGroup>>>(restRequest);

            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<DeviceGroup>> GetAccessControlDeviceGroup(int id,
            int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/DeviceGroup/GetAccessControlDeviceGroup", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceGroup>>>(restRequest);

            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyDeviceGroup(DeviceGroup deviceGroup)
        {
            var restRequest = new RestRequest($"Commands/v2/DeviceGroup/", Method.PUT);
            restRequest.AddJsonBody(deviceGroup);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);

            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyDeviceGroupMember(string node, int groupId)
        {
            var restRequest = new RestRequest($"Commands/v2/DeviceGroup/ModifyDeviceGroupMember", Method.PUT);
            restRequest.AddQueryParameter("node", node);
            restRequest.AddQueryParameter("groupId", groupId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);

            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteDeviceGroup(int id)
        {
            var restRequest = new RestRequest("Commands/v2/DeviceGroup/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id",id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);

            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteDeviceGroupMember(uint id)
        {
            var restRequest = new RestRequest($"Commands/v2/DeviceGroup/DeleteDeviceGroupMember/{id}", Method.DELETE);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }


    }
}