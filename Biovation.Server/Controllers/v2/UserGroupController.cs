using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public Task<ResultViewModel<PagingResult<UserGroup>>> GetUsersGroup(long userId, int userGroupId)
        {
            return Task.Run(() => _userGroupService.UsersGroup(userId, userGroupId));
        }

        [HttpPost]
        //public Task<IActionResult> AddUserGroup([FromBody]UserGroup userGroup = default)
        //{
        //    throw null;
        //}//TODO...

        [HttpPut]
        public Task<ResultViewModel> ModifyUserGroup([FromBody] UserGroup userGroup = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var existingUserGroup = userGroup != null && userGroup.Id == 0 ? null : _userGroupService.GetAccessControlUserGroup(userGroup.Id).Data[0];
                    if (existingUserGroup is null && userGroup.Id != 0)
                    {
                        return new ResultViewModel
                        {
                            Validate = 0,
                            Code = 400,
                            Message = "Provided user group id is wrong, the user group does not exist."
                        };
                    }

                    var usersToDelete = existingUserGroup?.Users.ExceptBy(userGroup.Users, member => member.UserId).ToList() ?? new List<UserGroupMember>();

                    var usersToAdd = (existingUserGroup is null ? userGroup.Users :
                        userGroup.Users?.ExceptBy(existingUserGroup.Users, member => member.UserId).ToList()) ?? new List<UserGroupMember>();

                    var existingAuthorizedDevicesOfUserToDelete = new Dictionary<long, List<DeviceBasicInfo>>();
                    var existingAuthorizedDevicesOfUserToAdd = new Dictionary<long, List<DeviceBasicInfo>>();

                    var existingAuthorizedUsersOfDevicesToDelete = new Dictionary<int, List<UserGroupMember>>();
                    var newAuthorizedUsersOfDevicesToDelete = new Dictionary<int, List<UserGroupMember>>();

                    var computeExistingDeletion = Task.Run(() => Parallel.ForEach(usersToDelete, user =>
                    {
                        var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser((int)(user.UserId)).Data;
                        if (!existingAuthorizedDevicesOfUserToDelete.ContainsKey(user.UserId))
                            existingAuthorizedDevicesOfUserToDelete.Add(user.UserId, new List<DeviceBasicInfo>());

                        existingAuthorizedDevicesOfUserToDelete[user.UserId].AddRange(authorizedDevicesOfUser);

                    })).ContinueWith(_ =>
                        Parallel.For(0, existingAuthorizedDevicesOfUserToDelete.Count, index =>
                        {
                            var element = existingAuthorizedDevicesOfUserToDelete.ElementAt(index);
                            var devices = element.Value.DistinctBy(device => device.DeviceId).ToList();
                            existingAuthorizedDevicesOfUserToDelete[element.Key] = devices;
                        }));

                    var existingAuthorizedUsersOfDevicesToAdd = new Dictionary<int, List<UserGroupMember>>();
                    var newAuthorizedUsersOfDevicesToAdd = new Dictionary<int, List<UserGroupMember>>();

                    var computeExistingAddition = Task.Run(() => Parallel.ForEach(usersToAdd, user =>
                    {
                        var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser((int)(user.UserId)).Data;
                        if (!existingAuthorizedDevicesOfUserToAdd.ContainsKey(user.UserId))
                            existingAuthorizedDevicesOfUserToAdd.Add(user.UserId, new List<DeviceBasicInfo>());

                        existingAuthorizedDevicesOfUserToAdd[user.UserId].AddRange(authorizedDevicesOfUser);
                    })).ContinueWith(_ =>
                        Parallel.For(0, existingAuthorizedDevicesOfUserToAdd.Count, index =>
                        {
                            var element = existingAuthorizedDevicesOfUserToAdd.ElementAt(index);
                            var devices = element.Value.DistinctBy(device => device.DeviceId).ToList();
                            existingAuthorizedDevicesOfUserToAdd[element.Key] = devices;
                        }));

                    Task.WaitAll(computeExistingAddition, computeExistingDeletion);

                    var result = _userGroupService.ModifyUserGroup(userGroup);
                    if (result.Validate != 1) return result;

                    var computeNewDeletion = Task.Run(() => Parallel.ForEach(usersToDelete, user =>
                    {
                        var oldAuthorizedDevicesOfUser =
                            existingAuthorizedDevicesOfUserToDelete.ContainsKey(user.UserId)
                                ? existingAuthorizedDevicesOfUserToDelete[user.UserId]
                                : new List<DeviceBasicInfo>();

                        var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser((int)(user.UserId)).Data;

                        var computeNewStateTask = Task.Run(() => Parallel.ForEach(authorizedDevicesOfUser, device =>
                        {
                            lock (newAuthorizedUsersOfDevicesToDelete)
                                if (!newAuthorizedUsersOfDevicesToDelete.ContainsKey(device.DeviceId))
                                    newAuthorizedUsersOfDevicesToDelete.Add(device.DeviceId,
                                        new List<UserGroupMember>());

                            newAuthorizedUsersOfDevicesToDelete[device.DeviceId].Add(user);
                        }));

                        var computeOldStateTask = Task.Run(() => Parallel.ForEach(oldAuthorizedDevicesOfUser, device =>
                        {
                            lock (existingAuthorizedUsersOfDevicesToDelete)
                                if (!existingAuthorizedUsersOfDevicesToDelete.ContainsKey(device.DeviceId))
                                    existingAuthorizedUsersOfDevicesToDelete.Add(device.DeviceId,
                                        new List<UserGroupMember>());

                            existingAuthorizedUsersOfDevicesToDelete[device.DeviceId].Add(user);
                        }));

                        Task.WaitAll(computeOldStateTask, computeNewStateTask);

                    })).ContinueWith(_ =>
                    {
                        var computeNewStateTask = Task.Run(() => Parallel.For(0, newAuthorizedUsersOfDevicesToDelete.Count,
                            index =>
                            {
                                var element = newAuthorizedUsersOfDevicesToDelete.ElementAt(index);
                                var users = element.Value.DistinctBy(user => user.UserId).ToList();
                                newAuthorizedUsersOfDevicesToDelete[element.Key] = users;
                            }));

                        var computeOldStateTask = Task.Run(() => Parallel.For(0, existingAuthorizedUsersOfDevicesToDelete.Count,
                                index =>
                                {
                                    var element = existingAuthorizedUsersOfDevicesToDelete.ElementAt(index);
                                    var users = element.Value.DistinctBy(user => user.UserId).ToList();
                                    existingAuthorizedUsersOfDevicesToDelete[element.Key] = users;
                                }));

                        Task.WaitAll(computeOldStateTask, computeNewStateTask);
                    });

                    var computeNewAddition = Task.Run(() => Parallel.ForEach(usersToAdd, user =>
                    {
                        var oldAuthorizedDevicesOfUser =
                            existingAuthorizedDevicesOfUserToAdd.ContainsKey(user.UserId)
                                ? existingAuthorizedDevicesOfUserToAdd[user.UserId]
                                : new List<DeviceBasicInfo>();
                        var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser((int)(user.UserId)).Data;
                        var computeNewStateTask = Task.Run(() => Parallel.ForEach(authorizedDevicesOfUser, device =>
                        {
                            lock (newAuthorizedUsersOfDevicesToAdd)
                                if (!newAuthorizedUsersOfDevicesToAdd.ContainsKey(device.DeviceId))
                                    newAuthorizedUsersOfDevicesToAdd.Add(device.DeviceId, new List<UserGroupMember>());

                            newAuthorizedUsersOfDevicesToAdd[device.DeviceId].Add(user);
                        }));

                        var computeOldStateTask = Task.Run(() => Parallel.ForEach(oldAuthorizedDevicesOfUser, device =>
                        {
                            lock (existingAuthorizedUsersOfDevicesToAdd)
                                if (!existingAuthorizedUsersOfDevicesToAdd.ContainsKey(device.DeviceId))
                                    existingAuthorizedUsersOfDevicesToAdd.Add(device.DeviceId,
                                        new List<UserGroupMember>());

                            existingAuthorizedUsersOfDevicesToAdd[device.DeviceId].Add(user);
                        }));

                        Task.WaitAll(computeOldStateTask, computeNewStateTask);
                    })).ContinueWith(_ =>
                    {
                        var computeNewStateTask = Task.Run(() => Parallel.For(0, newAuthorizedUsersOfDevicesToAdd.Count,
                            index =>
                            {
                                var element = newAuthorizedUsersOfDevicesToAdd.ElementAt(index);
                                var users = element.Value.DistinctBy(user => user.UserId).ToList();
                                newAuthorizedUsersOfDevicesToAdd[element.Key] = users;
                            }));

                        var computeOldStateTask = Task.Run(() =>
                            Parallel.For(0, existingAuthorizedUsersOfDevicesToAdd.Count,
                                index =>
                                {
                                    var element = existingAuthorizedUsersOfDevicesToAdd.ElementAt(index);
                                    var users = element.Value.DistinctBy(user => user.UserId).ToList();
                                    existingAuthorizedUsersOfDevicesToAdd[element.Key] = users;
                                }));

                        Task.WaitAll(computeOldStateTask, computeNewStateTask);
                    });

                    Task.WaitAll(computeNewAddition, computeNewDeletion);

                    foreach (var deviceKey in existingAuthorizedUsersOfDevicesToDelete.Keys)
                    {
                        await Task.Run(async () =>
                        {
                            var device = _deviceService.GetDevice(deviceKey).Data;
                            var usersToDeleteFromDevice = (newAuthorizedUsersOfDevicesToDelete.ContainsKey(deviceKey) && newAuthorizedUsersOfDevicesToDelete[deviceKey]?.Count > 0
                                ? existingAuthorizedUsersOfDevicesToDelete[deviceKey]
                                    .ExceptBy(newAuthorizedUsersOfDevicesToDelete[deviceKey], member => member.UserId)
                                : existingAuthorizedUsersOfDevicesToDelete[deviceKey]).Select(user =>
                                new User { Id = user.UserId, UserName = user.UserName });

                            var deleteUserRestRequest =
                                new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/DeleteUserFromDevice",
                                    Method.POST);
                            deleteUserRestRequest.AddQueryParameter("code", device.Code.ToString());
                            deleteUserRestRequest.AddJsonBody(usersToDeleteFromDevice.Select(user => user.Id));
                            /*var deletionResult =*/
                            await _restClient.ExecuteAsync<ResultViewModel>(deleteUserRestRequest);

                            //return result.StatusCode == HttpStatusCode.OK ? result.Data : new List<ResultViewModel> { new ResultViewModel { Id = deviceId, Validate = 0, Message = result.ErrorMessage } };
                        });
                    }

                    foreach (var deviceKey in newAuthorizedUsersOfDevicesToAdd.Keys)
                    {
                        await Task.Run(async () =>
                        {
                            var device = _deviceService.GetDevice(deviceKey).Data;
                            var usersToDeleteFromDevice = (existingAuthorizedUsersOfDevicesToAdd.ContainsKey(deviceKey) && existingAuthorizedUsersOfDevicesToAdd[deviceKey]?.Count > 0
                                ? newAuthorizedUsersOfDevicesToAdd[deviceKey]
                                    .ExceptBy(existingAuthorizedUsersOfDevicesToAdd[deviceKey], member => member.UserId)
                                : newAuthorizedUsersOfDevicesToAdd[deviceKey]).Select(user =>
                                new User { Id = user.UserId, UserName = user.UserName });

                            var sendUserRestRequest =
                                new RestRequest($"{device.Brand.Name}/{device.Brand.Name}User/SendUserToDevice", Method.GET);
                            sendUserRestRequest.AddQueryParameter("code", device.Code.ToString());
                            sendUserRestRequest.AddQueryParameter("userId", JsonConvert.SerializeObject(usersToDeleteFromDevice.Select(user => user.Id)));
                            /*var additionResult =*/
                            await _restClient.ExecuteAsync<List<ResultViewModel>>(sendUserRestRequest);

                            //return result.StatusCode == HttpStatusCode.OK ? result.Data : new List<ResultViewModel> { new ResultViewModel { Id = deviceId, Validate = 0, Message = result.ErrorMessage } };
                        });
                    }
                    return result;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }

        [HttpDelete]
        [Route("{groupId}")]
        public Task<ResultViewModel> DeleteUserGroups(int groupId = default)
        {
            return Task.Run(() => _userGroupService.DeleteUserGroups(groupId));
        }

        [HttpPut]
        [Route("UserGroupMember")]
        public Task<ResultViewModel> ModifyUserGroupMember([FromBody] List<UserGroupMember> member)
        {
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
                        var deviceBrands = _deviceService.GetDeviceBrands().Data;
                        foreach (var deviceBrand in deviceBrands)
                        {
                            //_communicationManager.CallRest(
                            //    $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}UserGroup/ModifyUserGroupMember", "Post", null, $"{JsonConvert.SerializeObject(member)}");
                            var restRequest =
                                new RestRequest(
                                    $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}UserGroup/ModifyUserGroupMember",
                                    Method.POST);
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

        [HttpGet]
        [Route("AccessControlUserGroup/{id}")]
        public Task<ResultViewModel<List<UserGroup>>> GetAccessControlUserGroup(int id = default)
        {
            return Task.Run(() => _userGroupService.GetAccessControlUserGroup(id));
        }

        [HttpPut]
        [Route("UsersOfGroup/{groupId}")]
        public Task<ResultViewModel> SendUsersOfGroup(int groupId)
        {
            return Task.Run(() =>
            {
                try
                {
                    var deviceBrands = _deviceService.GetDeviceBrands().Data;
                    var userGroup = _userGroupService.UsersGroup(userGroupId: groupId)?.Data?.Data.FirstOrDefault();
                    if (userGroup != null)
                        foreach (var unused in userGroup.Users)
                        {
                            //var user = _userService.GetUser(userGroupMember.UserId);

                            foreach (var deviceBrand in deviceBrands)
                            {
                                var restRequest =
                                    new RestRequest(
                                        $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}User/SendUserToAllDevices",
                                        Method.POST);
                                _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
                            }
                        }

                    return new ResultViewModel { Validate = 1, Id = groupId };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = "SendUsersToDevice Failed." };
                }
            });
        }

        [HttpPost]
        [Route("UserGroupMember")]
        public Task<ResultViewModel> SyncUserGroupMember([FromBody] string listUsers = default)
        {
            return Task.Run(() =>
            {
                try
                {
                    var xml = $"{{Users: {listUsers} }}";

                    var xmlObject = JsonConvert.DeserializeXmlNode(xml, "Root");
                    var firstStep = _userGroupService.SyncUserGroupMember(xmlObject?.OuterXml);

                    //if (firstStep.Validate == 1)
                    //{
                    //    var groupUser = JsonConvert.DeserializeObject<List<UserGroupMember>>(lstUsers).GroupBy(g => g.GroupId).ToList();
                    //    var count = groupUser.Count;
                    //    for (int i = 0; i < count; i++)
                    //    {
                    //        var member = _userGroupService.GetUserGroup(groupUser[i].Key);

                    //    }
                    //}

                    return new ResultViewModel { Validate = firstStep.Validate };
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message);
                    return new ResultViewModel { Validate = 0, Message = "SyncUserGroupMember Failed." };
                }
            });
        }
    }
}