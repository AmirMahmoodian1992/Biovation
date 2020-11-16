using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Server.Attribute;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class DeviceGroupController : ControllerBase
    {
        private readonly DeviceService _deviceService;
        private readonly DeviceGroupService _deviceGroupService;

        private readonly RestClient _restClient;

        public DeviceGroupController(DeviceService deviceService, DeviceGroupService deviceGroupService, RestClient restClient)
        {
            _deviceService = deviceService;
            _deviceGroupService = deviceGroupService;
            _restClient = restClient;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ResultViewModel<PagingResult<DeviceGroup>>> GetDeviceGroup(int id = default, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _deviceGroupService.GetDeviceGroups(id, pageNumber, pageSize, HttpContext.Items["Token"].ToString()));
        }

        //[HttpPost]
        //public Task<IActionResult> AddDeviceGroup([FromBody]DeviceGroup deviceGroup = default)
        //{
        //    ///TODO: change the modify sp
        //}


        [HttpPut]
        public Task<ResultViewModel> ModifyDeviceGroup([FromBody] DeviceGroup deviceGroup)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () =>
            {
                try
                {
                    var existingDeviceGroup = deviceGroup.Id == 0 ? null : _deviceGroupService.GetDeviceGroups(deviceGroup.Id, token: token)?.Data?.Data.FirstOrDefault();
                    if (existingDeviceGroup is null && deviceGroup.Id != 0)
                        return new ResultViewModel
                        {
                            Validate = 0,
                            Code = 400,
                            Message = "Provided device group id is wrong, the device group does not exist."
                        };

                    var deletedDevices =
                        existingDeviceGroup?.Devices?.ExceptBy(deviceGroup.Devices ?? new List<DeviceBasicInfo>(), device => device.DeviceId) ?? new List<DeviceBasicInfo>();

                    var addedDevices = (existingDeviceGroup is null ? deviceGroup.Devices :
                        deviceGroup.Devices?.ExceptBy(existingDeviceGroup.Devices ?? new List<DeviceBasicInfo>(), device => device.DeviceId)) ?? new List<DeviceBasicInfo>();

                    var existingAuthorizedUsersOfDeletedDevice = new Dictionary<int, List<User>>();
                    var existingAuthorizedUsersOfAddedDevice = new Dictionary<int, List<User>>();

                    var computeExistingAuthorizedUsersToDelete =
                        Task.Run(() => Parallel.ForEach(deletedDevices, device =>
                        {
                            var authorizedUsersOfDevice =
                                _deviceService.GetAuthorizedUsersOfDevice(device.DeviceId, token)?.Data?.Data;

                            if (authorizedUsersOfDevice is null)
                                return;

                            if (!existingAuthorizedUsersOfDeletedDevice.ContainsKey(device.DeviceId))
                                existingAuthorizedUsersOfDeletedDevice.Add(device.DeviceId, new List<User>());

                            existingAuthorizedUsersOfDeletedDevice[device.DeviceId].AddRange(authorizedUsersOfDevice);
                        })).ContinueWith(_ =>
                             Parallel.For(0, existingAuthorizedUsersOfAddedDevice.Count, index =>
                             {
                                 var element = existingAuthorizedUsersOfDeletedDevice.ElementAt(index);
                                 var authorizedUsers = element.Value.DistinctBy(user => user.Code).ToList();
                                 lock (existingAuthorizedUsersOfDeletedDevice)
                                     existingAuthorizedUsersOfDeletedDevice[element.Key] = authorizedUsers;
                             })
                       );

                    var computeExistingAuthorizedUsersToAdd =
                        Task.Run(() => Parallel.ForEach(addedDevices, device =>
                        {
                            var authorizedUsersOfDevice =
                                _deviceService.GetAuthorizedUsersOfDevice(device.DeviceId, token)?.Data?.Data;

                            if (authorizedUsersOfDevice is null)
                                return;

                            if (!existingAuthorizedUsersOfAddedDevice.ContainsKey(device.DeviceId))
                                existingAuthorizedUsersOfAddedDevice.Add(device.DeviceId, new List<User>());

                            existingAuthorizedUsersOfAddedDevice[device.DeviceId].AddRange(authorizedUsersOfDevice);
                        })).ContinueWith(_ =>
                            Parallel.For(0, existingAuthorizedUsersOfAddedDevice.Count, index =>
                            {
                                var element = existingAuthorizedUsersOfAddedDevice.ElementAt(index);
                                var authorizedUsers = element.Value.DistinctBy(user => user.Code).ToList();
                                lock (existingAuthorizedUsersOfAddedDevice)
                                    existingAuthorizedUsersOfAddedDevice[element.Key] = authorizedUsers;
                            })
                        );

                    Task.WaitAll(computeExistingAuthorizedUsersToDelete, computeExistingAuthorizedUsersToAdd);

                    var result = _deviceGroupService.ModifyDeviceGroup(deviceGroup, token);

                    if (result.Validate == 1)
                    {
                        foreach (var deviceId in existingAuthorizedUsersOfDeletedDevice.Keys)
                        {
                            await Task.Run(async () =>
                            {
                                var device = _deviceService.GetDevice(deviceId, token: token).Data;

                                var newAuthorizedUsersOfDevice =
                                    _deviceService.GetAuthorizedUsersOfDevice(deviceId, token)?.Data?.Data;

                                var usersToDelete = newAuthorizedUsersOfDevice?.Count > 0
                                    ? existingAuthorizedUsersOfDeletedDevice[deviceId].ExceptBy(
                                        newAuthorizedUsersOfDevice,
                                        user => user.Code)
                                    : existingAuthorizedUsersOfDeletedDevice[deviceId];

                                var deleteUserRestRequest =
                                    new RestRequest(
                                        $"{device.Brand.Name}/{device.Brand.Name}Device/DeleteUserFromDevice",
                                        Method.POST);
                                deleteUserRestRequest.AddQueryParameter("code", device.Code.ToString());
                                deleteUserRestRequest.AddJsonBody(usersToDelete.Select(user => user.Code));
                                /*var deletionResult =*/
                                if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                                {
                                    deleteUserRestRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                                }
                                await _restClient.ExecuteAsync<ResultViewModel>(deleteUserRestRequest);
                            });
                        }


                        foreach (var deviceId in existingAuthorizedUsersOfAddedDevice.Keys)
                        {
                            await Task.Run(async () =>
                            {
                                var device = _deviceService.GetDevice(deviceId, token: token).Data;

                                var newAuthorizedUsersOfDevice =
                                        _deviceService.GetAuthorizedUsersOfDevice(deviceId, token)?.Data?.Data;

                                var usersToAdd = existingAuthorizedUsersOfDeletedDevice.ContainsKey(deviceId) &&
                                                     existingAuthorizedUsersOfDeletedDevice[deviceId]?.Count > 0
                                        ? newAuthorizedUsersOfDevice.ExceptBy(
                                            existingAuthorizedUsersOfAddedDevice[deviceId], user => user.Code)
                                        : newAuthorizedUsersOfDevice;

                                if (usersToAdd is null)
                                    return;
                                
                                var sendUserRestRequest =
                                        new RestRequest($"{device.Brand.Name}/{device.Brand.Name}User/SendUserToDevice",
                                            Method.GET);
                                sendUserRestRequest.AddQueryParameter("code", device.Code.ToString());
                                sendUserRestRequest.AddQueryParameter("userId",
                                    JsonConvert.SerializeObject(usersToAdd.Select(user => user.Code)));
                                /*var additionResult =*/
                                if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                                {
                                    sendUserRestRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                                }
                                await _restClient.ExecuteAsync<List<ResultViewModel>>(sendUserRestRequest);
                            });
                        }
                    }

                    return result;
                }
                catch (Exception e)
                {
                    return new ResultViewModel { Message = e.Message, Validate = 0 };
                }
            });
        }



        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteDeviceGroup([FromRoute] int id = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _deviceGroupService.DeleteDeviceGroup(id, token));
        }


        [HttpPost]
        [Route("DeleteDeviceGroups")]
        public Task<List<ResultViewModel>> DeleteDeviceGroups([FromBody] int[] ids)
        {
            return Task.Run(() =>
            {
                try
                {
                    return ids.Select(id =>
                            _deviceGroupService.DeleteDeviceGroup(id, HttpContext.Items["Token"].ToString()))
                        .ToList();
                }
                catch (Exception e)
                {
                    return new List<ResultViewModel> { new ResultViewModel { Message = e.Message, Validate = 0 } };
                }
            });
        }

        [HttpDelete]
        [Route("{id}/DeviceGroupsMembers")]
        public Task<ResultViewModel> DeleteDeviceGroup([FromRoute] uint id = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _deviceGroupService.DeleteDeviceGroupMember(id, token));
        }
    }
}
