using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Gateway.Controllers.v1
{
    [Route("biovation/api/[controller]")]
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

        //[HttpPost]
        //[Route("controller")]public ResultViewModel ModifyUserGroup(UserGroup userGroup)
        //{
        //    try
        //    {
        //        var result = _userGroupService.ModifyUserGroup(userGroup);
        //        return result;
        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.Log(exception);
        //        throw;
        //    }
        //}


        //[HttpPost]
        //[Route("controller")]public Task<ResultViewModel> ModifyUserGroup(UserGroup userGroup)
        //{
        //    return Task.Run(() =>
        //    {
        //        try
        //        {
        //            var existingUserGroup = _userGroupService.GetUserGroup(userGroup.Id);
        //            if (existingUserGroup is null)
        //                return new ResultViewModel { Validate = 0, Code = 400, Message = "Provided user group id is wrong, the user group does not exist." };

        //            var usersToDelete = existingUserGroup.Users.ExceptBy(userGroup.Users, member => member.UserId);
        //            var usersToAdd = userGroup.Users.ExceptBy(existingUserGroup.Users, member => member.UserId);

        //            //var changedUsers = new List<UserGroupMember>();
        //            //changedUsers.AddRange(usersToDelete);
        //            //changedUsers.AddRange(usersToAdd);
        //            //changedUsers = changedUsers.DistinctBy(member => member.UserId).ToList();

        //            var usersToDeleteFromDevice = new Dictionary<int, List<UserGroupMember>>();
        //            var usersToAddToDevice = new Dictionary<int, List<UserGroupMember>>();

        //            var deletionComputationTask = Task.Run(() => Parallel.ForEach(usersToDelete, userToDelete =>
        //            {
        //                var existingUserGroupsOfUser = _userGroupService.GetUserGroupsOfUser(userToDelete.UserId);
        //                var userExistingDevices = new List<DeviceBasicInfo>();
        //                var userNewAuthorizedDevices = new List<DeviceBasicInfo>();

        //                Parallel.ForEach(existingUserGroupsOfUser, userGroupOfUser =>
        //                {
        //                    //foreach (var userGroupOfUser in existingUserGroupsOfUser)
        //                    //{
        //                    var accessGroups = _accessGroupService.GetAccessGroupsOfUserGroup(userGroupOfUser.Id, 4);
        //                    Parallel.ForEach(accessGroups, accessGroup =>
        //                    {
        //                        //foreach (var accessGroup in accessGroups)
        //                        //{
        //                        var deviceGroups = accessGroup.DeviceGroup;
        //                        foreach (var deviceGroup in deviceGroups)
        //                        {
        //                            if (deviceGroup.Devices == null)
        //                                continue;

        //                            userExistingDevices.AddRange(deviceGroup.Devices);
        //                            if (userGroupOfUser.Id != userGroup.Id)
        //                                userNewAuthorizedDevices.AddRange(deviceGroup.Devices);
        //                        }
        //                    });
        //                });

        //                var devicesToDeleteUser =
        //                    userExistingDevices.ExceptBy(userNewAuthorizedDevices, device => device.DeviceId);

        //                foreach (var device in devicesToDeleteUser)
        //                {
        //                    if (!usersToDeleteFromDevice.ContainsKey(device.DeviceId))
        //                        usersToDeleteFromDevice.Add(device.DeviceId, new List<UserGroupMember>());

        //                    usersToDeleteFromDevice[device.DeviceId].Add(userToDelete);
        //                }
        //            }));

        //            var addComputationTask = Task.Run(() => Parallel.ForEach(usersToAdd, userToAdd =>
        //            {
        //                var existingUserGroupsOfUser = _userGroupService.GetUserGroupsOfUser(userToAdd.UserId);
        //                existingUserGroupsOfUser.Add(userGroup);
        //                var userExistingDevices = new List<DeviceBasicInfo>();
        //                var userNewAuthorizedDevices = new List<DeviceBasicInfo>();

        //                Parallel.ForEach(existingUserGroupsOfUser, userGroupOfUser =>
        //                {
        //                    //foreach (var userGroupOfUser in existingUserGroupsOfUser)
        //                    //{
        //                    var accessGroups = _accessGroupService.GetAccessGroupsOfUserGroup(userGroupOfUser.Id, 4);

        //                    //var newAccessGroup = _accessGroupService.GetAccessGroupsOfUserGroup(userGroup.Id, 4);
        //                    //accessGroups.AddRange(newAccessGroup);
        //                    //accessGroups = accessGroups.DistinctBy(accessGroup => accessGroup.Id).ToList();

        //                    Parallel.ForEach(accessGroups, accessGroup =>
        //                    {
        //                        //foreach (var accessGroup in accessGroups)
        //                        //{
        //                        var deviceGroups = accessGroup.DeviceGroup;
        //                        foreach (var deviceGroup in deviceGroups)
        //                        {
        //                            if (deviceGroup.Devices == null)
        //                                continue;

        //                            userNewAuthorizedDevices.AddRange(deviceGroup.Devices);
        //                            if (userGroupOfUser.Id != userGroup.Id)
        //                                userExistingDevices.AddRange(deviceGroup.Devices);
        //                        }
        //                    });
        //                });

        //                var devicesToAddUser =
        //                    userNewAuthorizedDevices.ExceptBy(userExistingDevices, device => device.DeviceId);

        //                foreach (var device in devicesToAddUser)
        //                {
        //                    if (!usersToAddToDevice.ContainsKey(device.DeviceId))
        //                        usersToAddToDevice.Add(device.DeviceId, new List<UserGroupMember>());

        //                    usersToAddToDevice[device.DeviceId].Add(userToAdd);
        //                }
        //            }));

        //            var result = _userGroupService.ModifyUserGroup(userGroup);

        //            if (result.Validate == 1)
        //            {
        //                Task.WaitAll(addComputationTask, deletionComputationTask);

        //                var deviceBrand = DeviceBrands. First(devBrand => devBrand.Code == device.Brand.Code);
        //                var deleteRequest = new RestRequest($"/{deviceBrand.Name}/{deviceBrand.Name}User/SendUserToDevice", Method.GET);
        //                deleteRequest.AddQueryParameter("code", device.Code.ToString());
        //                deleteRequest.AddQueryParameter("userId", $"[{userId}]");
        //                deleteRequest.AddQueryParameter("updateServerSideIdentification", bool.TrueString);

        //                var restResult = await _restClient.ExecuteAsync(restRequest);

        //                var deleteRequest = new RestRequest()
        //            }

        //            return result;
        //        }
        //        catch (Exception exception)
        //        {
        //            Logger.Log(exception);
        //            return new ResultViewModel { Validate = 0, Message = exception.ToString() };
        //        }
        //    });
        //}

        [HttpPost]
        [Route("ModifyUserGroup")]
        public Task<ResultViewModel> ModifyUserGroup(UserGroup userGroup)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var existingUserGroup = userGroup.Id == 0 ? null : _userGroupService.GetUserGroup(userGroup.Id);
                    if (existingUserGroup is null && userGroup.Id != 0)
                        return new ResultViewModel
                        {
                            Validate = 0,
                            Code = 400,
                            Message = "Provided user group id is wrong, the user group does not exist."
                        };

                    var usersToDelete = existingUserGroup?.Users.ExceptBy(userGroup.Users, member => member.UserId).ToList() ?? new List<UserGroupMember>();

                    var usersToAdd = (existingUserGroup is null ? userGroup.Users :
                        userGroup.Users?.ExceptBy(existingUserGroup.Users, member => member.UserId).ToList()) ?? new List<UserGroupMember>();

                    var existingAuthorizedDevicesOfUserToDelete = new Dictionary<long, List<DeviceBasicInfo>>();
                    var existingAuthorizedDevicesOfUserToAdd = new Dictionary<long, List<DeviceBasicInfo>>();

                    var existingAuthorizedUsersOfDevicesToDelete = new Dictionary<int, List<UserGroupMember>>();
                    var newAuthorizedUsersOfDevicesToDelete = new Dictionary<int, List<UserGroupMember>>();

                    var computeExistingDeletion = Task.Run(() => Parallel.ForEach(usersToDelete, user =>
                    {
                        var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser(user.UserId).Result;
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
                        var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser(user.UserId).Result;
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

                    var result = await _userGroupService.ModifyUserGroup(userGroup);
                    if (result.Validate != 1) return result;

                    var computeNewDeletion = Task.Run(() => Parallel.ForEach(usersToDelete, user =>
                    {
                        var oldAuthorizedDevicesOfUser =
                            existingAuthorizedDevicesOfUserToDelete.ContainsKey(user.UserId)
                                ? existingAuthorizedDevicesOfUserToDelete[user.UserId]
                                : new List<DeviceBasicInfo>();

                        var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser(user.UserId).Result;

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
                        var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser(user.UserId).Result;
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
                            var device = _deviceService.GetDeviceInfo(deviceKey);
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
                            var device = _deviceService.GetDeviceInfo(deviceKey);
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

        [HttpPost]
        [Route("ModifyUserGroupMemeber")]
        public ResultViewModel ModifyUserGroupMemeber(List<UserGroupMember> member)
        {
            try
            {
                if (member.Count == 0)
                    return new ResultViewModel { Validate = 1, Message = "Empty input" };

                var strWp = JsonConvert.SerializeObject(member);
                var wrappedDocument = $"{{ UserGroupMember: {strWp} }}";
                var xDocument = JsonConvert.DeserializeXmlNode(wrappedDocument, "Root");
                var node = xDocument.OuterXml;

                var result = _userGroupService.ModifyUserGroupMember(node, member[0].GroupId);

                Task.Run(() =>
                {
                    var deviceBrands = _deviceService.GetDeviceBrands();
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
        }

        [HttpGet]
        [Route("GetUsersGroup")]
        public List<UserGroup> GetUsersGroup(long userId)
        {
            try
            {
                return _userGroupService.GetUserGroups(userId);
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }

        [HttpGet]
        [Route("GetUserGroup")]
        public UserGroup GetUserGroup(int userGroupId)
        {
            try
            {
                return _userGroupService.GetUserGroup(userGroupId);
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }
        //[HttpPost]
        //[Route("controller")]public List<KeyValuePair<int, bool>> DeleteUserGroups([FromBody]List<int> groupIds)
        //{
        //    var resultList = new List<KeyValuePair<int, bool>>();

        //    foreach (var groupId in groupIds)
        //    {
        //        var result = _userGroupService.DeleteUserGroup(groupId);
        //        resultList.Add(new KeyValuePair<int, bool>(groupId, result.Validate == 1));
        //    }

        //    return resultList;
        //}

        [HttpPost]
        [Route("DeleteUserGroups")]
        public List<ResultViewModel> DeleteUserGroups([FromBody] List<int> groupIds)
        {
            try
            {
                var resultList = new List<ResultViewModel>();
                foreach (var group in groupIds)
                {
                    var result = _userGroupService.DeleteUserGroup(group);
                    resultList.Add(new ResultViewModel { Validate = result.Validate, Message = result.Message, Id = group });
                }

                return resultList;
            }
            catch (Exception exception)
            {
                return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = exception.Message } };
            }
        }

        [HttpGet]
        [Route("GetAccessControlUserGroup")]
        public List<UserGroup> GetAccessControlUserGroup(int id)
        {
            return _userGroupService.GetAccessControlUserGroup(id);
        }

        [HttpPost]
        [Route("SendUsersOfGroup")]
        public ResultViewModel SendUsersOfGroup(int userGroupId)
        {
            try
            {
                var deviceBrands = _deviceService.GetDeviceBrands();
                var userGroup = _userGroupService.GetUserGroup(userGroupId);
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

                return new ResultViewModel { Validate = 1, Id = userGroupId };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Message = "SendUsersToDevice Failed." };
            }
        }

        [HttpPost]
        [Route("SyncUserGroupMember")]
        public ResultViewModel SyncUserGroupMember(string lstUsers)
        {
            try
            {
                var xml = $"{{Users: {lstUsers} }}";

                var xmlObject = JsonConvert.DeserializeXmlNode(xml, "Root");
                var firstStep = _userGroupService.SyncUserGroupMember(xmlObject.OuterXml);

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