﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class AccessGroupRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public AccessGroupRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public async Task<ResultViewModel<PagingResult<AccessGroup>>> GetAccessGroups(long userId = default,
            int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default, int timeZoneId = 0,
            int pageNumber = default, int pageSize = default, int nestingDepthLevel = 5, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/AccessGroup", Method.GET);
            restRequest.AddQueryParameter(nameof(userId), userId.ToString());
            restRequest.AddQueryParameter(nameof(userGroupId), userGroupId.ToString());
            restRequest.AddQueryParameter(nameof(id), id.ToString());
            restRequest.AddQueryParameter(nameof(deviceId), deviceId.ToString());
            restRequest.AddQueryParameter(nameof(deviceGroupId), deviceGroupId.ToString());
            restRequest.AddQueryParameter(nameof(timeZoneId), timeZoneId.ToString());
            restRequest.AddQueryParameter(nameof(pageNumber), pageNumber.ToString());
            restRequest.AddQueryParameter(nameof(pageSize), pageSize.ToString());
            restRequest.AddQueryParameter(nameof(nestingDepthLevel), nestingDepthLevel.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<AccessGroup>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<AccessGroup>> GetAccessGroup(int id = default, int nestingDepthLevel = 4, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/AccessGroup/{id}", Method.GET);
            restRequest.AddUrlSegment("id", id.ToString());
            restRequest.AddQueryParameter("nestingDepthLevel", nestingDepthLevel.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<AccessGroup>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> GetDeviceOfAccessGroup(int accessGroupId,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/AccessGroup/DeviceOfAccessGroup", Method.GET);
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceBasicInfo>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<List<User>>> GetAdminUserOfAccessGroup(long id = default, int accessGroupId = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/User/Users", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest);
            return requestResult.Data;
        }

        public ResultViewModel<PagingResult<ServerSideIdentificationCacheModel>>
            GetServerSideIdentificationCacheOfAccessGroup(int accessGroupId, string brandCode, long userId,
                int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/AccessGroup/ServerSideIdentificationCacheOfAccessGroup", Method.GET);
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            restRequest.AddQueryParameter("brandCode", brandCode);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<ServerSideIdentificationCacheModel>>>(restRequest);
            return requestResult.Result.Data;
        }

        public async Task<ResultViewModel> AddAccessGroup(AccessGroup accessGroup = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/AccessGroup", Method.POST);
            restRequest.AddJsonBody(accessGroup ?? new AccessGroup());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> ModifyAccessGroup(AccessGroup accessGroup, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/AccessGroup/AccessGroup", Method.PUT);
            restRequest.AddJsonBody(accessGroup);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
        public async Task<ResultViewModel> ModifyAccessGroupAdminUsers(string xmlAdminUsers = default, int accessGroupId = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/AccessGroup/AccessGroupAdminUsers", Method.PUT);
            restRequest.AddQueryParameter("xmlAdminUsers", xmlAdminUsers ?? string.Empty);
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
        public async Task<ResultViewModel> ModifyAccessGroupDeviceGroup(string xmlDeviceGroup = default, int accessGroupId = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/AccessGroup/AccessGroupDeviceGroup", Method.PUT);
            restRequest.AddQueryParameter("xmlDeviceGroup", xmlDeviceGroup ?? string.Empty);
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
        public async Task<ResultViewModel> ModifyAccessGroupUserGroup(string xmlUserGroup = default, int accessGroupId = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/AccessGroup/AccessGroupUserGroup", Method.PUT);
            restRequest.AddQueryParameter("xmlUserGroup", xmlUserGroup ?? string.Empty);
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
        public async Task<ResultViewModel> DeleteAccessGroup(int id, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/AccessGroup/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
    }
}