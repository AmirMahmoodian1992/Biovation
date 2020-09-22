using Biovation.CommonClasses;
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
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AccessGroupController : Controller
    {
        //private readonly CommunicationManager<ResultViewModel> _communicationManager = new CommunicationManager<ResultViewModel>();
        private readonly RestClient _restClient;
        private readonly AccessGroupService _accessGroupService;
        private readonly DeviceService _deviceService;

        public AccessGroupController(RestClient restClient, AccessGroupService accessGroupService, DeviceService deviceService)
        {
            _restClient = restClient;
            _accessGroupService = accessGroupService;
            _deviceService = deviceService;

        }

        [HttpGet, Route("AccessGroups")]
        public List<AccessGroup> AccessGroups(long userId = 0)
        {
            return _accessGroupService.GetAccessGroups(adminUserId: (int)userId);
        }

        //[HttpGet, Route("GetAccessGroupsByFilter")]
        //public List<AccessGroup> GetAccessGroupsByFilter(int adminUserId = 0, int userGroupId = 0, int id = 0, int deviceId = 0, int userId = 0)
        //{
        //    return _accessGroupService.GetAccessGroups(adminUserId: adminUserId, userGroupId: userGroupId, id: id,
        //        deviceId: deviceId, userId: userId);
        //}

        [HttpGet]
        [Route("AccessGroup")]
        public AccessGroup AccessGroup(int id)
        {
            return _accessGroupService.GetAccessGroup(id);
        }

        [HttpGet, Route("GetAccessGroupsByFilter")]
        public List<AccessGroup> AccessGroupsByFilter(int id, int deviceGroupId, int userId)
        {
            return _accessGroupService.GetAccessGroups(id: id,
                deviceGroupId: deviceGroupId, adminUserId: userId);
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


            var saved = _accessGroupService.ModifyAccessGroup(JsonConvert.DeserializeObject<AccessGroup>(accessGroup));
            ResultViewModel result;

            if (saved.Validate != 1)
                result = new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };
            else
            {

                var deviceResult = _accessGroupService.ModifyAccessGroupDeviceGroup(xmlDevices?.OuterXml, (int)saved.Id);
                if (deviceResult.Validate != 1)
                    result = new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };
                else
                {

                    var adminUsersResult = _accessGroupService.ModifyAccessGroupAdminUsers(xmlAdmins?.OuterXml, (int)saved.Id);
                    if (adminUsersResult.Validate != 1)
                        result = new ResultViewModel { Validate = 0, Message = "ذخیره انجام نشد مجددا تلاش فرمایید" };
                    else
                    {
                        var userGroupResult = _accessGroupService.ModifyAccessGroupUserGroup(xmlUsers?.OuterXml, (int)saved.Id);

                        result = userGroupResult;
                    }
                }
            }

            Task.Run(() =>
            {
                var deviceBrands = _deviceService.GetDeviceBrands();

                foreach (var restRequest in deviceBrands.Select(deviceBrand => new RestRequest(
                    $"{deviceBrand.Name}/{deviceBrand.Name}AccessGroup/ModifyAccessGroup",
                    Method.POST)))
                {
                    _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                }
            });

            return result;
        }

        [HttpPost]
        [Route("DeleteAccessGroup")]
        public ResultViewModel DeleteAccessGroup(int id)
        {
            return _accessGroupService.DeleteAccessGroup(id);
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
        [Route("SendAccessGroupToDevice")]
        public List<ResultViewModel> SendAccessGroupToDevices(int accessGroupId)
        {
            var resultList = new List<ResultViewModel>();


            var devices = _accessGroupService.GetDeviceOfAccessGroup(accessGroupId);

            foreach (var device in devices)
            {
                var restRequest =
                    new RestRequest(
                        $"{device.Brand.Name}/{device.Brand.Name}AccessGroup/SendAccessGroupToDevice",
                        Method.GET);
                restRequest.AddParameter("code", device.Code);
                restRequest.AddParameter("accessGroupId", accessGroupId);
                _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            }
            return resultList;
        }

        [HttpPost]
        [Route("SendAccessGroupToDevice")]
        public ResultViewModel SendAccessGroupToDevice(int accessGroupId, int deviceId)
        {
            var device = _deviceService.GetDevice(deviceId);
            var restRequest =
                new RestRequest(
                    $"{device.Brand.Name}/{device.Brand.Name}AccessGroup/SendAccessGroupToDevice",
                    Method.GET);
            restRequest.AddParameter("code", device.Code);
            restRequest.AddParameter("accessGroupId", accessGroupId);
            _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return new ResultViewModel { Validate = 1 };
        }

        [HttpPost]
        [Route("SendAllUsersToAllDevicesInAccessGroup")]
        public ResultViewModel SendAllUsersToAllDevicesInAccessGroup(int accessGroupId)
        {
            try
            {
                var deviceBrands = _deviceService.GetDeviceBrands();
                var accessGroup = _accessGroupService.GetAccessGroup(accessGroupId);
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