using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Extension;
using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Biovation.Constants;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using Newtonsoft.Json;
using RestSharp;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class UserGroupController : ControllerBase
    {
        private readonly RestClient _restClient;

        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly UserGroupService _userGroupService;
        private readonly TaskService _taskService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly DeviceBrands _deviceBrands;

        public UserGroupController(UserService userService, DeviceService deviceService, UserGroupService userGroupService, RestClient restClient, TaskService taskService, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, DeviceBrands deviceBrands)
        {
            _userService = userService;
            _deviceService = deviceService;
            _userGroupService = userGroupService;
            _restClient = restClient;
            _taskService = taskService;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _deviceBrands = deviceBrands;
        }

        [HttpGet]
        [Route("{id?}")]
        public Task<ResultViewModel<PagingResult<UserGroup>>> GetUsersGroup([FromRoute] int id = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return _userGroupService.GetUsersGroupBase(token, id);
        }

        //[HttpPost]
        //public Task<IActionResult> AddUserGroup([FromBody]UserGroup userGroup = default)
        //{
        //    throw null;
        //}//TODO...



        [HttpPut]
        public Task<ResultViewModel> ModifyUserGroup([FromBody] UserGroup userGroup)
        {
            var token = (string)HttpContext.Items["Token"];
            var creatorUser = HttpContext.GetUser();
            return _userGroupService.ModifyUserGroupBase(token, creatorUser, userGroup);
        }

        //todo: re implement based on new signature
        [HttpPatch]
        [Route("{id}/UserGroupMember")]
        public Task<ResultViewModel> ModifyUserGroupMember([FromRoute] int id, [FromBody] List<UserGroupMember> member)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() =>
            {
                //TODO we have problem here in convert string node to List<userGroupMemeber>????
                try
                {
                    if (member.Count == 0)
                        return new ResultViewModel { Validate = 1, Message = "Empty input" };

                    var strWp = JsonConvert.SerializeObject(member);
                    var wrappedDocument = $"{{ UserGroupMember: {strWp} }}";
                    var xDocument = JsonConvert.DeserializeXmlNode(wrappedDocument, "Root");
                    var node = xDocument?.OuterXml;

                    // var result = _userGroupService.ModifyUserGroupMember(node, member[0].GroupId);
                    var result = new ResultViewModel();

                    Task.Run(() =>
                    {
                        var deviceBrands = _deviceService.GetDeviceBrands(token: token)?.Data?.Data;
                        if (deviceBrands == null) return;
                        foreach (var deviceBrand in deviceBrands)
                        {
                            //_communicationManager.CallRest(
                            //    $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}UserGroup/ModifyUserGroupMember", "Post", null, $"{JsonConvert.SerializeObject(member)}");
                            var restRequest =
                                new RestRequest(
                                    $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}UserGroup/ModifyUserGroupMember",
                                    Method.POST);
                            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                            {
                                restRequest.AddHeader("Authorization",
                                    HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                            }

                            _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
                        }
                    });

                    return result;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }

        [HttpPatch]
        [Route("{id}/UsersOfGroup")]
        public Task<ResultViewModel> SendUsersOfGroup([FromRoute] int id)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() =>
            {
                try
                {
                    var deviceBrands = _deviceService.GetDeviceBrands(token: token)?.Data?.Data;
                    var userGroup = _userGroupService.UserGroups(userGroupId: id, token: token)?.Data?.Data.FirstOrDefault();
                    if (userGroup is null || deviceBrands is null) return new ResultViewModel { Success = false, Validate = 0, Message = "Provided user group is wrong", Id = id };
                    foreach (var userGroupMember in userGroup.Users)
                    {
                        var user = _userService.GetUsers(code: userGroupMember.UserId, token: token)?.Data?.Data.FirstOrDefault();

                        foreach (var deviceBrand in deviceBrands)
                        {
                            var restRequest =
                                new RestRequest(
                                    $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}User/SendUserToAllDevices",
                                    Method.POST);
                            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                            {
                                restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                            }

                            restRequest.AddJsonBody(user);
                            _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
                        }
                    }

                    return new ResultViewModel { Validate = 1, Id = id };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = "SendUsersToDevice Failed." };
                }
            });
        }

        [HttpPost]
        [Route("SyncUserGroupMember")]
        public Task<ResultViewModel> SyncUserGroupMember([FromBody] string listUsers = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return _userGroupService.SyncUserGroupMemberBase(token, listUsers);
        }
    }
}