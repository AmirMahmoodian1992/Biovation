using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using MoreLinq.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiVersion("2.0")]
    [Route("biovation/api/v2/[controller]")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly UserCardService _userCardService;
        private readonly UserGroupService _userGroupService;
        private readonly AccessGroupService _accessGroupService;
        private readonly FingerTemplateService _fingerTemplateService;

        private readonly RestClient _restClient;

        public UserController(UserService userService, DeviceService deviceService, UserGroupService userGroupService, AccessGroupService accessGroupService, RestClient restClient, FingerTemplateService fingerTemplateService, UserCardService userCardService)
        {
            _userService = userService;
            _deviceService = deviceService;
            _userGroupService = userGroupService;
            _restClient = restClient;
            _fingerTemplateService = fingerTemplateService;
            _userCardService = userCardService;
            _accessGroupService = accessGroupService;
        }

        [HttpGet]
        //[Authorize(Policy = Policies.User)]
        //[Authorize(Policy = "OverrideTest")]
        public Task<ResultViewModel<PagingResult<User>>> GetUsersByFilter(int from = default,
            int size = default, bool getTemplatesData = default, long userId = default, long code = default, string filterText = default,
            int type = default, bool withPicture = default, bool isAdmin = default, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _userService.GetUsers(from, size, getTemplatesData, userId, code, filterText, type,
               withPicture, isAdmin, pageNumber, pageSize, HttpContext.Items["Token"].ToString()));
        }


        [HttpGet]
        [Route("{id:long}")]
        public Task<ResultViewModel<PagingResult<User>>> GetUsersByFilter([FromRoute] long id)
        {
            return Task.Run(() => _userService.GetUsers(userId: id, token: HttpContext.Items["Token"].ToString()));
        }
        //[HttpPost]
        //public Task<IActionResult> AddUser([FromBody] User user)
        //{
        //}

        [HttpPut]
        public Task<ResultViewModel> ModifyUser([FromBody] User user)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () =>
            {
                try
                {
                    var existingUser = _userService.GetUsers(userId: user.Id, token: token).Data.Data.FirstOrDefault();

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
                        user.IdentityCard ??= existingUser.IdentityCard;
                        user.Image = user.Image is null || user.Image.Length < 1 ? existingUser.Image : user.Image;
                        user.Type = user.Type == default ? existingUser.Type : user.Type;
                        user.TelNumber = string.IsNullOrWhiteSpace(user.TelNumber)
                            ? existingUser.TelNumber
                            : user.TelNumber;
                    }

                    var result = _userService.ModifyUser(user, token);

                    await Task.Run(() =>
                    {
                        var deviceBrands = _deviceService.GetDeviceBrands(token: token)?.Data?.Data;
                        if (deviceBrands is null)
                            return;

                        foreach (var restRequest in deviceBrands.Select(deviceBrand => new RestRequest($"/{deviceBrand.Name}/{deviceBrand.Name}User/ModifyUser", Method.POST)))
                        {
                            restRequest.AddJsonBody(user);
                            if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                            {
                                restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                            }
                            _restClient.ExecuteAsync<ResultViewModel>(restRequest);
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

        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteUser([FromRoute] int id = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _userService.DeleteUser(id, token));
        }

        [HttpPost]
        [Route("DeleteUsers")]
        public Task<ResultViewModel> DeleteUsers([FromBody] List<int> ids = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _userService.DeleteUsers(ids, token));
        }

        [HttpGet]
        [Route("{id}/FingerTemplates")]
        public Task<ResultViewModel<PagingResult<FingerTemplate>>> GetFingerTemplateByUserId([FromRoute] int id = default, string fingerTemplateType = default, int templateIndex = default, int pageNumber = default, int pageSize = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _fingerTemplateService.FingerTemplates(id, templateIndex, fingerTemplateType, pageNumber, pageSize, token: token));
        }

        [HttpPost]
        [Route("{id}/FingerTemplate")]
        public Task<IActionResult> AddUserFingerTemplate([FromRoute] int id, [FromBody] FingerTemplate fingerTemplate = default)
        {
            throw new NotImplementedException();
        }

        [HttpPatch]
        [Route("{id}/FingerTemplate")]
        public Task<ResultViewModel> ModifyUserFingerTemplate([FromRoute] int id, [FromBody] FingerTemplate fingerTemplate = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _fingerTemplateService.ModifyFingerTemplate(fingerTemplate, token));
        }

        [HttpDelete]
        [Route("{id}/FingerTemplates")]
        public Task<ResultViewModel> DeleteFingerTemplateByUserId([FromRoute] int id = default, int templateIndex = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _fingerTemplateService.DeleteFingerTemplate(id, templateIndex, token));
        }

        [HttpGet]
        [Route("{id}/Card")]
        public Task<ResultViewModel<PagingResult<UserCard>>> GetUserCard(long id, bool isActive,
            int pageNumber = default, int pageSize = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _userCardService.GetCardsByFilter(id, isActive, pageNumber, pageSize, token));
        }

        ////batch delete
        //[HttpPost]
        //[Route("/DeleteUser")]
        //public Task<IActionResult> DeleteUser([FromBody]List<int> ids = default)
        //{
        //    throw null;
        //}


        //if deviceId == 0 then send ids to all of device
        [HttpPut]
        [Route("SendUsersToDevices")]
        public Task<ResultViewModel> SendUsersToDevice([FromBody] int[] ids, [FromBody] int[] deviceIds = default)
        {
            var token = (string)HttpContext.Items["Token"];
            try
            {
                if (!ids.Any())
                {
                    return Task.Run(() => new ResultViewModel { Validate = 0, Message = "User is empty" });
                }

                var result = new List<ResultViewModel>();
                if (deviceIds == null)
                    return Task.Run(() =>
                        result.Any(e => e.Success == false)
                            ? new ResultViewModel { Validate = 0, Message = "" }
                            : new ResultViewModel { Validate = 1, Message = "Failed to send all of them" });
                foreach (var device in deviceIds)
                {
                    foreach (var id in ids)
                    {
                        var deviceBasic = _deviceService.GetDevice(device, token: token).Data;
                        if (deviceBasic == null)
                        {
                            var msg = "DeviceId " + device + " does not exist.";
                            Logger.Log(msg);
                            return Task.Run(() => new ResultViewModel { Validate = 0, Message = msg });
                        }

                        var restRequest =
                            new RestRequest(
                                $"/biovation/api/{deviceBasic.Brand.Name}/{deviceBasic.Brand.Name}User/SendUserToDevice",
                                Method.GET);
                        restRequest.AddParameter("code", deviceBasic.Code);
                        restRequest.AddParameter("userId", id);
                        if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                        {
                            restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                        }
                        result.AddRange(_restClient.ExecuteAsync<List<ResultViewModel>>(restRequest).Result.Data);
                    }
                }

                return Task.Run(() => result.Any(e => e.Success == false) ? new ResultViewModel { Validate = 0, Message = "" } : new ResultViewModel { Validate = 1, Message = "Failed to send all of them" });
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return Task.Run(() => new ResultViewModel { Validate = 0, Message = "SendUserToDevice Failed." });
            }
        }


        [HttpPatch]
        [Route("{id}/Password")]
        public Task<ResultViewModel> ModifyPassword([FromRoute] int id = default, string password = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _userService.ModifyPassword(id, password, token));
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
        public Task<ResultViewModel> SyncDeviceAndAccessGroup([FromBody] ParamViewModel param)
        {
            return Task.Run(async () =>
            {
                var changeUsers = param.ChangeUsers;
                var updateUsers = param.UpdateUsers;
                if (string.IsNullOrEmpty(changeUsers))
                {
                    return new ResultViewModel { Success = false };
                }

                changeUsers = changeUsers.Trim(',');
                var usersToSync = changeUsers.Split(',').Select(s => Convert.ToInt64(s)).ToArray();
                var result = await Sync(usersToSync.Distinct().ToArray(), updateUsers);
                return result;
            });
        }


        private Task<ResultViewModel> Sync(long[] usersToSync = default, string updateUsers = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() =>
            {
                var deviceBrands = _deviceService.GetDeviceBrands(token: token)?.Data?.Data;
                if (deviceBrands is null)
                    return new ResultViewModel { Success = false, Validate = 0, Code = 400, Message = "Could not load device brands" };

                try
                {
                    foreach (var deviceBrand in deviceBrands)
                    {
                        //_communicationManager.CallRest($"/biovation/api/{brand.Name}/{brand.Name}User/DeleteUserFromAllTerminal", "Post", null, $"{JsonConvert.SerializeObject(lstchangeUsers)}");
                        var restRequest =
                            new RestRequest($"/{deviceBrand.Name}/{deviceBrand.Name}User/DeleteUserFromAllTerminal", Method.POST);
                        restRequest.AddJsonBody(usersToSync ?? Array.Empty<long>());
                        if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                        {
                            restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                        }
                        _restClient.ExecuteAsync(restRequest);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, "Delete User From Device");
                    return new ResultViewModel { Success = false };
                }

                if (updateUsers != null && string.IsNullOrEmpty(updateUsers.Replace("<Root/>", "")))
                {
                    return new ResultViewModel { Success = true };
                }

                //var xmlDoc = new XmlDocument();
                //xmlDoc.LoadXml(updateUsers);
                //var fromXml = JsonConvert.SerializeXmlNode(xmlDoc);
                //var fromJson = JsonConvert.DeserializeObject<List<User>>(fromXml);
                List<UserGroupMember> lstUserGroupMember;
                try
                {
                    var serializer = new XmlSerializer(typeof(List<UserGroupMember>), new XmlRootAttribute("Root"));
                    var stringReader = new StringReader(updateUsers ?? string.Empty);
                    lstUserGroupMember = (List<UserGroupMember>)serializer.Deserialize(stringReader);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, "Error on serialize users");
                    return new ResultViewModel { Success = false };
                }

                var resultUpdate = UpdateUserGroupMember(usersToSync, lstUserGroupMember);
                if (!resultUpdate.Result.Success)
                {
                    return new ResultViewModel { Success = false };
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
                            var accessGroups = _accessGroupService.GetAccessGroups(lstUserGroupMember[i].UserId, token: token).Data.Data;
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
                                        if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                                        {
                                            restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                                        }
                                        _restClient.ExecuteAsync(restRequest);
                                    }
                                }
                            }
                        }
                    });

                    return new ResultViewModel { Success = true };

                }
                catch (Exception exception)
                {
                    Logger.Log(exception, "Add User To Devices");
                    return new ResultViewModel { Success = false };
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
        //                        Logger.Log($"Not a standard access group, [{accessGroup.Id?}].\n", "The access group does not have any device group.");
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
        //                            var parameters = new List<object> { $"code={device.Code}", $"userId=[{lstUserGroupMember[i].UserId?}]", };
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
        [Route("{id}/EnrollFaceTemplate")]
        public Task<ResultViewModel> EnrollFaceTemplate(int id = default, int deviceId = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () =>
            {
                var user = _userService.GetUsers(userId: id, token: token);
                if (user is null)
                    return new ResultViewModel { Validate = 0, Id = id, Message = "Wrong user id is provided." };

                var device = _deviceService.GetDevice(deviceId, token: token).Data;
                if (device is null)
                    return new ResultViewModel { Validate = 0, Id = deviceId, Message = "Wrong device id is provided." };

                var restRequest = new RestRequest($@"{device.Brand.Name}/{device.Brand.Name}User/EnrollFaceTemplate", Method.POST);
                restRequest.AddQueryParameter("userId", id.ToString());
                restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                {
                    restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                }
                var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                return result.StatusCode == HttpStatusCode.OK ? result.Data : new ResultViewModel { Validate = 0, Id = (long)result.StatusCode, Message = result.ErrorMessage };
            });
        }

        [HttpPatch]
        [Route("UserGroupsOfUsers")]
        public Task<List<ResultViewModel>> UpdateUserGroupsOfUser([FromBody] string usersGroupIds = default, bool sendUsersToDevice = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() =>
            {
                try
                {
                    var resultList = new List<ResultViewModel>();
                    //var userIdList = JsonConvert.DeserializeObject<List<int>>(userIds);
                    var userGroupIdList = JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(usersGroupIds ?? string.Empty);

                    var deviceBrands = _deviceService.GetDeviceBrands(token: token)?.Data?.Data;

                    foreach (var userId in userGroupIdList.Keys)
                    {
                        if (!userGroupIdList.ContainsKey(userId))
                            continue;

                        // for rolling back on problem occuring
                        var userExistingDevices = new List<DeviceBasicInfo>();
                        var userGroupsOfUser = _userGroupService.UserGroups(token: token)?.Data?.Data;
                        foreach (var userGroup in userGroupsOfUser)
                        {
                            var accessGroups = _accessGroupService.GetAccessGroups(userGroupId: userGroup.Id, token: token).Data.Data;
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

                        var result = _userService.DeleteUserGroupsOfUser(userId, token: token);
                        if (result.Validate != 1)
                        {
                            resultList.Add(new ResultViewModel
                            { Id = userId, Validate = 0, Message = $"Cannot update user groups of user {userId}" });
                            Logger.Log($"Cannot update user groups of user {userId}");

                            foreach (var userGroup in userGroupsOfUser)
                            {
                                try
                                {
                                    var userGroupMember =
                                        userGroup.Users.FirstOrDefault(userGroupMem => userGroupMem.UserId == userId);
                                    _userGroupService.AddUserGroup(userGroupMember, token);
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
                            _userGroupService.AddUserGroup(new UserGroupMember
                            {
                                UserId = userId,
                                GroupId = userGroupId,
                                UserType = 1.ToString(),
                                UserTypeTitle = string.Empty
                            }, token);
                        }

                        Logger.Log($"User groups of user {userId} updated successfully");
                        resultList.Add(new ResultViewModel
                        {
                            Id = userId,
                            Validate = 1,
                            Message = $"User groups of user {userId} updated successfully"
                        });


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
                                    //TODO ignore int nestingDepthLevel in getAccessGroups, is it correct?
                                    var accessGroups = _accessGroupService.GetAccessGroups(userId, token: token).Data.Data;
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
                                        userExistingDevices.ExceptBy(devicesToExistsOn, device => device.DeviceId)
                                            .ToList();

                                    var devicesToAdd =
                                        devicesToExistsOn.ExceptBy(userExistingDevices, device => device.DeviceId)
                                            .ToList();

                                    foreach (var device in devicesToAdd)
                                    {
                                        if (deviceBrands is null)
                                            continue;

                                        var deviceBrand = deviceBrands.First(devBrand =>
                                            devBrand.Code == device.Brand.Code);
                                        var restRequest = new RestRequest(
                                            $"/{deviceBrand.Name}/{deviceBrand.Name}User/SendUserToDevice", Method.GET);
                                        restRequest.AddQueryParameter("code", device.Code.ToString());
                                        restRequest.AddQueryParameter("userId", $"[{userId}]");
                                        restRequest.AddQueryParameter("updateServerSideIdentification",
                                            bool.TrueString);
                                        if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                                        {
                                            restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                                        }
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
                                        if (deviceBrands is null)
                                            continue;

                                        var deviceBrand = deviceBrands.First(devBrand =>
                                            devBrand.Code == deviceToDelete.Brand.Code);
                                        var listOfUserId = new List<int> { userId };
                                        var restRequest = new RestRequest(
                                            $"/{deviceBrand.Name}/{deviceBrand.Name}Device/DeleteUserFromDevice",
                                            Method.POST);
                                        restRequest.AddQueryParameter("code", deviceToDelete.Code.ToString());
                                        restRequest.AddQueryParameter("updateServerSideIdentification",
                                            bool.TrueString);
                                        restRequest.AddJsonBody(listOfUserId);
                                        if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                                        {
                                            restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                                        }

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
                    return new List<ResultViewModel>
                        {new ResultViewModel {Id = 0, Validate = 0, Message = "Cannot update user groups of users"}};
                }
            });
        }

        [HttpPatch]
        [Route("UpdateUserGroupsMember")]
        private Task<ResultViewModel> UpdateUserGroupMember(long[] userIds, [FromBody] List<UserGroupMember> lstToAdd)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() =>
            {
                try
                {
                    var count = userIds.Length;
                    var groupIds = new List<int>();
                    for (var i = 0; i < count; i++)
                    {
                        var group = _userGroupService.UserGroups(token: token)?.Data?.Data;
                        groupIds.AddRange(group.Select(s => s.Id));
                    }

                    if (groupIds.Any())
                    {
                        var grpIds = string.Join(",", groupIds.Distinct());

                        var restRequest = new RestRequest("/UserGroupMember/GetUserGroupMemberDetail", Method.GET);
                        restRequest.AddQueryParameter("userGroupId", grpIds);
                        if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                        {
                            restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                        }
                        var member = _restClient.Execute<List<UserGroupMember>>(restRequest);

                        var grpMember = member.Data.GroupBy(g => g.GroupId).ToList();
                        foreach (var members in grpMember)
                        {
                            var strWp = JsonConvert.SerializeObject(members);
                            var wrappedDocument = $"{{ UserGroupMember: {strWp} }}";
                            var xDocument = JsonConvert.DeserializeXmlNode(wrappedDocument, "Root");
                            var node = xDocument?.OuterXml;

                            //_userGroupService.ModifyUserGroupMember(node, members.Key);
                        }
                    }

                    foreach (var userMember in lstToAdd)
                    {
                        _userGroupService.AddUserGroup(userMember, token);
                    }

                    return new ResultViewModel { Success = true };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, "Error on Get User Group Member");
                    return new ResultViewModel { Success = true };
                }
            });
        }


        [HttpDelete]
        [Route("{id}/UserFromDevice/{deviceId}")]
        [Authorize]
        public Task<ResultViewModel> RemoveUserFromDevice([FromRoute] int id = default, [FromRoute] int deviceId = default)
        {
            return Task.Run(() =>
            {
                var restRequest = new RestRequest("Biovation/api/v2/Device/{id}/UserFromDevice/{userId}", Method.DELETE);
                restRequest.AddUrlSegment("id", deviceId);
                restRequest.AddUrlSegment("deviceId", id);
                if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                {
                    restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                }
                return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
            });
        }

        [HttpPost]
        [Route("UserFromDevice/{deviceId}")]
        [Authorize]
        public Task<ResultViewModel> RetrieveUserDevice([FromRoute] int deviceId = default, [FromBody] JArray userId = default)
        {

            return Task.Run(async () =>
            {
                var restRequest = new RestRequest("Biovation/api/v2/Device/{id}/UserFromDevice", Method.POST);
                restRequest.AddUrlSegment("id", deviceId);
                restRequest.AddJsonBody(userId);
                if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                {
                    restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                }
                return (await _restClient.ExecuteAsync<ResultViewModel>(restRequest)).Data;
            });
        }

        [HttpPost]
        [Route("UsersListFromDevice/{deviceId}")]
        public Task<List<User>> RetrieveUsersOfDevice([FromRoute] int deviceId = default)
        {
            return Task.Run(async () =>
            {
                var restRequest = new RestRequest("Biovation/api/v2/Device/{id}/UsersListFromDevice", Method.POST);
                restRequest.AddUrlSegment("id", deviceId);
                if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                {
                    restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                }
                return (await _restClient.ExecuteAsync<List<User>>(restRequest)).Data;
            });
        }

        [HttpPost]
        [Route("UserToDevice/{id}")]
        [Authorize]
        public Task<ResultViewModel> SendUserToDevice(int id = default)
        {
            return Task.Run(async () =>
            {
                var restRequest = new RestRequest("Biovation/api/v2/Device/UserToDevice/{id}", Method.POST);
                restRequest.AddUrlSegment("id", id);
                if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                {
                    restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                }
                return (await _restClient.ExecuteAsync<ResultViewModel>(restRequest)).Data;
            });
        }

        [HttpPost]
        [Route("UserToAllDevice")]
        [Authorize]
        public Task<List<ResultViewModel>> SendUsersToAllDevice([FromBody] string ids = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () =>
            {
                try
                {
                    var userIds = JsonConvert.DeserializeObject<int[]>(ids);
                    var deviceBrands = _deviceService.GetDeviceBrands(token: token)?.Data?.Data;
                    var length = userIds.Length;
                    var result = new List<ResultViewModel>();
                    for (var i = 0; i < length; i++)
                    {
                        var user = _userService.GetUsers(userId: userIds[i], token: token).Data.Data.FirstOrDefault();
                        if (user == null)
                        {
                            Logger.Log($"User {userIds[i]} not exists.");
                            result.Add(new ResultViewModel
                            { Validate = 0, Message = $"User {userIds[i]} not exists.", Id = userIds[i] });
                        }

                        if (deviceBrands == null) continue;
                        foreach (var deviceBrand in deviceBrands)
                        {
                            //var restResult = _communicationManager.CallRest(
                            //            $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}User/SendUserToAllDevices", "Post", null, $"{JsonConvert.SerializeObject(user)}");

                            var restRequest =
                                new RestRequest($"/{deviceBrand.Name}/{deviceBrand.Name}User/SendUserToAllDevices",
                                    Method.POST);
                            restRequest.AddJsonBody(user);

                            var restResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                            result.Add(new ResultViewModel
                            {
                                Validate = restResult.Data?.Validate ?? 0,
                                Id = userIds[i],
                                Message = deviceBrand.Name
                            });
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
