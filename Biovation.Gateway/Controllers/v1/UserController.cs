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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Biovation.Gateway.Controllers.v1
{
    [Route("biovation/api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly UserGroupService _userGroupService;
        private readonly AccessGroupService _accessGroupService;

        private readonly RestClient _restClient;

        public UserController(UserService userService, DeviceService deviceService, UserGroupService userGroupService, AccessGroupService accessGroupService)
        {
            _userService = userService;
            _deviceService = deviceService;
            _userGroupService = userGroupService;
            _accessGroupService = accessGroupService;
            _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }
        [HttpGet]
        [Route("GetUsersByFilter")]
        public Task<List<User>> GetUsersByFilter(long onlineUserId = 0, int from = 0, int size = 0, bool getTemplatesData = true, long userId = default, string filterText = null, int type = default, bool withPicture = true, bool isAdmin = false)
        {
            return _userService.GetUsersByFilter(onlineUserId, from, size, getTemplatesData, userId, filterText, type,
                withPicture, isAdmin);
        }

        [HttpGet]
        [Route("GetUsers")]
        public Task<List<User>> GetUsers(long onlineUserId = 0, int from = 0, int size = 0, bool getTemplatesData = true)
        {
            return Task.Run(async () =>
            {
                try
                {
                    return await _userService.GetUsers(onlineUserId, from, size, getTemplatesData);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new List<User>();
                }
            });
        }

        [HttpGet]
        [Route("GetAdminUser")]
        public List<User> GetAdminUser(long userId = 0)
        {
            return _userService.GetAdminUser(userId);
        }

        [HttpGet]
        [Route("GetAdminUserOfAccessGroup")]
        public List<User> GetAdminUserOfAccessGroup(long userId = 0, int accessGroupId = 0)
        {
            return _userService.GetAdminUserOfAccessGroup(userId, accessGroupId);
        }

        [HttpGet]
        [Route("GetUser")]
        public User GetUser(int id)
        {
            try
            {
                return _userService.GetUser(id);
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                throw;
            }
        }

        [HttpGet]
        [Route("SearchUser")]
        public List<User> SearchUser(string filterText, long userId)
        {
            try
            {
                return _userService.GetUser(filterText, userId);
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }

        [HttpGet]
        [Route("SearchUser")]////////
        public List<User> SearchUser(string filterText, int type, long userId)
        {
            try
            {
                return _userService.GetUser(filterText, type, userId);
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }

        [HttpPost]
        [Route("ModifyUser")]
        public Task<ResultViewModel> ModifyUser([FromBody] User user)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var existingUser = _userService.GetUser(user.Id, false);

                    if (existingUser != null)
                    {
                        user.UserName = string.IsNullOrWhiteSpace(user.UserName)
                            ? existingUser.UserName
                            : user.UserName;

                        user.StartDate = user.StartDate == default ? existingUser.StartDate : user.StartDate;
                        user.EndDate = user.EndDate == default ? existingUser.EndDate : user.EndDate;
                        user.AdminLevel = user.AdminLevel == default ? existingUser.AdminLevel : user.AdminLevel;
                        user.Email = string.IsNullOrWhiteSpace(user.Email) ? existingUser.Email : user.Email;
                        user.FirstName = string.IsNullOrWhiteSpace(user.FirstName)
                            ? existingUser.FirstName
                            : user.FirstName;
                        user.SurName = string.IsNullOrWhiteSpace(user.SurName) ? existingUser.SurName : user.SurName;
                        user.FullName = string.IsNullOrWhiteSpace(user.FullName)
                            ? existingUser.FullName
                            : user.FullName;
                        user.IdentityCard = user.IdentityCard ?? existingUser.IdentityCard;
                        user.Image = user.Image is null || user.Image.Length < 1 ? existingUser.Image : user.Image;
                        user.Type = user.Type == default ? existingUser.Type : user.Type;
                        user.TelNumber = string.IsNullOrWhiteSpace(user.TelNumber)
                            ? existingUser.TelNumber
                            : user.TelNumber;
                    }

                    var result = await _userService.ModifyUser(user);

                    await Task.Run(async () =>
                    {
                        var deviceBrands = _deviceService.GetDeviceBrands();
                        foreach (var deviceBrand in deviceBrands)
                        {
                            //_communicationManager.CallRest($"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}User/ModifyUser", "Post", null, $"{JsonConvert.SerializeObject(user)}");
                            var restRequest = new RestRequest($"/{deviceBrand.Name}/{deviceBrand.Name}User/ModifyUser", Method.POST);
                            restRequest.AddJsonBody(user);

                            await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                        }
                    });


                    return result;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = exception.Message };
                }
            });
        }

        [HttpPost]
        [Route("DeleteUser")]
        public List<ResultViewModel> DeleteUser(int[] ids)
        {
            try
            {
                var result = _userService.DeleteUser(ids);

                //Task.Run(() =>
                //{
                //    foreach (var user in result)
                //    {
                //        if(user.Validate == 1)
                //        {
                //            var deviceBrands = _deviceService.GetDeviceBrands();
                //            foreach (var deviceBrand in deviceBrands)
                //            {
                //                _communicationManager.CallRest(
                //                $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}User/DeleteUserFromAllTerminal", "Post", null, $"{JsonConvert.SerializeObject(ids)}");
                //            }
                //        }

                //    }

                //});
                return result;
            }
            catch (Exception exception)
            {
                return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = exception.Message } };
            }
        }

        [HttpPost]
        [Route("SendUserToDevice")]
        public List<ResultViewModel> SendUserToDevice(string deviceId, string userId)
        {
            try
            {
                var deviceIds = JsonConvert.DeserializeObject<int[]>(deviceId);
                //return _userService.SendUserToDevice(deviceId, userId);
                if (!userId.Any())
                {
                    return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = "User is empty" } };
                }

                var result = new List<ResultViewModel>();
                foreach (var device in deviceIds)
                {
                    var deviceBasic = _deviceService.GetDeviceInfo(device);
                    if (deviceBasic == null)
                    {
                        var msg = "DeviceId " + device + " does not exist.";
                        Logger.Log(msg);
                        return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = msg } };
                    }

                    //var parameters = new List<object> { $"code={deviceBasic.Code}", $"userId={userId}" };
                    //result.AddRange(_communicationManagerLst.CallRest(
                    //    $"/biovation/api/{deviceBasic.Brand.Name}/{deviceBasic.Brand.Name}User/SendUserToDevice", "Get", parameters, null));
                    var restRequest =
                        new RestRequest(
                            $"/biovation/api/{deviceBasic.Brand.Name}/{deviceBasic.Brand.Name}User/SendUserToDevice",
                            Method.GET);
                    restRequest.AddParameter("code", deviceBasic.Code);
                    restRequest.AddParameter("userId", userId);
                    result.AddRange(_restClient.ExecuteAsync<List<ResultViewModel>>(restRequest).Result.Data);
                }

                return result;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = "SendUserToDevice Failed." } };
            }
        }

        [HttpPost]
        [Route("SendUserToAllDevices")]
        public Task<List<ResultViewModel>> SendUserToAllDevices(string ids)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var userIds = JsonConvert.DeserializeObject<int[]>(ids);
                    var deviceBrands = _deviceService.GetDeviceBrands();
                    var length = userIds.Length;
                    var result = new List<ResultViewModel>();
                    for (var i = 0; i < length; i++)
                    {
                        var user = _userService.GetUser(userIds[i]);
                        if (user == null)
                        {
                            Logger.Log($"User {userIds[i]} not exists.");
                            result.Add(new ResultViewModel
                            { Validate = 0, Message = $"User {userIds[i]} not exists.", Id = userIds[i] });
                        }

                        foreach (var deviceBrand in deviceBrands)
                        {
                            //var restResult = _communicationManager.CallRest(
                            //            $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}User/SendUserToAllDevices", "Post", null, $"{JsonConvert.SerializeObject(user)}");

                            var restRequest =
                                new RestRequest($"/{deviceBrand.Name}/{deviceBrand.Name}User/SendUserToAllDevices",
                                    Method.POST);
                            restRequest.AddJsonBody(user);

                            var restResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                            result.Add(new ResultViewModel { Validate = restResult.Data?.Validate ?? 0, Id = userIds[i], Message = deviceBrand.Name });
                        }

                        //result.Add(new ResultViewModel { Validate = 1, Id = userIds[i] });
                    }

                    return result;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new List<ResultViewModel>
                        {new ResultViewModel {Validate = 0, Message = "SendUserToDevice Failed.", Id = 0}};
                }
            });
        }

        [HttpGet]
        [Route("ModifyPassword")]
        public ResultViewModel ModifyPassword(int userId, string password)
        {
            try
            {
                return _userService.ModifyPassword(userId, password);
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Validate = 0 };
            }
        }

        ///// <param name="updateUsers">لیست افرادی که تغییر کرده و در گروه بایویی هم حضور دارند و باید به دستگاههای جدید ارسال شوند</param>
        ///// <param name="changeUsers">لیست افرادی که تغییر کرده اند و باید از روی دستگاهها پاک شوند</param>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SyncDeviceAndAccessGroup")]
        public Task<bool> SyncDeviceAndAccessGroup([FromBody] ParamViewModel param)
        {
            return Task.Run(async () =>
            {
                var changeUsers = param.ChangeUsers;
                var updateUsers = param.UpdateUsers;
                if (string.IsNullOrEmpty(changeUsers))
                {
                    return false;
                }

                changeUsers = changeUsers.Trim(',');
                var usersToSync = changeUsers.Split(',').Select(s => Convert.ToInt64(s)).ToArray();
                var result = await Sync(usersToSync.Distinct().ToArray(), updateUsers);
                return result;
            });
        }

        private Task<bool> Sync(long[] usersToSync, string updateUsers)
        {
            return Task.Run(() =>
            {
                var deviceBrands = _deviceService.GetDeviceBrands();
                try
                {
                    foreach (var deviceBrand in deviceBrands)
                    {
                        //_communicationManager.CallRest($"/biovation/api/{brand.Name}/{brand.Name}User/DeleteUserFromAllTerminal", "Post", null, $"{JsonConvert.SerializeObject(lstchangeUsers)}");
                        var restRequest =
                            new RestRequest($"/{deviceBrand.Name}/{deviceBrand.Name}User/DeleteUserFromAllTerminal", Method.POST);
                        restRequest.AddJsonBody(usersToSync);

                        _restClient.ExecuteAsync(restRequest);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, "Delete User From Device");
                    return false;
                }

                if (string.IsNullOrEmpty(updateUsers.Replace("<Root/>", "")))
                {
                    return true;
                }

                //var xmlDoc = new XmlDocument();
                //xmlDoc.LoadXml(updateUsers);
                //var fromXml = JsonConvert.SerializeXmlNode(xmlDoc);
                //var fromJson = JsonConvert.DeserializeObject<List<User>>(fromXml);
                List<UserGroupMember> lstUserGroupMember;
                try
                {
                    var serializer = new XmlSerializer(typeof(List<UserGroupMember>), new XmlRootAttribute("Root"));
                    var stringReader = new StringReader(updateUsers);
                    lstUserGroupMember = (List<UserGroupMember>)serializer.Deserialize(stringReader);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, "Error on serialize users");
                    return false;
                }

                var resultUpdate = UpdateUserGroupMember(usersToSync, lstUserGroupMember);
                if (!resultUpdate)
                {
                    return false;
                }

                try
                {
                    Task.Run(() =>
                    {
                        //updateUsers = updateUsers.Trim(',');
                        //var lstupdateUsers = updateUsers.Split(',').Select(s => Convert.ToInt64(s)).ToArray();
                        var count = lstUserGroupMember.Count();
                        for (var i = 0; i < count; i++)
                        {
                            var accessGroups = _accessGroupService.GetAccessGroupsOfUser(lstUserGroupMember[i].UserId);
                            foreach (var accessGroup in accessGroups)
                            {
                                if (accessGroup.DeviceGroup == null)
                                {
                                    Logger.Log($"Not a standard access group, [{accessGroup.Id}].\n",
                                        "The access group does not have any device group.");
                                    continue;
                                }

                                foreach (var deviceGroup in accessGroup.DeviceGroup)
                                {
                                    if (deviceGroup.Devices == null)
                                    {
                                        continue;
                                    }

                                    foreach (var device in deviceGroup.Devices)
                                    {
                                        //var parameters = new List<object> { $"code={device.Code}", $"userId=[{lstUserGroupMember[i].UserId}]", };
                                        //_communicationManager.CallRest($"/biovation/api/{deviceBrand?.Name}/{deviceBrand?.Name}User/SendUserToDevice","Get", parameters, null);

                                        var deviceBrand = deviceBrands.First(devBrand => devBrand.Code == device.Brand.Code);
                                        var restRequest = new RestRequest($"/{deviceBrand.Name}/{deviceBrand.Name}User/SendUserToDevice", Method.GET);
                                        restRequest.AddQueryParameter("code", device.Code.ToString());
                                        restRequest.AddQueryParameter("userId", $"[{lstUserGroupMember[i].UserId}]");

                                        _restClient.ExecuteAsync(restRequest);
                                    }
                                }
                            }
                        }
                    });

                    return true;

                }
                catch (Exception exception)
                {
                    Logger.Log(exception, "Add User To Devices");
                    return false;
                }
            });
        }

        //[HttpPost]
        //private bool Sync(long[] usersList)
        //{
        //    var deviceBrands = _deviceService.GetDeviceBrands();
        //    try
        //    {
        //        foreach (var brand in deviceBrands)
        //        {
        //            _communicationManager.CallRest(
        //                        $"/biovation/api/{brand.Name}/{brand.Name}User/DeleteUserFromAllTerminal", "Post", null, $"{JsonConvert.SerializeObject(usersList)}");
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.Log(exception, "Delete User From Device");
        //        return false;
        //    }

        //    if (string.IsNullOrEmpty(updateUsers.Replace("<Root/>", "")))
        //    {
        //        return true;
        //    }
        //    //var xmlDoc = new XmlDocument();
        //    //xmlDoc.LoadXml(updateUsers);
        //    //var fromXml = JsonConvert.SerializeXmlNode(xmlDoc);
        //    //var fromJson = JsonConvert.DeserializeObject<List<User>>(fromXml);
        //    List<UserGroupMember> lstUserGroupMember;
        //    try
        //    {
        //        var serializer = new XmlSerializer(typeof(List<UserGroupMember>), new XmlRootAttribute("Root"));
        //        var stringReader = new StringReader(updateUsers);
        //        lstUserGroupMember = (List<UserGroupMember>)serializer.Deserialize(stringReader);
        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.Log(exception, "Error on serialize users");
        //        return false;
        //    }

        //    var resultUpdate = UpdateUserGroupMember(usersList, lstUserGroupMember);
        //    if (!resultUpdate)
        //    {
        //        return false;
        //    }
        //    try
        //    {
        //        Task.Run(() =>
        //        {
        //            //updateUsers = updateUsers.Trim(',');
        //            //var lstupdateUsers = updateUsers.Split(',').Select(s => Convert.ToInt64(s)).ToArray();
        //            var service = new AccessGroupService();
        //            var count = lstUserGroupMember.Count();
        //            for (var i = 0; i < count; i++)
        //            {
        //                var accessGroups = service.GetAccessGroupsOfUser(lstUserGroupMember[i].UserId);
        //                foreach (var accessGroup in accessGroups)
        //                {
        //                    if (accessGroup.DeviceGroup == null)
        //                    {
        //                        Logger.Log($"Not a standard access group, [{accessGroup.Id}].\n", "The access group does not have any device group.");
        //                        continue;
        //                    }
        //                    foreach (var deviceGroup in accessGroup.DeviceGroup)
        //                    {
        //                        if (deviceGroup.Devices == null)
        //                        {
        //                            continue;
        //                        }
        //                        foreach (var device in deviceGroup.Devices)
        //                        {
        //                            var deviceBrand = deviceBrands.FirstOrDefault(devBrand => devBrand.Id == device.BrandId);
        //                            var parameters = new List<object> { $"code={device.Code}", $"userId=[{lstUserGroupMember[i].UserId}]", };
        //                            _communicationManager.CallRest(
        //                                $"/biovation/api/{deviceBrand?.Name}/{deviceBrand?.Name}User/SendUserToDevice", "Get", parameters, null);
        //                        }
        //                    }
        //                }
        //            }
        //        });

        //        return true;

        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.Log(exception, "Add User To Devices");
        //        return false;
        //    }
        //}

        [HttpPost]
        [Route("EnrollFaceTemplate")]
        public Task<ResultViewModel> EnrollFaceTemplate(int userId, int deviceId)
        {
            return Task.Run(async () =>
            {
                var user = _userService.GetUser(userId, false);
                if (user is null)
                    return new ResultViewModel { Validate = 0, Id = userId, Message = "Wrong user id is provided." };

                var device = _deviceService.GetDeviceInfo(deviceId);
                if (device is null)
                    return new ResultViewModel { Validate = 0, Id = deviceId, Message = "Wrong device id is provided." };

                var restRequest = new RestRequest($@"{device.Brand.Name}/{device.Brand.Name}User/EnrollFaceTemplate", Method.POST);
                restRequest.AddQueryParameter("userId", userId.ToString());
                restRequest.AddQueryParameter("deviceId", deviceId.ToString());

                var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                return result.StatusCode == HttpStatusCode.OK ? result.Data : new ResultViewModel { Validate = 0, Id = (long)result.StatusCode, Message = result.ErrorMessage };
            });
        }

        private bool UpdateUserGroupMember(long[] userIds, List<UserGroupMember> lstToAdd)
        {
            try
            {
                var count = userIds.Length;
                var groupIds = new List<int>();
                for (var i = 0; i < count; i++)
                {
                    var group = _userGroupService.GetUserGroupsOfUser(userIds[i]);
                    groupIds.AddRange(group.Select(s => s.Id));
                }
                if (groupIds.Any())
                {
                    var grpIds = string.Join(",", groupIds.Distinct());

                    var restRequest = new RestRequest("/UserGroupMember/GetUserGroupMemberDetail", Method.GET);
                    restRequest.AddQueryParameter("userGroupId", grpIds);
                    //var parameters = new List<object> { $"userGroupid={grpIds}", };
                    //var member = _communicationUserGroup.CallRest("/api/Biovation/UserGroupMember/GetUserGroupMemberDetail", "Get", parameters);
                    var member = _restClient.Execute<List<UserGroupMember>>(restRequest);

                    var grpMember = member?.Data.GroupBy(g => g.GroupId).ToList() ?? new List<IGrouping<int, UserGroupMember>>();
                    foreach (var members in grpMember)
                    {
                        var strWp = JsonConvert.SerializeObject(members);
                        var wrappedDocument = $"{{ UserGroupMember: {strWp} }}";
                        var xDocument = JsonConvert.DeserializeXmlNode(wrappedDocument, "Root");
                        var node = xDocument.OuterXml;

                        _userGroupService.ModifyUserGroupMember(node, members.Key);
                    }
                }

                foreach (var userMember in lstToAdd)
                {
                    _userGroupService.AddUserGroupMember(userMember);
                }
                return true;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, "Error on Get User Group Member");
                return false;
            }
        }

        [HttpGet]
        [Route("UpdateUserGroupsOfUsers")]
        public List<ResultViewModel> UpdateUserGroupsOfUsers(/*string userIds,*/ string usersGroupIds, bool sendUsersToDevice = true)
        {
            try
            {
                var resultList = new List<ResultViewModel>();
                //var userIdList = JsonConvert.DeserializeObject<List<int>>(userIds);
                var userGroupIdList = JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(usersGroupIds);

                var deviceBrands = _deviceService.GetDeviceBrands();

                foreach (var userId in userGroupIdList.Keys)
                {
                    if (!userGroupIdList.ContainsKey(userId))
                        continue;

                    // for rolling back on problem occuring
                    var userExistingDevices = new List<DeviceBasicInfo>();
                    var userGroupsOfUser = _userGroupService.GetUserGroupsOfUser(userId);
                    foreach (var userGroup in userGroupsOfUser)
                    {
                        var accessGroups = _accessGroupService.GetAccessGroupsOfUserGroup(userGroup.Id, 4);
                        foreach (var accessGroup in accessGroups)
                        {
                            var deviceGroups = accessGroup.DeviceGroup;
                            foreach (var deviceGroup in deviceGroups)
                            {
                                if (deviceGroup.Devices == null)
                                    continue;

                                userExistingDevices.AddRange(deviceGroup.Devices);
                            }
                        }
                    }

                    var result = _userService.DeleteUserGroupsOfUser(userId);
                    if (result.Validate != 1)
                    {
                        resultList.Add(new ResultViewModel { Id = userId, Validate = 0, Message = $"Cannot update user groups of user {userId}" });
                        Logger.Log($"Cannot update user groups of user {userId}");

                        foreach (var userGroup in userGroupsOfUser)
                        {
                            try
                            {
                                var userGroupMember = userGroup.Users.FirstOrDefault(userGroupMem => userGroupMem.UserId == userId);
                                _userGroupService.AddUserGroupMember(userGroupMember);
                            }
                            catch (Exception)
                            {
                                //ignore
                            }
                        }
                        continue;
                    }

                    foreach (var userGroupId in userGroupIdList[userId])
                    {
                        _userGroupService.AddUserGroupMember(new UserGroupMember
                        {
                            UserId = userId,
                            GroupId = userGroupId,
                            UserType = 1.ToString(),
                            UserTypeTitle = string.Empty
                        });
                    }

                    Logger.Log($"User groups of user {userId} updated successfully");
                    resultList.Add(new ResultViewModel { Id = userId, Validate = 1, Message = $"User groups of user {userId} updated successfully" });


                    //foreach (var deviceBrand in deviceBrands)
                    //{
                    //    Task.Run(async () =>
                    //    {
                    //        var modifyUserGroupRestRequest =
                    //            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}UserGroup/ModifyUserGroupMember", Method.POST);
                    //        modifyUserGroupRestRequest.AddJsonBody(new List<UserGroupMember>());
                    //       await _restClient.ExecuteAsync<ResultViewModel>(modifyUserGroupRestRequest);
                    //    });
                    //}

                    if (sendUsersToDevice)
                    {
                        try
                        {
                            Task.Run(async () =>
                            {
                                var devicesToExistsOn = new List<DeviceBasicInfo>();
                                var accessGroups = _accessGroupService.GetAccessGroupsOfUser(userId, 4);
                                foreach (var accessGroup in accessGroups)
                                {
                                    if (accessGroup.DeviceGroup == null)
                                    {
                                        Logger.Log($"Not a standard access group, [{accessGroup.Id}].",
                                            "The access group does not have any device group.");
                                        continue;
                                    }

                                    foreach (var deviceGroup in accessGroup.DeviceGroup)
                                    {
                                        if (deviceGroup.Devices == null)
                                            continue;

                                        devicesToExistsOn.AddRange(deviceGroup.Devices);
                                    }
                                }


                                var devicesToDelete =
                                    userExistingDevices.ExceptBy(devicesToExistsOn, device => device.DeviceId).ToList();

                                var devicesToAdd =
                                    devicesToExistsOn.ExceptBy(userExistingDevices, device => device.DeviceId).ToList();

                                foreach (var device in devicesToAdd)
                                {
                                    var deviceBrand = deviceBrands.First(devBrand => devBrand.Code == device.Brand.Code);
                                    var restRequest = new RestRequest($"/{deviceBrand.Name}/{deviceBrand.Name}User/SendUserToDevice", Method.GET);
                                    restRequest.AddQueryParameter("code", device.Code.ToString());
                                    restRequest.AddQueryParameter("userId", $"[{userId}]");
                                    restRequest.AddQueryParameter("updateServerSideIdentification", bool.TrueString);

                                    var restResult = await _restClient.ExecuteAsync(restRequest);

                                    if (restResult.IsSuccessful && restResult.StatusCode == HttpStatusCode.OK)
                                        resultList.Add(new ResultViewModel
                                        {
                                            Id = userId,
                                            Validate = 1,
                                            Message =
                                                $"User {userId} deleted from device {device.Code} successfully"
                                        });
                                }

                                foreach (var deviceToDelete in devicesToDelete)
                                {
                                    var deviceBrand = deviceBrands.First(devBrand => devBrand.Code == deviceToDelete.Brand.Code);
                                    var listOfUserId = new List<int> { userId };
                                    var restRequest = new RestRequest($"/{deviceBrand.Name}/{deviceBrand.Name}Device/DeleteUserFromDevice", Method.POST);
                                    restRequest.AddQueryParameter("code", deviceToDelete.Code.ToString());
                                    restRequest.AddQueryParameter("updateServerSideIdentification", bool.TrueString);
                                    restRequest.AddJsonBody(listOfUserId);

                                    var restResult = await _restClient.ExecuteAsync(restRequest);

                                    if (restResult.IsSuccessful && restResult.StatusCode == HttpStatusCode.OK)
                                        resultList.Add(new ResultViewModel
                                        {
                                            Id = userId,
                                            Validate = 1,
                                            Message =
                                            $"User {userId} transferred to device {deviceToDelete.Code} successfully"
                                        });
                                }
                            });
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, "Add User To Devices");
                            resultList.Add(new ResultViewModel
                            {
                                Id = userId,
                                Validate = 1,
                                Message = $"Error on transferring user {userId} to device"
                            });
                        }
                    }
                }

                return resultList;
            }
            catch (Exception exception)
            {
                Logger.Log(exception, "Error on Get User Group Member");
                return new List<ResultViewModel> { new ResultViewModel { Id = 0, Validate = 0, Message = "Cannot update user groups of users" } };
            }
        }

        [HttpGet]
        [Route("UpdateUserGroupsOfUser")]
        public ResultViewModel UpdateUserGroupsOfUser(long userId, string userGroupIds)
        {
            try
            {
                // for rolling back on problem occuring
                //var userGroupsOfUser = _userGroupService.GetUserGroupsOfUser(userId);

                var userGroupIdList = JsonConvert.DeserializeObject<List<int>>(userGroupIds);

                var result = _userService.DeleteUserGroupsOfUser((int)userId);
                if (result.Validate != 1) return new ResultViewModel { Id = userId, Validate = 0, Message = $"Cannot update user groups of user {userId}" };

                foreach (var userGroupId in userGroupIdList)
                {
                    _userGroupService.AddUserGroupMember(new UserGroupMember
                    {
                        UserId = userId,
                        GroupId = userGroupId,
                        UserType = 1.ToString(),
                        UserTypeTitle = string.Empty
                    });
                }

                return new ResultViewModel { Id = userId, Validate = 1, Message = $"User groups of user {userId} updated successfully" };
            }
            catch (Exception exception)
            {
                Logger.Log(exception, "Error on Get User Group Member");
                return new ResultViewModel { Id = userId, Validate = 0, Message = $"Cannot update user groups of user {userId}" };
            }
        }

        [HttpPost]
        [Route("SendUsersDataToDevice")]
        public Task<List<ResultViewModel>> SendUsersDataToDevice([FromBody] List<int> userIds, int deviceId = default)
        {
            return Task.Run(() =>
            {
                var results = new List<ResultViewModel>();

                var tasks = new List<Task>();
                if (deviceId == default)
                {
                    var deviceBrands = _deviceService.GetDeviceBrands();
                    foreach (var deviceBrand in deviceBrands)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            var restRequest =
                                new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}User/SendUsersDataToDevice",
                                    Method.POST);
                            if (deviceId != default)
                                restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                            if (userIds != null)
                                restRequest.AddJsonBody(userIds);

                            var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                            lock (results)
                            {
                                if (result.StatusCode == HttpStatusCode.OK && result.Data != null)
                                    results.Add(new ResultViewModel
                                    {
                                        Id = result.Data.Id,
                                        Code = result.Data.Code,
                                        Validate = result.IsSuccessful && result.Data.Validate == 1 ? 1 : 0,
                                        Message = deviceBrand.Name
                                    });
                            }
                        }));
                    }
                }
                else
                {
                    var device = _deviceService.GetDeviceInfo(deviceId);
                    tasks.Add(Task.Run(async () =>
                    {
                        var restRequest =
                            new RestRequest($"{device.Brand.Name}/{device.Brand.Name}User/SendUsersDataToDevice",
                                Method.POST);
                        if (deviceId != default)
                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                        if (userIds != null)
                            restRequest.AddJsonBody(userIds);

                        var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                        lock (results)
                        {
                            if (result.StatusCode == HttpStatusCode.OK && result.Data != null)
                                results.Add(new ResultViewModel
                                {
                                    Id = result.Data.Id,
                                    Code = result.Data.Code,
                                    Validate = result.IsSuccessful && result.Data.Validate == 1 ? 1 : 0,
                                    Message = device.Brand.Name
                                });
                        }
                    }));
                }

                Task.WaitAll(tasks.ToArray());
                return results;
            });
        }
    }

    public class ParamViewModel
    {
        public string UpdateUsers { get; set; }
        public string ChangeUsers { get; set; }

        public string GetJson()
        {
            var model = new ParamViewModel
            {
                UpdateUsers = "<Root><UserGroupMember></UserGroupMember><UserGroupMember></UserGroupMember></Root>",
                ChangeUsers = "941364,951444,961515"
            };
            return JsonConvert.SerializeObject(model);
        }
    }
}
