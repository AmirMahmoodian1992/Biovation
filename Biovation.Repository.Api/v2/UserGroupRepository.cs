using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public List<List<ResultViewModel>> SendUsersOfGroup(List<ServiceInstance> serviceInstances, Lookup deviceBrand, User user, string token = default)
        {
            var resultViewModels = new List<List<ResultViewModel>>();
            foreach (var serviceInstance in serviceInstances)
            {
                var restRequest =
                    new RestRequest(
                        $"/biovation/api/{deviceBrand.Name}/{serviceInstance.Id}/{deviceBrand.Name}User/SendUserToAllDevices",
                        Method.POST);
                token ??= _biovationConfigurationManager.DefaultToken;
                restRequest.AddHeader("Authorization", token);

                restRequest.AddJsonBody(user);

                resultViewModels.Add(_restClient.ExecuteAsync<List<ResultViewModel>>(restRequest).GetAwaiter().GetResult().Data);
            }

            return resultViewModels;
        }

        public ResultViewModel DeleteUserFromDevice(DeviceBasicInfo device, IEnumerable<User> usersToDeleteFromDevice, string token = default)
        {
            var deleteUserRestRequest =
                new RestRequest($"{device.Brand.Name}/{device.ServiceInstance.Id}/{device.Brand.Name}Device/DeleteUserFromDevice",
                    Method.POST);

            deleteUserRestRequest.AddQueryParameter("code", device.Code.ToString());

            deleteUserRestRequest.AddJsonBody(usersToDeleteFromDevice.Select(user => user.Code));

            token ??= _biovationConfigurationManager.DefaultToken;
            deleteUserRestRequest.AddHeader("Authorization", token);

            return _restClient.ExecuteAsync<ResultViewModel>(deleteUserRestRequest).GetAwaiter().GetResult().Data;
        }

        public List<ResultViewModel> SendUserToDevice(DeviceBasicInfo device, IEnumerable<User> usersToDeleteFromDevice, string token = default)
        {
            var sendUserRestRequest =
                new RestRequest($"{device.Brand.Name}/{device.ServiceInstance.Id}/{device.Brand.Name}User/SendUserToDevice", Method.GET);

            sendUserRestRequest.AddQueryParameter("code", device.Code.ToString());

            sendUserRestRequest.AddQueryParameter("userId", JsonConvert.SerializeObject(usersToDeleteFromDevice.Select(user => user.Code)));

            token ??= _biovationConfigurationManager.DefaultToken;
            sendUserRestRequest.AddHeader("Authorization", token);

            return _restClient.ExecuteAsync<List<ResultViewModel>>(sendUserRestRequest).GetAwaiter().GetResult().Data;
        }

        public async Task<List<ResultViewModel>> ModifyUserGroupMember(Lookup deviceBrand, List<ServiceInstance> serviceInstances, string token = default)
        {
            var result = new List<ResultViewModel>();

            foreach (var serviceInstance in serviceInstances)
            {
                var restRequest =
                    new RestRequest(
                        $"/biovation/api/{deviceBrand.Name}/{serviceInstance.Id}/{deviceBrand.Name}UserGroup/ModifyUserGroupMember",
                        Method.POST);

                token ??= _biovationConfigurationManager.DefaultToken;
                restRequest.AddHeader("Authorization", token);

                var restResult = await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
                if (restResult.IsSuccessful && restResult.StatusCode == HttpStatusCode.OK && restResult.Data != null)
                    result.AddRange(restResult.Data);
            }

            return result;
        }
    }
}