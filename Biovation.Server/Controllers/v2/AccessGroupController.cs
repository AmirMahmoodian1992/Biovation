﻿using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
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
    public class AccessGroupController : ControllerBase
    {
        private readonly RestClient _restClient;
        private readonly AccessGroupService _accessGroupService;
        private readonly DeviceService _deviceService;
        private readonly UserGroupService _userGroupService;
        private readonly DeviceGroupService _deviceGroupService;

        public AccessGroupController(RestClient restClient, AccessGroupService accessGroupService, DeviceService deviceService, UserGroupService userGroupService, DeviceGroupService deviceGroupService)
        {
            _restClient = restClient;
            _accessGroupService = accessGroupService;
            _deviceService = deviceService;
            _userGroupService = userGroupService;
            _deviceGroupService = deviceGroupService;
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ResultViewModel<AccessGroup>> AccessGroup([FromRoute] int id, int nestingDepthLevel = 5)
        {
            return await _accessGroupService.GetAccessGroup(id, nestingDepthLevel, HttpContext.Items["Token"] as string);
        }

        [HttpGet]
        public async Task<ResultViewModel<PagingResult<AccessGroup>>> AccessGroups(long userId = default, int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default, int timeZoneId = default, int pageNumber = default, int pageSize = default, int nestingDepthLevel = 5)
        {
            return await _accessGroupService.GetAccessGroups(userId, userGroupId, id, deviceId, deviceGroupId,
                timeZoneId, pageNumber, pageSize, nestingDepthLevel, token: HttpContext.Items["Token"] as string);
        }

        [HttpPost]
        public async Task<ResultViewModel> AddAccessGroup([FromBody] AccessGroup accessGroup)
        {
            return await _accessGroupService.AddAccessGroup(accessGroup, HttpContext.Items["Token"] as string);
        }

        [HttpPatch]
        public async Task<ResultViewModel> ModifyAccessGroup(string accessGroup = default, string deviceGroup = default, string userGroup = default, string adminUserIds = default)
        {
            var token = HttpContext.Items["Token"] as string;
            var xmlDevice = $"{{ DeviceGroup: {deviceGroup} }}";
            var xmlUser = $"{{ UserGroup: {userGroup} }}";
            var xmlAdmin = $"{{AdminUsers: {adminUserIds} }}";

            var xmlDevices = JsonConvert.DeserializeXmlNode(xmlDevice, "Root");
            var xmlUsers = JsonConvert.DeserializeXmlNode(xmlUser, "Root");
            var xmlAdmins = JsonConvert.DeserializeXmlNode(xmlAdmin, "Root");


            var saved = await _accessGroupService.ModifyAccessGroup(JsonConvert.DeserializeObject<AccessGroup>(accessGroup ?? string.Empty));
            ResultViewModel result;

            if (saved.Validate != 1)
                result = new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };
            else
            {

                var deviceResult = await _accessGroupService.ModifyAccessGroupDeviceGroup(xmlDevices?.OuterXml, (int)saved.Id, token);
                if (deviceResult.Validate != 1)
                    result = new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };
                else
                {

                    var adminUsersResult = await _accessGroupService.ModifyAccessGroupAdminUsers(xmlAdmins?.OuterXml, (int)saved.Id, token);
                    if (adminUsersResult.Validate != 1)
                        result = new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };
                    else
                    {
                        var userGroupResult = await _accessGroupService.ModifyAccessGroupUserGroup(xmlUsers?.OuterXml, (int)saved.Id, token);

                        result = userGroupResult;
                    }
                }
            }

            /*
              var restRequest = new RestRequest($"{deviceBrand.Name}/{device.Brand?.Name}AccessGroup/ModifyAccessGroup");

                         restRequest.AddQueryParameter("DeviceGroup",deviceGroup);
                        restRequest.AddQueryParameter(" UserGroup", userGroup);
                        restRequest.AddQueryParameter("AdminUsers", adminUserIds); 

            */

            _ = Task.Run(async () =>
            {
                //var restRequest = new RestRequest("Queries/v2/Device/DeviceBrands", Method.GET);
                //restRequest.AddHeader("Authorization", token!);

                //var deviceBrands = (_restClient.ExecuteAsync<PagingResult<Lookup>>(restRequest)).Result.Data.Data;
                var deviceBrands = (await _deviceService.GetDeviceBrands())?.Data?.Data;

                if (deviceBrands != null)
                    foreach (var restRequest in deviceBrands.Select(deviceBrand => new RestRequest(
                        $"{deviceBrand.Name}/{deviceBrand.Name}AccessGroup/ModifyAccessGroup",
                        Method.POST)))
                    {
                        restRequest.AddHeader("Authorization", token!);
                        await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                    }
            }).ConfigureAwait(false);

            return result;
        }

        [HttpGet]
        [Route("{id}/UserGroups")]
        public async Task<ResultViewModel<List<UserGroup>>> GetAccessControlUserGroup([FromRoute] int id = default)
        {
            return await _userGroupService.GetAccessControlUserGroup(id, HttpContext.Items["Token"] as string);
        }

        [HttpGet]
        [Route("{id}/DeviceGroups")]
        public async Task<ResultViewModel<PagingResult<DeviceGroup>>> GetAccessControlDeviceGroup([FromRoute] int id = default, int pageNumber = default, int pageSize = default)
        {
            return await _deviceGroupService.GetAccessControlDeviceGroup(id, pageNumber, pageSize, HttpContext.Items["Token"] as string);
        }

        [HttpGet]
        [Route("{id}/AdminUsers")]
        public async Task<ResultViewModel<List<User>>> GetAdminUserOfAccessGroup([FromRoute] long id = default, int accessGroupId = default)
        {
            return await _accessGroupService.GetAdminUserOfAccessGroup(id, accessGroupId, HttpContext.Items["Token"] as string);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ResultViewModel> DeleteAccessGroups([FromRoute] int id = default)
        {
            return await _accessGroupService.DeleteAccessGroup(id, HttpContext.Items["Token"] as string);
        }

        [HttpPost]
        [Route("{id}/SendAllUsersToAllDevicesInAccessGroup")]
        public async Task<ResultViewModel> SendAllUsersToAllDevicesInAccessGroup([FromRoute] int id = default)
        {
            try
            {
                var token = HttpContext.Items["Token"] as string;
                var deviceBrands = (await _deviceService.GetDeviceBrands())?.Data?.Data;
                var accessGroup = (await _accessGroupService.GetAccessGroup(id, token: token)).Data;
                if (accessGroup == null)
                {
                    Logger.Log("No such access group found.\n");
                    return new ResultViewModel { Validate = 0 };
                }

                if (accessGroup.UserGroup == null || accessGroup.DeviceGroup == null)
                {
                    Logger.Log("Not a standard access group.\n");
                    return new ResultViewModel { Validate = 0 };
                }

                foreach (var deviceGroup in accessGroup.DeviceGroup)
                {
                    if (deviceGroup.Devices == null)
                    {
                        //Logger.Log("No device to send users on.\n");
                        //return new ResultViewModel { Validate = 1 };
                        continue;
                    }

                    foreach (var device in deviceGroup.Devices)
                    {
                        var deviceBrand = deviceBrands?.FirstOrDefault(devBrand => devBrand.Code == device.Brand.Code);
                        //var parameters = new List<object> { $"accessGroupId={accessGroupId}", $"code={device.Code}" };
                        //_communicationManager.CallRest(
                        //    $"/biovation/api/{deviceBrand?.Name}/{deviceBrand?.Name}AccessGroup/SendAccessGroupToDevice", "Get", parameters, null);
                        var restRequest =
                            new RestRequest(
                                $"{deviceBrand?.Name}/{deviceBrand?.Name}AccessGroup/SendAccessGroupToDevice",
                                Method.GET);
                        restRequest.AddParameter("code", device.Code);
                        restRequest.AddParameter("accessGroupId", id);
                        restRequest.AddHeader("Authorization", token!);
                        await _restClient.ExecuteAsync<ResultViewModel>(restRequest).ConfigureAwait(false);

                        foreach (var userGroup in accessGroup.UserGroup)
                        {
                            if (userGroup.Users == null)
                            {
                                //    Logger.Log("No user to send on devices.\n");
                                //    return new ResultViewModel { Validate = 1 };
                                continue;
                            }

                            //foreach (var user in userGroup.Users)
                            //{
                            //var userids = string.Join(",", userGroup.Users.Select(s => s.Id).ToArray());
                            var userids = JsonConvert.SerializeObject(userGroup.Users.Select(s => s.UserId).ToArray());
                            //parameters = new List<object> { $"code={device.Code}", $"userId={userids}", };
                            //_communicationManager.CallRest(
                            //    $"/biovation/api/{deviceBrand?.Name}/{deviceBrand?.Name}User/SendUserToDevice", "Get", parameters, null);
                            restRequest =
                                new RestRequest(
                                    $"{deviceBrand?.Name}/{deviceBrand?.Name}User/SendUserToDevice",
                                    Method.GET);
                            restRequest.AddParameter("code", device.Code);
                            restRequest.AddParameter("userId", userids);
                            restRequest.AddHeader("Authorization", token!);
                            await _restClient.ExecuteAsync<ResultViewModel>(restRequest).ConfigureAwait(false);
                            //}
                        }
                    }
                }

                return new ResultViewModel { Validate = 1 };
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return new ResultViewModel { Validate = 0, Message = "SendAllUsersToAllDevicesInAccessGroup Failed." };
            }
        }


        [HttpPost]
        [Route("{id}/SendAccessGroupToDevices")]
        public async Task<ResultViewModel> SendAccessGroupToDevices([FromRoute] int id)
        {
            var token = HttpContext.Items["Token"] as string;
            var devices = (await _accessGroupService.GetDeviceOfAccessGroup(id, token: token)).Data.Data;

            foreach (var device in devices)
            {
                var restRequest =
                    new RestRequest(
                        $"{device.Brand.Name}/{device.Brand.Name}AccessGroup/SendAccessGroupToDevice",
                        Method.GET);
                restRequest.AddParameter("code", device.Code);
                restRequest.AddParameter("accessGroupId", id);
                restRequest.AddHeader("Authorization", token!);
                await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            }
            return new ResultViewModel { Validate = 1 };
        }

        [HttpPost]
        [Route("{id}/SendAccessGroupToDevice/{deviceId}")]
        public async Task<ResultViewModel> SendAccessGroupToDevice([FromRoute] int id, [FromRoute] int deviceId)
        {
            var token = HttpContext.Items["Token"] as string;

            var device = (await _deviceService.GetDevice(deviceId, token))?.Data;
            if (device is null)
            {
                return new ResultViewModel
                {
                    Success = false,
                    Validate = 0,
                    Code = 404,
                    Id = deviceId,
                    Message = "Provided device Id is wrong."
                };
            }

            var restRequest =
                new RestRequest(
                    $"{device.Brand.Name}/{device.Brand.Name}AccessGroup/SendAccessGroupToDevice",
                    Method.GET);
            restRequest.AddParameter("code", device.Code);
            restRequest.AddParameter("accessGroupId", id);
            restRequest.AddHeader("Authorization", token!);
            await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return new ResultViewModel { Validate = 1 };
        }
    }
}