using Biovation.Domain;
using RestSharp;
using System.Collections.Generic;
using Biovation.CommonClasses.Manager;

namespace Biovation.Repository.Api.v2
{
    public class UserRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public UserRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public ResultViewModel<PagingResult<User>> GetUsers(int from = default,
            int size = default, bool getTemplatesData = default, long userId = default, string filterText = default,
            int type = default, bool withPicture = default, bool isAdmin = default, int pageNumber = default,
            int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/User/Users", Method.GET);
            //restRequest.AddQueryParameter("onlineId", onlineId.ToString());
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
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<User>>>(restRequest);
            return requestResult.Result.Data;
        }

        /// <summary>
        /// گرفتن لیست ادمین ها
        /// </summary>
        /// <returns></returns>
        public ResultViewModel<PagingResult<User>> GetAdminUser(long userId = 0, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/User/Users", Method.GET);
            restRequest.AddQueryParameter("onlineId", userId.ToString());
            restRequest.AddQueryParameter("isAdmin", bool.TrueString);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<User>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<List<User>> GetAdminUserOfAccessGroup(long id = default, int accessGroupId = default, string token = default)
        {

            var restRequest = new RestRequest("Queries/v2/User/Users", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<int> GetUsersCount(string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/User/UsersCount", Method.GET);
          
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<int>>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel<List<DeviceBasicInfo>> GetAuthorizedDevicesOfUser(int userId, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/User/AuthorizedDevicesOfUser", Method.GET);
            restRequest.AddQueryParameter("userId", userId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<DeviceBasicInfo>>>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel ModifyUser(User user, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/User", Method.PUT);
            restRequest.AddJsonBody(user);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteUser(int id = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/User/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteUsers(List<int> ids, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/User/DeleteUsers", Method.POST);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddJsonBody(ids);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteUserGroupsOfUser(int userId, int userTypeId = 1, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/User/UserGroupsOfUser", Method.DELETE);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("userTypeId", userTypeId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteUserGroupOfUser(int userId, int userGroupId, int userTypeId = 1, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/User/UserGroupOfUser", Method.DELETE);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("userGroupId", userGroupId.ToString());
            restRequest.AddQueryParameter("userTypeId", userTypeId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel ModifyPassword(int id = default, string password = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/User/Password/{id}", Method.PATCH);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddUrlSegment("id", id.ToString());
            restRequest.AddQueryParameter("password", password ?? string.Empty);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }




    }
}