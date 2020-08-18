﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Server.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class UserGroupController : Controller
    {
        private readonly RestClient _restClient;

        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly UserGroupService _userGroupService;

        public UserGroupController(UserService userService, DeviceService deviceService, UserGroupService userGroupService)
        {
            _userService = userService;
            _deviceService = deviceService;
            _userGroupService = userGroupService;
            _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }



        [HttpGet]
        [Route("{id}")]
        public Task<IActionResult> GetUsersGroup(long id = default, int groupId = default)
        {
            throw null;
        }

        [HttpPost]
        public Task<IActionResult> AddUserGroup([FromBody]UserGroup userGroup = default)
        {
            throw null;
        }

        [HttpPut]
        public Task<IActionResult> ModifyUserGroup([FromBody]UserGroup userGroup = default)
        {
            throw null;
        }

        [HttpDelete]
        [Route("{groupId}")]
        public Task<IActionResult> DeleteUserGroups( int groupId = default)
        {
            throw null;
        }

        [HttpPut]
        [Route("UserGroupMember")]
        public Task<IActionResult> ModifyUserGroupMember([FromBody]List<UserGroupMember> member)
        {
            throw null;
        }

        [HttpGet]
        [Route("AccessControlUserGroup/{id}")]
        public Task<IActionResult> GetAccessControlUserGroup(int id = default)
        {
            throw null;
        }

        [HttpPut]
        [Route("UsersOfGroup/{groupId}")]
        public Task<IActionResult> SendUsersOfGroup(int groupId)
        {
            throw null;
        }

        [HttpPost]
        [Route("UserGroupMember")]
        public Task<IActionResult> SyncUserGroupMember([FromBody]List<User> listUsers = default)
        {
            throw null;
        }
    }
}