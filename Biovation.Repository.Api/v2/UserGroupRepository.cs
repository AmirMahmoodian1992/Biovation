using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<ResultViewModel<PagingResult<UserGroup>>> UserGroups(int userGroupId, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/UserGroup", Method.GET);
            restRequest.AddQueryParameter("id", userGroupId.ToString());
            //restRequest.AddQueryParameter("adminUserId", userId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<UserGroup>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<List<UserGroup>>> GetAccessControlUserGroup(int id = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/UserGroup/AccessControlUserGroup/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<List<UserGroup>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> SyncUserGroupMember(string lstUser, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/UserGroup/SyncUserGroupMember", Method.GET);
            restRequest.AddQueryParameter("lstUser", lstUser);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> AddUserGroup(UserGroupMember userGroupMember, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/UserGroup", Method.POST);
            restRequest.AddJsonBody(userGroupMember);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> ModifyUserGroup(UserGroup userGroup, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/UserGroup", Method.PUT);
            restRequest.AddJsonBody(userGroup);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> DeleteUserGroup(int id = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/UserGroup/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
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