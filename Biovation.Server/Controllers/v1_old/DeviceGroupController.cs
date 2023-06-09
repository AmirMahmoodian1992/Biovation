﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.API.v2;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using Newtonsoft.Json;
using RestSharp;

namespace Biovation.Server.Controllers.v1
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class DeviceGroupController : Controller
    {
        private readonly DeviceService _deviceService;
        private readonly DeviceGroupService _deviceGroupService;

        private readonly RestClient _restClient;

        public DeviceGroupController(DeviceService deviceService, DeviceGroupService deviceGroupService)
        {
            _deviceService = deviceService;
            _deviceGroupService = deviceGroupService;
            _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }

        [HttpGet]
        [Route("GetDeviceGroup")]
        public List<DeviceGroup> GetDeviceGroup(int? id, long userId)
        {
            return id == null ? _deviceGroupService.GetDeviceGroups(userId:userId).Data.Data : _deviceGroupService.GetDeviceGroups((int)id, userId).Data.Data;
        }

        [HttpPost]
        [Route("ModifyDeviceGroup")]
        public Task<ResultViewModel> ModifyDeviceGroup(DeviceGroup deviceGroup)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var existingDeviceGroup = deviceGroup.Id == 0 ? null : _deviceGroupService.GetDeviceGroups(deviceGroup.Id).Data.Data.FirstOrDefault();
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
                                _deviceService.GetAuthorizedUsersOfDevice(device.DeviceId).Data.Data;

                            if (!existingAuthorizedUsersOfDeletedDevice.ContainsKey(device.DeviceId))
                                existingAuthorizedUsersOfDeletedDevice.Add(device.DeviceId, new List<User>());

                            existingAuthorizedUsersOfDeletedDevice[device.DeviceId].AddRange(authorizedUsersOfDevice);
                        })).ContinueWith(_ =>
                             Parallel.For(0, existingAuthorizedUsersOfAddedDevice.Count, index =>
                             {
                                 var element = existingAuthorizedUsersOfDeletedDevice.ElementAt(index);
                                 var authorizedUsers = element.Value.DistinctBy(user => user.Id).ToList();
                                 lock (existingAuthorizedUsersOfDeletedDevice)
                                     existingAuthorizedUsersOfDeletedDevice[element.Key] = authorizedUsers;
                             })
                       );

                    var computeExistingAuthorizedUsersToAdd =
                        Task.Run(() => Parallel.ForEach(addedDevices, device =>
                        {
                            var authorizedUsersOfDevice =
                                _deviceService.GetAuthorizedUsersOfDevice(device.DeviceId).Data.Data;

                            if (!existingAuthorizedUsersOfAddedDevice.ContainsKey(device.DeviceId))
                                existingAuthorizedUsersOfAddedDevice.Add(device.DeviceId, new List<User>());

                            existingAuthorizedUsersOfAddedDevice[device.DeviceId].AddRange(authorizedUsersOfDevice);
                        })).ContinueWith(_ =>
                            Parallel.For(0, existingAuthorizedUsersOfAddedDevice.Count, index =>
                            {
                                var element = existingAuthorizedUsersOfAddedDevice.ElementAt(index);
                                var authorizedUsers = element.Value.DistinctBy(user => user.Id).ToList();
                                lock (existingAuthorizedUsersOfAddedDevice)
                                    existingAuthorizedUsersOfAddedDevice[element.Key] = authorizedUsers;
                            })
                        );

                    Task.WaitAll(computeExistingAuthorizedUsersToDelete, computeExistingAuthorizedUsersToAdd);

                    var result = _deviceGroupService.ModifyDeviceGroup(deviceGroup);

                    if (result.Validate == 1)
                    {
                        foreach (var deviceId in existingAuthorizedUsersOfDeletedDevice.Keys)
                        {
                            await Task.Run(async () =>
                            {
                                var device = _deviceService.GetDevice(deviceId).Data;

                                var newAuthorizedUsersOfDevice =
                                    _deviceService.GetAuthorizedUsersOfDevice(deviceId).Data.Data;

                                var usersToDelete = newAuthorizedUsersOfDevice?.Count > 0
                                    ? existingAuthorizedUsersOfDeletedDevice[deviceId].ExceptBy(
                                        newAuthorizedUsersOfDevice,
                                        user => user.Id)
                                    : existingAuthorizedUsersOfDeletedDevice[deviceId];

                                var deleteUserRestRequest =
                                    new RestRequest(
                                        $"{device.Brand.Name}/{device.Brand.Name}Device/DeleteUserFromDevice",
                                        Method.POST);
                                deleteUserRestRequest.AddQueryParameter("code", device.Code.ToString());
                                deleteUserRestRequest.AddJsonBody(usersToDelete.Select(user => user.Id));
                                /*var deletionResult =*/
                                await _restClient.ExecuteAsync<ResultViewModel>(deleteUserRestRequest);
                            });
                        }


                        foreach (var deviceId in existingAuthorizedUsersOfAddedDevice.Keys)
                        {
                            await Task.Run(async () =>
                            {
                                var device = _deviceService.GetDevice(deviceId).Data;

                                var newAuthorizedUsersOfDevice =
                                        _deviceService.GetAuthorizedUsersOfDevice(deviceId).Data.Data;

                                var usersToAdd = existingAuthorizedUsersOfDeletedDevice.ContainsKey(deviceId) &&
                                                     existingAuthorizedUsersOfDeletedDevice[deviceId]?.Count > 0
                                        ? newAuthorizedUsersOfDevice.ExceptBy(
                                            existingAuthorizedUsersOfAddedDevice[deviceId], user => user.Id)
                                        : newAuthorizedUsersOfDevice;

                                var sendUserRestRequest =
                                        new RestRequest($"{device.Brand.Name}/{device.Brand.Name}User/SendUserToDevice",
                                            Method.GET);
                                sendUserRestRequest.AddQueryParameter("code", device.Code.ToString());
                                sendUserRestRequest.AddQueryParameter("userId",
                                    JsonConvert.SerializeObject(usersToAdd.Select(user => user.Id)));
                                /*var additionResult =*/
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

        [HttpPost]
        [Route("DeleteDeviceGroup")]
        public List<ResultViewModel> DeleteDeviceGroup([FromBody] int[] ids)
        {
            try
            {
                //var id = !string.IsNullOrEmpty(ids) ? JsonConvert.DeserializeObject<int[]>(ids) : new int[0];

                return ids.Select(id => _deviceGroupService.DeleteDeviceGroup(id)).ToList();
            }
            catch (Exception e)
            {
                return new List<ResultViewModel> { new ResultViewModel { Message = e.Message, Validate = 0 } };
            }
        }


        [HttpGet]
        [Route("GetAccessControlDeviceGroup")]
        public List<DeviceGroup> GetAccessControlDeviceGroup(int id)
        {
            return _deviceGroupService.GetAccessControlDeviceGroup(id).Data.Data;
        }
    }
}
