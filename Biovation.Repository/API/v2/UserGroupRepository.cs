using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class UserGroupRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public UserGroupRepository(GenericRepository repository, RestClient restClient)
        {
            _repository = repository;
            _restClient = restClient;
        }

        public IActionResult GetUsersGroup(int groupId = default)
        {
            throw null;
        }

        public ResultViewModel<List<UserGroup>> UsersGroup(long userId, int userGroupId)
        {
            var restRequest = new RestRequest($"Queries/v2/UserGroup/GetUsersGroup", Method.GET);
            restRequest.AddQueryParameter("userId", userId.ToString());
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


    }
}