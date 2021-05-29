using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

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

        public ResultViewModel<PagingResult<DeviceGroup>> GetDeviceGroups(int? deviceGroupId,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/DeviceGroup/GetDeviceGroups", Method.GET);
            restRequest.AddQueryParameter("deviceGroupId", deviceGroupId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceGroup>>>(restRequest);

            return requestResult.Result.Data;
        }

        public async Task<ResultViewModel<PagingResult<DeviceGroup>>> GetAccessControlDeviceGroup(int id,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/DeviceGroup/GetAccessControlDeviceGroup", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceGroup>>>(restRequest);

            return requestResult.Data;
        }

        public async Task<ResultViewModel> ModifyDeviceGroup(DeviceGroup deviceGroup, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/DeviceGroup", Method.PUT);
            restRequest.AddJsonBody(deviceGroup);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            
            return requestResult.Data;
        }

        public async Task<ResultViewModel> ModifyDeviceGroupMember(string node, int groupId, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/DeviceGroup/ModifyDeviceGroupMember", Method.PUT);
            restRequest.AddQueryParameter("node", node);
            restRequest.AddQueryParameter("groupId", groupId.ToString());
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

            return requestResult.Data;
        }

        public async Task<ResultViewModel> DeleteDeviceGroup(int id, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/DeviceGroup/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

            return requestResult.Data;
        }
        public async Task<ResultViewModel> DeleteDeviceGroupMember(uint id, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/DeviceGroup/DeleteDeviceGroupMember/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public ResultViewModel DeleteUserFromDevice(DeviceBasicInfo device, IEnumerable<User> usersToDelete, string token = default)
        {
            var deleteUserRestRequest =
                new RestRequest(
                    $"{device.Brand.Name}/{device.ServiceInstance.Id}/{device.Brand.Name}Device/DeleteUserFromDevice",
                    Method.POST);

            deleteUserRestRequest.AddQueryParameter("code", device.Code.ToString());
            deleteUserRestRequest.AddJsonBody(usersToDelete.Select(user => user.Code));
            token ??= _biovationConfigurationManager.DefaultToken;
            deleteUserRestRequest.AddHeader("Authorization", token);
            return _restClient.ExecuteAsync<ResultViewModel>(deleteUserRestRequest).GetAwaiter().GetResult().Data;
        }

        public List<ResultViewModel> SendUserToDevice(DeviceBasicInfo device, IEnumerable<User> usersToAdd, string token = default)
        {
            var sendUserRestRequest =
                new RestRequest($"{device.Brand.Name}/{device.ServiceInstance.Id}/{device.Brand.Name}User/SendUserToDevice",
                    Method.GET);

            sendUserRestRequest.AddQueryParameter("code", device.Code.ToString());
            sendUserRestRequest.AddQueryParameter("userId", JsonConvert.SerializeObject(usersToAdd.Select(user => user.Code)));
            token ??= _biovationConfigurationManager.DefaultToken;
            sendUserRestRequest.AddHeader("Authorization", token);
            return _restClient.ExecuteAsync<List<ResultViewModel>>(sendUserRestRequest).GetAwaiter().GetResult().Data;
        }
    }
}