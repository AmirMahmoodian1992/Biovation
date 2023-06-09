﻿using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class AccessGroupController : ControllerBase
    {
        //private readonly CommunicationManager<ResultViewModel> _communicationManager = new CommunicationManager<ResultViewModel>();
        private readonly RestClient _restClient;
        private readonly AccessGroupService _accessGroupService;
        private readonly DeviceService _deviceService;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        private readonly string _kasraAdminToken;

        public AccessGroupController(RestClient restClient, AccessGroupService accessGroupService, DeviceService deviceService, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _accessGroupService = accessGroupService;
            _deviceService = deviceService;
            _biovationConfigurationManager = biovationConfigurationManager;
            _kasraAdminToken = _biovationConfigurationManager.KasraAdminToken;
        }

        [HttpGet, Route("AccessGroups")]
        public List<AccessGroup> AccessGroups(long userId = 0)
        {
            return _accessGroupService.GetAccessGroups(adminUserId: (int)userId, token: _kasraAdminToken);
        }

        [HttpGet]
        [Route("AccessGroup")]
        public AccessGroup AccessGroup(int id)
        {
            return _accessGroupService.GetAccessGroup(id, token: _kasraAdminToken);
        }

        [HttpGet, Route("GetAccessGroupsByFilter")]
        public List<AccessGroup> AccessGroupsByFilter(int id = default, int deviceGroupId = default, int userId = default, int adminUserId = default, int userGroupId = default, int deviceId = default)
        {
            return _accessGroupService.GetAccessGroups(id: id,
                deviceGroupId: deviceGroupId, adminUserId: userId == default ? adminUserId : userId, deviceId: deviceId, userGroupId: userGroupId, token: _kasraAdminToken);
        }

        [HttpPost]
        [Route("ModifyAccessGroup")]
        public ResultViewModel ModifyAccessGroup(string accessGroup, string deviceGroup, string userGroup, string adminUserIds)
        {
            var xmlDevice = $"{{ DeviceGroup: {deviceGroup} }}";
            var xmlUser = $"{{ UserGroup: {userGroup} }}";
            var xmlAdmin = $"{{AdminUsers: {adminUserIds} }}";

            var xmlDevices = JsonConvert.DeserializeXmlNode(xmlDevice, "Root");
            var xmlUsers = JsonConvert.DeserializeXmlNode(xmlUser, "Root");
            var xmlAdmins = JsonConvert.DeserializeXmlNode(xmlAdmin, "Root");


            var saved = _accessGroupService.ModifyAccessGroup(JsonConvert.DeserializeObject<AccessGroup>(accessGroup), token: _kasraAdminToken);
            ResultViewModel result;

            if (saved.Validate != 1)
                result = new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };
            else
            {

                var deviceResult = _accessGroupService.ModifyAccessGroupDeviceGroup(xmlDevices?.OuterXml, (int)saved.Id, token: _kasraAdminToken);
                if (deviceResult.Validate != 1)
                    result = new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };
                else
                {

                    var adminUsersResult = _accessGroupService.ModifyAccessGroupAdminUsers(xmlAdmins?.OuterXml, (int)saved.Id, token: _kasraAdminToken);
                    if (adminUsersResult.Validate != 1)
                        result = new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };
                    else
                    {
                        var userGroupResult = _accessGroupService.ModifyAccessGroupUserGroup(xmlUsers?.OuterXml, (int)saved.Id, token: _kasraAdminToken);

                        result = userGroupResult;
                    }
                }
            }

            Task.Run(() =>
            {
                var deviceBrands = _deviceService.GetDeviceBrands(token: _kasraAdminToken);

                foreach (var restRequest in deviceBrands.Select(deviceBrand => new RestRequest(
                    $"{deviceBrand.Name}/{deviceBrand.Name}AccessGroup/ModifyAccessGroup",
                    Method.POST)))
                {
                    restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
                    _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                }
            });

            return result;
        }

        [HttpPost]
        [Route("DeleteAccessGroup")]
        public ResultViewModel DeleteAccessGroup(int id)
        {
            return _accessGroupService.DeleteAccessGroup(id, token: _kasraAdminToken);
        }

        //[HttpPost]
        //public List<ResultViewModel> SendAccessGroupToAllDevices(int accessGroupId)
        //{
        //    var resultList = new List<ResultViewModel>();
        //    var deviceBrands = _deviceService.GetDeviceBrands();

        //    foreach (var deviceBrand in deviceBrands)
        //    {
        //        resultList.AddRange(_communicationManager.CallRest($"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}AccessGroup/SendAccessGroupToAllDevices", "Post", null,
        //                                                            $"{JsonConvert.SerializeObject(accessGroupId)}"));
        //    }

        //    return resultList;
        //}

        [HttpPost]
        [Route("SendAccessGroupToDevices")]
        public List<ResultViewModel> SendAccessGroupToDevices(int accessGroupId)
        {
            var resultList = new List<ResultViewModel>();


            var devices = _accessGroupService.GetDeviceOfAccessGroup(accessGroupId, token: _kasraAdminToken);

            foreach (var device in devices)
            {
                var restRequest =
                    new RestRequest(
                        $"{device.Brand.Name}/{device.Brand.Name}AccessGroup/SendAccessGroupToDevice",
                        Method.GET);
                restRequest.AddParameter("code", device.Code);
                restRequest.AddParameter("accessGroupId", accessGroupId);
                restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
                _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            }
            return resultList;
        }

        [HttpPost]
        [Route("SendAccessGroupToDevice")]
        public ResultViewModel SendAccessGroupToDevice(int accessGroupId, int deviceId)
        {
            var device = _deviceService.GetDevice(deviceId, token: _kasraAdminToken);
            var restRequest =
                new RestRequest(
                    $"{device.Brand.Name}/{device.Brand.Name}AccessGroup/SendAccessGroupToDevice",
                    Method.GET);
            restRequest.AddParameter("code", device.Code);
            restRequest.AddParameter("accessGroupId", accessGroupId);
            restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
            _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return new ResultViewModel { Validate = 1 };
        }

        [HttpPost]
        [Route("SendAllUsersToAllDevicesInAccessGroup")]
        public ResultViewModel SendAllUsersToAllDevicesInAccessGroup(int accessGroupId)
        {
            try
            {
                var deviceBrands = _deviceService.GetDeviceBrands(token: _kasraAdminToken);
                var accessGroup = _accessGroupService.GetAccessGroup(accessGroupId, token: _kasraAdminToken);
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
                        var deviceBrand = deviceBrands.FirstOrDefault(devBrand => devBrand.Code == device.Brand.Code);
                        var restRequest =
                            new RestRequest(
                                $"{deviceBrand?.Name}/{deviceBrand?.Name}AccessGroup/SendAccessGroupToDevice",
                                Method.GET);
                        restRequest.AddParameter("code", device.Code);
                        restRequest.AddParameter("accessGroupId", accessGroupId);
                        restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
                        _restClient.ExecuteAsync<ResultViewModel>(restRequest);

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
                            restRequest =
                                new RestRequest(
                                    $"{deviceBrand?.Name}/{deviceBrand?.Name}User/SendUserToDevice",
                                    Method.GET);
                            restRequest.AddParameter("code", device.Code);
                            restRequest.AddParameter("userId", userids);
                            restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
                            _restClient.ExecuteAsync<ResultViewModel>(restRequest);
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
    }
}