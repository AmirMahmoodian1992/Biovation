using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Biovation.Server.Controllers.v2
{

    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
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

        [HttpGet]
        public Task<ResultViewModel<PagingResult<AccessGroup>>> AccessGroups(long userId = default, int adminUserId = default, int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _accessGroupService.GetAccessGroups(userId, adminUserId, userGroupId, id, deviceId, deviceGroupId, pageNumber, pageSize)
            );
        }

        [HttpPost]
        public Task<ResultViewModel> AddAccessGroup([FromBody]AccessGroup accessGroup)
        {
            return Task.Run(()=>_accessGroupService.AddAccessGroup(accessGroup));
        }

        [HttpPatch]
        public Task<ResultViewModel> ModifyAccessGroup(string accessGroup = default, string deviceGroup = default, string userGroup = default, string adminUserIds = default)
        {
            return Task.Run(() =>

            {
                var xmlDevice = $"{{ DeviceGroup: {deviceGroup} }}";
                var xmlUser = $"{{ UserGroup: {userGroup} }}";
                var xmlAdmin = $"{{AdminUsers: {adminUserIds} }}";

                var xmlDevices = JsonConvert.DeserializeXmlNode(xmlDevice, "Root");
                var xmlUsers = JsonConvert.DeserializeXmlNode(xmlUser, "Root");
                var xmlAdmins = JsonConvert.DeserializeXmlNode(xmlAdmin, "Root");



                var saved = _accessGroupService.ModifyAccessGroup(JsonConvert.DeserializeObject<AccessGroup>(accessGroup ?? string.Empty));
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

                /*
                  var restRequest = new RestRequest($"{deviceBrand.Name}/{device.Brand?.Name}AccessGroup/ModifyAccessGroup");

                             restRequest.AddQueryParameter("DeviceGroup",deviceGroup);
                            restRequest.AddQueryParameter(" UserGroup", userGroup);
                            restRequest.AddQueryParameter("AdminUsers", adminUserIds); 

                */

                Task.Run(() =>
                {
                    var restRequest = new RestRequest($"Queries/v2/Device/DeviceBrands", Method.GET);
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
                );
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ResultViewModel<AccessGroup>> AccessGroup([FromRoute]int id, int nestingDepthLevel = default)
        {
            return Task.Run( () => _accessGroupService.GetAccessGroup(id,nestingDepthLevel));
        }

        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteAccessGroups(int id = default)
        {
            return Task.Run(()=> _accessGroupService.DeleteAccessGroup(id));
        }

        [HttpPost]
        [Route("AllUsersToAllDevicesInAccessGroup/{accessGroupId}")]
        public Task<ResultViewModel> SendAllUsersToAllDevicesInAccessGroup(int accessGroupId = default)
        {
            return Task.Run(() => { 
            try
            {

                var deviceBrands = _deviceService.GetDeviceBrands().Data;
                var accessGroup = _accessGroupService.GetAccessGroup(id: accessGroupId).Data;
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
            });
        }


        [HttpPost]
        [Route("AccessGroupToDevice/{accessGroupId}")]
        public List<ResultViewModel> SendAccessGroupToDevice(int accessGroupId)
        {
            var resultList = new List<ResultViewModel>();


            var devices = _accessGroupService.GetDeviceOfAccessGroup(accessGroupId: accessGroupId).Data.Data;

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


    }
}