using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service;
using Biovation.Service.SQL.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Biovation.Server.Controllers.v1
{

    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AccessGroupController : Controller
    {
        //private readonly CommunicationManager<ResultViewModel> _communicationManager = new CommunicationManager<ResultViewModel>();
        private readonly AccessGroupService _accessGroupService;
        private readonly DeviceService _deviceService;
        private readonly RestClient _restServer;

        public AccessGroupController(AccessGroupService accessGroupService, DeviceService deviceService)
        {
            _accessGroupService = accessGroupService;
            _deviceService = deviceService;
            _restServer =
                new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}");
            //_communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        }

        [HttpGet]
        [Route("AccessGroups")]
        public List<AccessGroup> AccessGroups(long userId = 0)
        {
            return _accessGroupService.GetAllAccessGroups(userId);
        }

        [HttpGet]
        [Route("GetAccessGroupsByFilter")]
        public List<AccessGroup> GetAccessGroupsByFilter(int adminUserId = 0, int userGroupId = 0, int id = 0, int deviceId = 0, int userId = 0)
        {
            return _accessGroupService.GetAccessGroupsByFilter(adminUserId, userGroupId, id, deviceId, userId);
        }

        [HttpGet]
        [Route("AccessGroup")]
        public AccessGroup AccessGroup(int id)
        {
            return _accessGroupService.GetAccessGroupById(id);
        }

        //for routing problem(same address)
        //[HttpGet]
        //[Route("AccessGroup")]/////////////
        //public List<AccessGroup> AccessGroup(int id, int deviceGroupId, int userId)
        //{
        //    return _accessGroupService.SearchAccessGroups(id, deviceGroupId, userId);
        //}

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


            var result = _accessGroupService.ModifyAccessGroup(JsonConvert.DeserializeObject<AccessGroup>(accessGroup), xmlDevices.OuterXml, xmlUsers.OuterXml, xmlAdmins.OuterXml);

            /*
              var restRequest = new RestRequest($"{deviceBrand.Name}/{device.Brand?.Name}AccessGroup/ModifyAccessGroup");
              
                         restRequest.AddQueryParameter("DeviceGroup",deviceGroup);
                        restRequest.AddQueryParameter(" UserGroup", userGroup);
                        restRequest.AddQueryParameter("AdminUsers", adminUserIds); 

            */

            Task.Run(() =>
            {
                var deviceBrands = _deviceService.GetDeviceBrands();
                foreach (var deviceBrand in deviceBrands)
                {
                    //_communicationManager.CallRest(
                    //    $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}AccessGroup/ModifyAccessGroup", "Post", null, accessGroup);

                    var restRequest =
                        new RestRequest(
                            $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}AccessGroup/ModifyAccessGroup",
                            Method.POST);
                    _restServer.ExecuteAsync<ResultViewModel>(restRequest);

                    //_apiClient.CallApi($"{deviceBrand.Name}/{deviceBrand.Name}AccessGroup/ModifyAccessGroup",
                    //    Method.Post, postBody: JsonConvert.DeserializeObject<AccessGroup>(accessGroup));
                }
            });

            return result;
        }

        [HttpPost]
        [Route("DeleteAccessGroup")]
        public ResultViewModel DeleteAccessGroup(int id)
        {
            return _accessGroupService.DeleteAccessGroupById(id);
        }

        //[HttpPost]
        //[Route("controller")]public List<ResultViewModel> SendAccessGroupToAllDevices(int accessGroupId)
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
        public List<ResultViewModel> SendAccessGroupToDevice(int accessGroupId)
        {
            var resultList = new List<ResultViewModel>();
            //var devices = JsonConvert.DeserializeObject<int[]>(deviceIds);
            var devices = _accessGroupService.GetDeviceOfAccessGroup(accessGroupId);
            foreach (var device in devices)
            {
                //var parameters = new List<object> { $"code={device.Code}", $"accessGroupId={accessGroupId}" };
                //resultList.Add(_communicationManager.CallRest($"/biovation/api/{device.Brand.Name}/{device.Brand.Name}AccessGroup/SendAccessGroupToDevice", "Get", parameters));
                var restRequest =
                    new RestRequest(
                        $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}AccessGroup/SendAccessGroupToDevice",
                        Method.GET);
                restRequest.AddParameter("code", device.Code);
                restRequest.AddParameter("accessGroupId", accessGroupId);
                _restServer.ExecuteAsync<ResultViewModel>(restRequest);
            }
            return resultList;
        }


        //for routing problem(same address)
        //[HttpPost]
        //[Route("SendAccessGroupToDevice")]//////////
        //public ResultViewModel SendAccessGroupToDevice(int accessGroupId, int deviceId)
        //{
        //    var device = _deviceService.GetDeviceInfo(deviceId);
        //    //var parameters = new List<object> { $"code={device.Code}", $"accessGroupId={accessGroupId}" };
        //    //_communicationManager.CallRest(
        //    //    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}AccessGroup/SendAccessGroupToDevice", "Get", parameters);
        //    var restRequest =
        //        new RestRequest(
        //            $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}AccessGroup/SendAccessGroupToDevice",
        //            Method.GET);
        //    restRequest.AddParameter("code", device.Code);
        //    restRequest.AddParameter("accessGroupId", accessGroupId);
        //    _restServer.ExecuteAsync<ResultViewModel>(restRequest);
        //    return new ResultViewModel { Validate = 1 };
        //}

        [HttpPost]
        [Route("SendAllUsersToAllDevicesInAccessGroup")]
        public ResultViewModel SendAllUsersToAllDevicesInAccessGroup(int accessGroupId)
        {
            try
            {
                var deviceBrands = _deviceService.GetDeviceBrands();
                var accessGroup = _accessGroupService.GetAccessGroupById(accessGroupId);
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
                        //var parameters = new List<object> { $"accessGroupId={accessGroupId}", $"code={device.Code}" };
                        //_communicationManager.CallRest(
                        //    $"/biovation/api/{deviceBrand?.Name}/{deviceBrand?.Name}AccessGroup/SendAccessGroupToDevice", "Get", parameters, null);
                        var restRequest =
                            new RestRequest(
                                $"/biovation/api/{deviceBrand?.Name}/{deviceBrand?.Name}AccessGroup/SendAccessGroupToDevice",
                                Method.GET);
                        restRequest.AddParameter("code", device.Code);
                        restRequest.AddParameter("accessGroupId", accessGroupId);
                        _restServer.ExecuteAsync<ResultViewModel>(restRequest);

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
                                    $"/biovation/api/{deviceBrand?.Name}/{deviceBrand?.Name}User/SendUserToDevice",
                                    Method.GET);
                            restRequest.AddParameter("code", device.Code);
                            restRequest.AddParameter("userId", userids);
                            _restServer.ExecuteAsync<ResultViewModel>(restRequest);
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