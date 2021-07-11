using Biovation.CommonClasses;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class UserGroupController : ControllerBase
    {
        private readonly AccessGroupService _accessGroupService;
        private readonly RestClient _restClient;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly UserGroupService _userGroupService;
        private readonly TaskService _taskService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        public UserGroupController(AccessGroupService accessGroupService, UserService userService, DeviceService deviceService, UserGroupService userGroupService, RestClient restClient, TaskService taskService, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities)
        {
            _userService = userService;
            _deviceService = deviceService;
            _userGroupService = userGroupService;
            _restClient = restClient;
            _taskService = taskService;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _accessGroupService = accessGroupService;
        }

        [HttpGet]
        [Route("{id?}")]
        public async Task<ResultViewModel<PagingResult<UserGroup>>> GetUsersGroup([FromRoute] int id = default)
        {
            return await _userGroupService.UserGroups(id, HttpContext.Items["Token"] as string);
        }

        //[HttpPost]
        //public Task<ResultViewModel> AddUserGroup([FromBody] UserGroup userGroup = default)
        //{
        //    var result = await _userGroupService.AddUserGroup(userGroup, token);
        //    if (result.Validate != 1) return result;
        //}

        // TODO - Verify Method
        [HttpPut]
        public async Task<ResultViewModel> ModifyUserGroup([FromBody] UserGroup userGroup)
        {
            var token = (string)HttpContext.Items["Token"];
            var creatorUser = HttpContext.GetUser();
            return await _userGroupService.ModifyUserGroupBase(token, creatorUser, userGroup);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ResultViewModel> DeleteUserGroup([FromRoute] int id = default)
        {
            return await _userGroupService.DeleteUserGroup(id, HttpContext.Items["Token"] as string);
        }

        [HttpPost]
        [Route("DeleteUserGroups")]
        public async Task<List<ResultViewModel>> DeleteUserGroups([FromBody] List<int> groupIds)
        {
            try
            {
                var resultList = new List<ResultViewModel>();
                foreach (var group in groupIds)
                {
                    var result = await _userGroupService.DeleteUserGroup(group, HttpContext.Items["Token"] as string);
                    resultList.Add(result);
                }

                return resultList;
            }
            catch (Exception exception)
            {
                return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = exception.Message } };
            }
        }
        //todo: re implement based on new signature
        [HttpPatch]
        [Route("{id}/UserGroupMember")]
        public async Task<ResultViewModel> ModifyUserGroupMember([FromRoute] int id, [FromBody] List<UserGroupMember> member)
        {
            //TODO we have problem here in convert string node to List<userGroupMemeber>????
            try
            {
                var token = HttpContext.Items["Token"] as string;
                if (member.Count == 0)
                    return new ResultViewModel { Validate = 1, Message = "Empty input" };

                //TODO !important
                //var strWp = JsonConvert.SerializeObject(member);
                //var wrappedDocument = $"{{ UserGroupMember: {strWp} }}";
                //var xDocument = JsonConvert.DeserializeXmlNode(wrappedDocument, "Root");
                //var node = xDocument?.OuterXml;

                //var result = _userGroupService.ModifyUserGroupMember(node, member[0].GroupId);
                var result = new ResultViewModel();

                //_ = Task.Run(() =>
                //{
                    _ = _userGroupService.ModifyUserGroupMember(token);

                    //var deviceBrands = _deviceService.GetDeviceBrands(token: token)?.Data?.Data;
                    //if (deviceBrands == null) return;
                    //foreach (var deviceBrand in deviceBrands)
                    //{
                    //    //_communicationManager.CallRest(
                    //    //    $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}UserGroup/ModifyUserGroupMember", "Post", null, $"{JsonConvert.SerializeObject(member)}");
                    //    var restRequest =
                    //        new RestRequest(
                    //            $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}UserGroup/ModifyUserGroupMember",
                    //            Method.POST);
                    //    if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                    //    {
                    //        restRequest.AddHeader("Authorization",
                    //            HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                    //    }

                    //    _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
                    //}
                //});

                return result;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Message = exception.ToString() };
            }
        }

        // TODO - Verify Method
        [HttpPatch]
        [Route("{id}/UsersOfGroup")]
        public async Task<ResultViewModel> SendUsersOfGroup([FromRoute] int id)
        {
            var token = HttpContext.Items["Token"] as string;
            try
            {
                var creatorUser = HttpContext.GetUser();
                var deviceBrands = (await _deviceService.GetDeviceBrands(token: token))?.Data?.Data;
                var userGroup = (await _userGroupService.UserGroups(id, token))?.Data?.Data.FirstOrDefault();
                if (userGroup is null || deviceBrands is null) return new ResultViewModel { Success = false, Validate = 0, Message = "Provided user group is wrong", Id = id };
                foreach (var userGroupMember in userGroup.Users)
                {
                    var user = (await _userService.GetUsers(code: userGroupMember.UserId, token: token))?.Data?.Data.FirstOrDefault();
                    if (user is null)
                        continue;

                    foreach (var deviceBrand in deviceBrands)
                    {
                        var accessGroups = (await _accessGroupService.GetAccessGroups(user.Id))?.Data?.Data ?? new List<AccessGroup>();
                        var userId = user.Code;

                        var task = new TaskInfo
                        {
                            Status = _taskStatuses.Queued,
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = _taskTypes.SendUsers,
                            Priority = _taskPriorities.Medium,
                            DeviceBrand = deviceBrand,
                            TaskItems = new List<TaskItem>()
                        };

                        if (!accessGroups.Any())
                        {
                            return new ResultViewModel { Id = user.Id, Validate = 0 };
                        }
                        foreach (var accessGroup in accessGroups)
                        {
                            foreach (var deviceGroup in accessGroup.DeviceGroup)
                            {
                                foreach (var deviceGroupMember in deviceGroup.Devices)
                                {
                                    task.TaskItems.Add(new TaskItem
                                    {
                                        Status = _taskStatuses.Queued,
                                        TaskItemType = _taskItemTypes.SendUser,
                                        Priority = _taskPriorities.Medium,
                                        DeviceId = deviceGroupMember.DeviceId,
                                        Data = JsonConvert.SerializeObject(new { UserId = userId }),
                                        IsParallelRestricted = true,
                                        IsScheduled = false,
                                        OrderIndex = 1
                                    });

                                }
                            }
                        }
                        await _taskService.InsertTask(task);
                        _ = _taskService.ProcessQueue(deviceBrand).ConfigureAwait(false);

                        //var restRequest =
                        //    new RestRequest(
                        //        $"{deviceBrand.Name}/{deviceBrand.Name}User/SendUserToAllDevices",
                        //        Method.POST);
                        //restRequest.AddHeader("Authorization", token!);
                        //restRequest.AddJsonBody(user);
                        //await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest).ConfigureAwait(false);
                        _ = _userGroupService.SendUsersOfGroup(id, token).ConfigureAwait(false);
                    }
                }

                return new ResultViewModel { Validate = 1, Id = id };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Message = "SendUsersToDevice Failed." };
            }
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