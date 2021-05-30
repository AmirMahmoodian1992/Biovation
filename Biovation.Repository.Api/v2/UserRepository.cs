﻿using Biovation.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using System.Linq;

namespace Biovation.Repository.Api.v2
{
    public class UserRepository
    {
        private readonly RestClient _restClient;
        private readonly SystemInfo _systemInformation;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public UserRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager, SystemInfo systemInformation)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
            _systemInformation = systemInformation;
        }

        public async Task<ResultViewModel<PagingResult<User>>> GetUsers(int from = default,
            int size = default, bool getTemplatesData = default, long userId = default, long code = default, string filterText = default,
            int type = default, bool withPicture = default, bool isAdmin = default, int pageNumber = default,
            int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/User/Users", Method.GET);
            //restRequest.AddQueryParameter("onlineId", onlineId.ToString());
            restRequest.AddQueryParameter("from", from.ToString());
            restRequest.AddQueryParameter("size", size.ToString());
            restRequest.AddQueryParameter("getTemplatesData", getTemplatesData.ToString());
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("code", code.ToString());
            restRequest.AddQueryParameter("filterText", filterText ?? string.Empty);
            restRequest.AddQueryParameter("type", type.ToString());
            restRequest.AddQueryParameter("withPicture", withPicture.ToString());
            restRequest.AddQueryParameter("isAdmin", isAdmin.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<User>>>(restRequest);
            return requestResult.Data;
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
        public async Task<ResultViewModel> ModifyUser(User user, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/User", Method.PUT);
            restRequest.AddJsonBody(user);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
        public async Task<ResultViewModel> DeleteUser(long id = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/User/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
        public async Task<ResultViewModel> DeleteUsers(List<int> ids, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/User/DeleteUsers", Method.POST);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddJsonBody(ids);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
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

        public async Task<ResultViewModel> DeleteUserGroupOfUser(int userId, int userGroupId, int userTypeId = 1, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/User/UserGroupOfUser", Method.DELETE);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("userGroupId", userGroupId.ToString());
            restRequest.AddQueryParameter("userTypeId", userTypeId.ToString());
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> ModifyPassword(int id = default, string password = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/User/Password/{id}", Method.PATCH);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            restRequest.AddUrlSegment("id", id.ToString());
            restRequest.AddQueryParameter("password", password ?? string.Empty);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        // TODO - Verify method.
        public async void AddUser(List<Lookup> deviceBrands, User user, string token = default)
        {
            var serviceInstances = _systemInformation.Services;
            foreach (var serviceInstance in serviceInstances)
            {
                foreach (var restRequest in deviceBrands.Select(deviceBrand => 
                    new RestRequest($"/{deviceBrand.Name}/{serviceInstance.Id}/{deviceBrand.Name}User/ModifyUser", Method.POST)))
                {
                    restRequest.AddJsonBody(user);
                    restRequest.AddHeader("Authorization", token!);
                    await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                }
            }
        }

        // TODO - Verify method.
        public async void ModifyUser(List<Lookup> deviceBrands, User user, string token = default)
        {
            var serviceInstances = _systemInformation.Services;
            foreach (var serviceInstance in serviceInstances)
            {
                foreach (var restRequest in deviceBrands.Select(deviceBrand => 
                    new RestRequest($"/{deviceBrand.Name}/{serviceInstance.Id}/{deviceBrand.Name}User/ModifyUser", Method.POST)))
                {
                    restRequest.AddJsonBody(user);
                    restRequest.AddHeader("Authorization", token!);
                    await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                }
            }
        }

        // TODO - Verify method.
        public Task<IRestResponse<ResultViewModel>> EnrollFaceTemplate(DeviceBasicInfo device, int id = default, int deviceId = default, string token = default)
        {
            var restRequest = new RestRequest($@"{device.Brand.Name}/{device.ServiceInstance.Id}/{device.Brand.Name}User/EnrollFaceTemplate", Method.POST);
            restRequest.AddQueryParameter("userId", id.ToString());
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            restRequest.AddHeader("Authorization", token!);
            return _restClient.ExecuteAsync<ResultViewModel>(restRequest);
        }

        // TODO - Verify method.
        public Task<IRestResponse> UpdateUserGroupsOfUser(Lookup deviceBrand, DeviceBasicInfo device, int userId, string token = default)
        {
            var restRequest = new RestRequest(
                $"/{deviceBrand.Name}/{device.ServiceInstance.Id}/{deviceBrand.Name}User/SendUserToDevice", Method.GET);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            restRequest.AddQueryParameter("userId", $"[{userId}]");
            restRequest.AddQueryParameter("updateServerSideIdentification",
                bool.TrueString);
            restRequest.AddHeader("Authorization", token!);
            return _restClient.ExecuteAsync(restRequest);
        }

        // TODO - Verify method.
        public async void Sync(Lookup deviceBrand, DeviceBasicInfo device, UserGroupMember userGroupMember, string token = default)
        {
            var restRequest = new RestRequest($"/{deviceBrand.Name}/{device.ServiceInstance.Id}/{deviceBrand.Name}User/SendUserToDevice", Method.GET);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            restRequest.AddQueryParameter("userId", $"[{userGroupMember.UserId}]");
            restRequest.AddHeader("Authorization", token!);
            await _restClient.ExecuteAsync(restRequest);
        }

        public void DeleteUserFromAllTerminal(List<Lookup> deviceBrands, long[] usersToSync = default, string token = default)
        {
            var serviceInstances = _systemInformation.Services;
            foreach (var restRequest in from deviceBrand in deviceBrands
                                        from serviceInstance in serviceInstances
                                        select
new RestRequest($"/{deviceBrand.Name}/{serviceInstance.Id}/{deviceBrand.Name}User/DeleteUserFromAllTerminal", Method.POST))
            {
                restRequest.AddJsonBody(usersToSync ?? Array.Empty<long>());
                token ??= _biovationConfigurationManager.DefaultToken;
                restRequest.AddHeader("Authorization", token);
                _restClient.ExecuteAsync(restRequest);
            }
        }

        // TODO - Verify method.
        public Task<IRestResponse> DeleteUserFromDevice(Lookup deviceBrand, DeviceBasicInfo device, List<int> listOfUserId, string token = default)
        {
            var restRequest = new RestRequest(
                $"/{deviceBrand.Name}/{device.ServiceInstance.Id}/{deviceBrand.Name}Device/DeleteUserFromDevice",
                Method.POST);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            restRequest.AddQueryParameter("updateServerSideIdentification",
                bool.TrueString);
            restRequest.AddJsonBody(listOfUserId);
            restRequest.AddHeader("Authorization", token!);
            return _restClient.ExecuteAsync(restRequest);
        }

        // TODO - Verify method.
        public List<ResultViewModel> SendUserToAllDevices(Lookup deviceBrand, long userId, User user)
        {
            var resultList = new List<ResultViewModel>();
            var serviceInstances = _systemInformation.Services;
            foreach (var serviceInstance in serviceInstances)
            {
                var restRequest = new RestRequest($"/{deviceBrand.Name}/{serviceInstance.Id}/{deviceBrand.Name}User/SendUserToAllDevices", Method.POST);
                restRequest.AddJsonBody(user);
                var restResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest).GetAwaiter().GetResult();
                resultList.Add(new ResultViewModel
                {
                    Validate = restResult.Data?.Validate ?? 0,
                    Id = userId,
                    Message = deviceBrand.Name
                });
            }
            
            return resultList;
        }
    }
}