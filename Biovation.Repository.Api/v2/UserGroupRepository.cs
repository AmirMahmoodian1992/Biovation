using System.Collections.Generic;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class UserGroupRepository
    {
        private readonly RestClient _restClient;
        public UserGroupRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel<List<UserGroup>> UsersGroup(long userId, int userGroupId)
        {
            var restRequest = new RestRequest($"Queries/v2/UserGroup/UsersGroup", Method.GET);
            restRequest.AddQueryParameter("OnlineUserId", userId.ToString());
            restRequest.AddQueryParameter("userGroupId", userGroupId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<UserGroup>>>(restRequest);
            return requestResult.Result.Data;
        }


        public ResultViewModel<List<UserGroup>> GetAccessControlUserGroup(int id = default)
        {
            var restRequest = new RestRequest($"Queries/v2/UserGroup/AccessControlUserGroup/{id}", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<List<UserGroup>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel SyncUserGroupMember(string lstUser)
        {
            var restRequest = new RestRequest($"Queries/v2/UserGroup/SyncUserGroupMember", Method.GET);
            restRequest.AddQueryParameter("lstUser", lstUser);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel AddUserGroup(UserGroupMember userGroupMember)
        {
            var restRequest = new RestRequest($"Commands/v2/UserGroup/", Method.POST);
            restRequest.AddJsonBody(userGroupMember);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel ModifyUserGroup( UserGroup userGroup)
        {
            var restRequest = new RestRequest($"Commands/v2/UserGroup/", Method.PUT);
            restRequest.AddJsonBody(userGroup);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteUserGroups(int groupId = default)
        {
            var restRequest = new RestRequest($"Commands/v2/UserGroup/{groupId}", Method.DELETE);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        
        public ResultViewModel ModifyUserGroupMember(List<UserGroupMember> member, int userGroupId)
        {
            var restRequest = new RestRequest($"Commands/v2/UserGroup/UserGroupMember", Method.PUT);
            restRequest.AddQueryParameter("userGroupId",userGroupId.ToString());
            restRequest.AddJsonBody(member);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }




    }
}