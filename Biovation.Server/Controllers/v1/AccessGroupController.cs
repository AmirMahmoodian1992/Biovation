using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.Domain;
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
        private readonly RestClient _restClient;

        public AccessGroupController(RestClient restClient)
        {
            _restClient = restClient;

        }

        [HttpGet, Route("AccessGroups")]
        public List<AccessGroup> AccessGroups(long userId = 0)
        {
            //return _accessGroupService.GetAllAccessGroups(userId);
            var restRequest = new RestRequest($"Queries/v2/AccessGroup/", Method.GET);
            restRequest.AddQueryParameter("userId", userId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<AccessGroup>>>(restRequest);
            return requestResult.Result.Data.Data.Data;
        }

        [HttpGet, Route("GetAccessGroupsByFilter")]
        public List<AccessGroup> GetAccessGroupsByFilter(int adminUserId = 0, int userGroupId = 0, int id = 0, int deviceId = 0, int userId = 0)
        {
            //return _accessGroupService.GetAccessGroupsByFilter(adminUserId, userGroupId, id, deviceId, userId);
            var restRequest = new RestRequest($"Queries/v2/AccessGroup/", Method.GET);
            restRequest.AddQueryParameter("adminUserId", adminUserId.ToString());
            restRequest.AddQueryParameter("userGroupId", userGroupId.ToString());
            restRequest.AddQueryParameter("id", id.ToString());
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            restRequest.AddQueryParameter("userId", userId.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<AccessGroup>>>(restRequest);
            return requestResult.Result.Data.Data.Data;
        }

        [HttpGet, Route("AccessGroup")]
        public AccessGroup AccessGroup(int id)
        {
            var restRequest = new RestRequest($"Queries/v2/AccessGroup/", Method.GET);
            restRequest.AddQueryParameter("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<AccessGroup>>>(restRequest);
            return requestResult.Result.Data.Data.Data[0];
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


            //var result = _accessGroupService.ModifyAccessGroup(JsonConvert.DeserializeObject<AccessGroup>(accessGroup), xmlDevices.OuterXml, xmlUsers.OuterXml, xmlAdmins.OuterXml);

            var restRequest = new RestRequest($"Queries/v2/AccessGroup", Method.PUT);
            restRequest.AddQueryParameter("accessGroup", JsonConvert.DeserializeObject<AccessGroup>(accessGroup).ToString() ?? string.Empty);
            if (xmlDevices != null) restRequest.AddQueryParameter("deviceGroup", xmlDevices.OuterXml);
            if (xmlUsers != null) restRequest.AddQueryParameter("userGroup", xmlUsers.OuterXml);
            if (xmlAdmins != null) restRequest.AddQueryParameter("adminUsers", xmlAdmins.OuterXml);
            var result = (_restClient.ExecuteAsync<ResultViewModel>(restRequest)).Result.Data;

            /*
              var restRequest = new RestRequest($"{deviceBrand.Name}/{device.Brand?.Name}AccessGroup/ModifyAccessGroup");
              
                         restRequest.AddQueryParameter("DeviceGroup",deviceGroup);
                        restRequest.AddQueryParameter(" UserGroup", userGroup);
                        restRequest.AddQueryParameter("AdminUsers", adminUserIds); 

            */

            Task.Run(() =>
            {
                 restRequest = new RestRequest($"Queries/v2/Device/DeviceBrands", Method.GET);
                var deviceBrands = (_restClient.ExecuteAsync<PagingResult<Lookup>>(restRequest)).Result.Data.Data;
                foreach (var deviceBrand in deviceBrands)
                {
                    //_communicationManager.CallRest(
                    //    $"/biovation/api/{deviceBrand.Name}/{deviceBrand.Name}AccessGroup/ModifyAccessGroup", "Post", null, accessGroup);

                     restRequest =
                        new RestRequest(
                            $"{deviceBrand.Name}/{deviceBrand.Name}AccessGroup/ModifyAccessGroup",
                            Method.POST);
                    _restClient.ExecuteAsync<ResultViewModel>(restRequest);

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
            var restRequest = new RestRequest($"Queries/v2/AccessGroup/", Method.DELETE);
            restRequest.AddQueryParameter("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
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

            var restRequest = new RestRequest($"Queries/v2/AccessGroup/DeviceOfAccessGroup", Method.GET);
            restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
            var devices = (_restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceBasicInfo>>>(restRequest)).Result.Data.Data.Data;

            foreach (var device in devices)
            {
                restRequest =
                    new RestRequest(
                        $"{device.Brand.Name}/{device.Brand.Name}AccessGroup/SendAccessGroupToDevice",
                        Method.GET);
                restRequest.AddParameter("code", device.Code);
                restRequest.AddParameter("accessGroupId", accessGroupId);
                _restClient.ExecuteAsync<ResultViewModel>(restRequest);
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
                
                var restRequest = new RestRequest($"Queries/v2/Device/DeviceBrands", Method.GET);
                var deviceBrands = (_restClient.ExecuteAsync<PagingResult<Lookup>>(restRequest)).Result.Data.Data;
                restRequest = new RestRequest($"Queries/v2/AccessGroup/{accessGroupId}", Method.GET);
                restRequest.AddQueryParameter("accessGroupId", accessGroupId.ToString());
                var accessGroup = (_restClient.ExecuteAsync<ResultViewModel<AccessGroup>> (restRequest)).Result.Data.Data;
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
                         restRequest =
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
                            //parameters = new List<object> { $"code={device.Code}", $"userId={userids}", };
                            //_communicationManager.CallRest(
                            //    $"/biovation/api/{deviceBrand?.Name}/{deviceBrand?.Name}User/SendUserToDevice", "Get", parameters, null);
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