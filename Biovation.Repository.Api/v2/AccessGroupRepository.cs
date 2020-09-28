using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class AccessGroupRepository
    {
        private readonly RestClient _restClient;
        public AccessGroupRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<AccessGroup>> GetAccessGroups(long userId = default, int adminUserId = default,
            int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default, int pageNumber = default,
            int pageSize = default)
        {
            var restRequest = new RestRequest("Queries/v2/AccessGroup/", Method.GET);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("adminUserId", adminUserId.ToString());
            restRequest.AddQueryParameter("userGroupId", userGroupId.ToString());
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            restRequest.AddQueryParameter("deviceGroupId", deviceGroupId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<AccessGroup>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<AccessGroup> GetAccessGroup(int id = default, int nestingDepthLevel = 4)
        {
            var restRequest = new RestRequest("Queries/v2/AccessGroup/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            restRequest.AddQueryParameter("nestingDepthLevel", nestingDepthLevel.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<AccessGroup>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<DeviceBasicInfo>> GetDeviceOfAccessGroup(int accessGroupId,
            int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest("Queries/v2/AccessGroup/DeviceOfAccessGroup", Method.GET);
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceBasicInfo>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<PagingResult<ServerSideIdentificationCacheModel>>
            GetServerSideIdentificationCacheOfAccessGroup(int accessGroupId, string brandCode, long userId,
                int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest("Queries/v2/AccessGroup/ServerSideIdentificationCacheOfAccessGroup", Method.GET);
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            restRequest.AddQueryParameter("brandCode", brandCode);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<ServerSideIdentificationCacheModel>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel AddAccessGroup(AccessGroup accessGroup = default)
        {
            var restRequest = new RestRequest("Commands/v2/AccessGroup/", Method.POST);
            restRequest.AddJsonBody(accessGroup ?? new AccessGroup());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyAccessGroup(AccessGroup accessGroup)
        {
            var restRequest = new RestRequest("Commands/v2/AccessGroup/AccessGroup", Method.PUT);
            restRequest.AddJsonBody(accessGroup);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel ModifyAccessGroupAdminUsers(string xmlAdminUsers = default, int accessGroupId = default)
        {
            var restRequest = new RestRequest("Commands/v2/AccessGroup/AccessGroupAdminUsers", Method.PUT);
            restRequest.AddQueryParameter("xmlAdminUsers", xmlAdminUsers ?? string.Empty);
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel ModifyAccessGroupDeviceGroup(string xmlDeviceGroup = default, int accessGroupId = default)
        {
            var restRequest = new RestRequest("Commands/v2/AccessGroup/AccessGroupDeviceGroup", Method.PUT);
            restRequest.AddQueryParameter("xmlDeviceGroup", xmlDeviceGroup ?? string.Empty);
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel ModifyAccessGroupUserGroup(string xmlUserGroup = default, int accessGroupId = default)
        {
            var restRequest = new RestRequest("Commands/v2/AccessGroup/AccessGroupUserGroup", Method.PUT);
            restRequest.AddQueryParameter("xmlUserGroup", xmlUserGroup ?? string.Empty);
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteAccessGroup(int id)
        {
            var restRequest = new RestRequest("Commands/v2/AccessGroup/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

    }
}