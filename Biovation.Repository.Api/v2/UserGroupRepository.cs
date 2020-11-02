using System.Collections.Generic;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class UserGroupRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public UserGroupRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public ResultViewModel<PagingResult<UserGroup>> UserGroups(int userGroupId, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/UserGroup", Method.GET);
            restRequest.AddQueryParameter("id", userGroupId.ToString());
            //restRequest.AddQueryParameter("adminUserId", userId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<UserGroup>>>(restRequest);
            return requestResult.Result.Data;
        }


        public ResultViewModel<List<UserGroup>> GetAccessControlUserGroup(int id = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/UserGroup/AccessControlUserGroup/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<UserGroup>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel SyncUserGroupMember(string lstUser, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/UserGroup/SyncUserGroupMember", Method.GET);
            restRequest.AddQueryParameter("lstUser", lstUser);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel AddUserGroup(UserGroupMember userGroupMember, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/UserGroup", Method.POST);
            restRequest.AddJsonBody(userGroupMember);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel ModifyUserGroup(UserGroup userGroup, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/UserGroup", Method.PUT);
            restRequest.AddJsonBody(userGroup);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteUserGroup(int groupId = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/UserGroup/{groupId}", Method.DELETE);
            restRequest.AddQueryParameter("groupId", groupId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel ModifyUserGroupMember(List<UserGroupMember> member, int userGroupId, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/UserGroup/UserGroupMember", Method.PUT);
            restRequest.AddQueryParameter("userGroupId", userGroupId.ToString());
            restRequest.AddJsonBody(member);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
    }
}