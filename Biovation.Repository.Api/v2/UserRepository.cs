using System.Collections.Generic;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class UserRepository
    {
        private readonly RestClient _restClient;
        public UserRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<User>> GetUsers(long onlineId = default, int from = default,
            int size = default, bool getTemplatesData = default, long userId = default, string filterText = default,
            int type = default, bool withPicture = default, bool isAdmin = default, int pageNumber = default,
            int pageSize = default)
        {
            var restRequest = new RestRequest($"Queries/v2/User/Users", Method.GET);
            restRequest.AddQueryParameter("onlineId", onlineId.ToString());
            restRequest.AddQueryParameter("from", from.ToString());
            restRequest.AddQueryParameter("size", size.ToString());
            restRequest.AddQueryParameter("getTemplatesData", getTemplatesData.ToString());
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("filterText", filterText ?? string.Empty);
            restRequest.AddQueryParameter("type", type.ToString());
            restRequest.AddQueryParameter("withPicture", withPicture.ToString());
            restRequest.AddQueryParameter("isAdmin", isAdmin.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<User>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<List<User>> GetAdminUserOfAccessGroup(long id = default, int accessGroupId = default)
        {

            var restRequest = new RestRequest($"Queries/v2/User/Users", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<int> GetUsersCount()
        {
            var restRequest = new RestRequest($"Queries/v2/User/UsersCount", Method.GET);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<int>>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel<List<DeviceBasicInfo>> GetAuthorizedDevicesOfUser(int userId)
        {
            var restRequest = new RestRequest($"Queries/v2/User/AuthorizedDevicesOfUser", Method.GET);
            restRequest.AddQueryParameter("userId", userId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<DeviceBasicInfo>>>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel ModifyUser(User user)
        {
            var restRequest = new RestRequest($"Commands/v2/User/", Method.PUT);
            restRequest.AddJsonBody(user);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteUser(int id = default)
        {
            var restRequest = new RestRequest($"Commands/v2/User/{id}", Method.DELETE);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteUsers(List<int> ids)
        {
            var restRequest = new RestRequest($"Commands/v2/User/DeleteUsers", Method.POST);
            restRequest.AddJsonBody(ids);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteUserGroupsOfUser(int userId, int userTypeId = 1)
        {
            var restRequest = new RestRequest($"Commands/v2/User/UserGroupsOfUser", Method.DELETE);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("userTypeId", userTypeId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteUserGroupOfUser(int userId, int userGroupId, int userTypeId = 1)
        {
            var restRequest = new RestRequest($"Commands/v2/User/UserGroupOfUser", Method.DELETE);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("userGroupId", userGroupId.ToString());
            restRequest.AddQueryParameter("userTypeId", userTypeId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel ModifyPassword(int id = default, string password = default)
        {
            var restRequest = new RestRequest($"Commands/v2/User/Password/{id}", Method.PATCH);
            restRequest.AddQueryParameter("password", password ?? string.Empty);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }




    }
}