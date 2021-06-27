using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Server.Attribute;
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

        public UserGroupController(UserService userService, DeviceService deviceService, UserGroupService userGroupService, RestClient restClient)
        {
            _userService = userService;
            _deviceService = deviceService;
            _userGroupService = userGroupService;
            _restClient = restClient;
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

        [HttpPut]
        public async Task<ResultViewModel> ModifyUserGroup([FromBody] UserGroup userGroup)
        {
            //TODO: Fix null values and question marks
            try
            {
                var token = HttpContext.Items["Token"] as string;
                if (userGroup is null)
                    return new ResultViewModel
                    { Success = false, Validate = 0, Code = 404, Message = "Null user group is provided!" };

                var existingUserGroup = userGroup.Id == 0 ? null : (await _userGroupService.UserGroups(userGroup.Id, token))?.Data?.Data.FirstOrDefault();
                if (existingUserGroup is null && userGroup.Id != 0)
                {
                    return new ResultViewModel
                    {
                        Validate = 0,
                        Code = 400,
                        Message = "Provided user group id is wrong, the user group does not exist."
                    };
                }

                var usersToDelete = existingUserGroup?.Users.ExceptBy(userGroup?.Users, member => member?.UserId).ToList() ?? new List<UserGroupMember>();

                var usersToAdd = (existingUserGroup is null ? userGroup.Users :
                    userGroup.Users?.ExceptBy(existingUserGroup.Users, member => member.UserId).ToList()) ?? new List<UserGroupMember>();

                var existingAuthorizedDevicesOfUserToDelete = new Dictionary<long, List<DeviceBasicInfo>>();
                var existingAuthorizedDevicesOfUserToAdd = new Dictionary<long, List<DeviceBasicInfo>>();

                var existingAuthorizedUsersOfDevicesToDelete = new Dictionary<int, List<UserGroupMember>>();
                var newAuthorizedUsersOfDevicesToDelete = new Dictionary<int, List<UserGroupMember>>();

                var computeExistingDeletion = Task.Run(() => Parallel.ForEach(usersToDelete, user =>
                {
                    var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser((int)(user.UserId), token).Data;
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
                    var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser((int)(user.UserId), token).Data;
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

                var result = await _userGroupService.ModifyUserGroup(userGroup, token);
                if (result.Validate != 1) return result;

                var computeNewDeletion = Task.Run(() => Parallel.ForEach(usersToDelete, user =>
                {
                    var oldAuthorizedDevicesOfUser =
                        existingAuthorizedDevicesOfUserToDelete.ContainsKey(user.UserId)
                            ? existingAuthorizedDevicesOfUserToDelete[user.UserId]
                            : new List<DeviceBasicInfo>();

                    var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser((int)(user.UserId), token).Data;

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
                    var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser((int)(user.UserId), token).Data;
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
                        var device = (await _deviceService.GetDevice(deviceKey, token)).Data;
                        var usersToDeleteFromDevice = (newAuthorizedUsersOfDevicesToDelete.ContainsKey(deviceKey) && newAuthorizedUsersOfDevicesToDelete[deviceKey]?.Count > 0
                            ? existingAuthorizedUsersOfDevicesToDelete[deviceKey]
                                .ExceptBy(newAuthorizedUsersOfDevicesToDelete[deviceKey], member => member.UserId)
                            : existingAuthorizedUsersOfDevicesToDelete[deviceKey]).Select(user =>
                            new User { Code = user.UserCode, UserName = user.UserName });

                        var deleteUserRestRequest =
                            new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/DeleteUserFromDevice",
                                Method.POST);
                        deleteUserRestRequest.AddQueryParameter("code", device.Code.ToString());
                        deleteUserRestRequest.AddJsonBody(usersToDeleteFromDevice.Select(user => user.Code));
                        /*var deletionResult =*/
                        deleteUserRestRequest.AddHeader("Authorization", token!);
                        await _restClient.ExecuteAsync<ResultViewModel>(deleteUserRestRequest);

                        //return result.StatusCode == HttpStatusCode.OK ? result.Data : new List<ResultViewModel> { new ResultViewModel { Id = deviceId, Validate = 0, Message = result.ErrorMessage } };
                    });
                }

                foreach (var deviceKey in newAuthorizedUsersOfDevicesToAdd.Keys)
                {
                    await Task.Run(async () =>
                    {
                        var device = (await _deviceService.GetDevice(deviceKey, token)).Data;
                        var usersToDeleteFromDevice = (existingAuthorizedUsersOfDevicesToAdd.ContainsKey(deviceKey) && existingAuthorizedUsersOfDevicesToAdd[deviceKey]?.Count > 0
                            ? newAuthorizedUsersOfDevicesToAdd[deviceKey]
                                .ExceptBy(existingAuthorizedUsersOfDevicesToAdd[deviceKey], member => member.UserId)
                            : newAuthorizedUsersOfDevicesToAdd[deviceKey]).Select(user =>
                            new User { Code = user.UserCode, UserName = user.UserName });

                        var sendUserRestRequest =
                            new RestRequest($"{device.Brand.Name}/{device.Brand.Name}User/SendUserToDevice", Method.GET);
                        sendUserRestRequest.AddQueryParameter("code", device.Code.ToString());
                        sendUserRestRequest.AddQueryParameter("userId", JsonConvert.SerializeObject(usersToDeleteFromDevice.Select(user => user.Code)));
                        /*var additionResult =*/
                        sendUserRestRequest.AddHeader("Authorization", token!);
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

                await Task.Run(async () =>
                {
                    var deviceBrands = (await _deviceService.GetDeviceBrands(token: token))?.Data?.Data;
                    if (deviceBrands == null) return;
                    foreach (var restRequest in deviceBrands.Select(deviceBrand => new RestRequest(
                        $"{deviceBrand.Name}/{deviceBrand.Name}UserGroup/ModifyUserGroupMember",
                        Method.POST)))
                    {
                        restRequest.AddHeader("Authorization", token!);

                        await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest).ConfigureAwait(false);
                    }
                }).ConfigureAwait(false);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Message = exception.ToString() };
            }
        }

        [HttpPatch]
        [Route("{id}/UsersOfGroup")]
        public async Task<ResultViewModel> SendUsersOfGroup([FromRoute] int id)
        {
            var token = HttpContext.Items["Token"] as string;
            try
            {
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
                        var restRequest =
                            new RestRequest(
                                $"{deviceBrand.Name}/{deviceBrand.Name}User/SendUserToAllDevices",
                                Method.POST);
                        restRequest.AddHeader("Authorization", token!);
                        restRequest.AddJsonBody(user);
                        await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest).ConfigureAwait(false);
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
        public async Task<ResultViewModel> SyncUserGroupMember([FromBody] string listUsers = default)
        {
            try
            {
                var token = HttpContext.Items["Token"] as string;
                var xml = $"{{Users: {listUsers} }}";

                var xmlObject = JsonConvert.DeserializeXmlNode(xml, "Root");
                var firstStep = await _userGroupService.SyncUserGroupMember(xmlObject?.OuterXml, token);

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
        }
    }
}