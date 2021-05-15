using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.CommonClasses.Extension;
using Biovation.Constants;

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

        private readonly TaskTypes _taskTypes;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskService _taskService;

        private readonly RestClient _restClient;

        public DeviceGroupController(DeviceService deviceService, DeviceGroupService deviceGroupService, RestClient restClient, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, TaskService taskService)
        {
            _deviceService = deviceService;
            _deviceGroupService = deviceGroupService;
            _restClient = restClient;
            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _taskService = taskService;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ResultViewModel<PagingResult<DeviceGroup>>> GetDeviceGroup(int id = default, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _deviceGroupService.GetDeviceGroups(id, pageNumber, pageSize, HttpContext.Items["Token"] as string));
        }

        //[HttpPost]
        //public Task<IActionResult> AddDeviceGroup([FromBody]DeviceGroup deviceGroup = default)
        //{
        //    ///TODO: change the modify sp
        //}


        [HttpPut]
        public async Task<ResultViewModel> ModifyDeviceGroup([FromBody] DeviceGroup deviceGroup)
        {

            try
            {
                var creatorUser = HttpContext.GetUser();
                var token = HttpContext.Items["Token"] as string;
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
                            _deviceService.GetAuthorizedUsersOfDevice(device.DeviceId, token).Result?.Data?.Data;

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
                            _deviceService.GetAuthorizedUsersOfDevice(device.DeviceId, token).Result?.Data?.Data;

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

                var result = await _deviceGroupService.ModifyDeviceGroup(deviceGroup, token);

                if (result.Validate == 1)
                {
                    foreach (var deviceId in existingAuthorizedUsersOfDeletedDevice.Keys)
                    {
                        await Task.Run(async () =>
                        {
                            var device = (await _deviceService.GetDevice(deviceId, token: token)).Data;

                            var newAuthorizedUsersOfDevice =
                               (await _deviceService.GetAuthorizedUsersOfDevice(deviceId, token))?.Data?.Data;

                            var usersToDelete = newAuthorizedUsersOfDevice?.Count > 0
                                ? existingAuthorizedUsersOfDeletedDevice[deviceId].ExceptBy(
                                    newAuthorizedUsersOfDevice,
                                    user => user.Code)
                                : existingAuthorizedUsersOfDeletedDevice[deviceId];

                            var task = new TaskInfo
                            {
                                CreatedAt = DateTimeOffset.Now,
                                CreatedBy = creatorUser,
                                TaskType = _taskTypes.DeleteUsers,
                                Priority = _taskPriorities.Medium,
                                DeviceBrand = device.Brand,
                                TaskItems = new List<TaskItem>(),
                                DueDate = DateTime.Today
                            };

                            foreach (var userCode in usersToDelete.Select(user => user.Code))
                            {

                                task.TaskItems.Add(new TaskItem
                                {
                                    Status = _taskStatuses.Queued,
                                    TaskItemType = _taskItemTypes.DeleteUserFromTerminal,
                                    Priority = _taskPriorities.Medium,
                                    DeviceId = device.DeviceId,
                                    Data = JsonConvert.SerializeObject(new { userCode }),
                                    IsParallelRestricted = true,
                                    IsScheduled = false,
                                    OrderIndex = 1,
                                    CurrentIndex = 0,
                                    TotalCount = 1
                                });
                            }
                            await _taskService.InsertTask(task);
                            await _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);

                            var deleteUserRestRequest =
                                new RestRequest(
                                    $"{device.Brand.Name}/{device.Brand.Name}Device/DeleteUserFromDevice",
                                    Method.POST);
                            deleteUserRestRequest.AddQueryParameter("code", device.Code.ToString());
                            deleteUserRestRequest.AddJsonBody(usersToDelete.Select(user => user.Code));
                            /*var deletionResult =*/
                            deleteUserRestRequest.AddHeader("Authorization", token!);
                            await _restClient.ExecuteAsync<ResultViewModel>(deleteUserRestRequest);
                        });
                    }


                    foreach (var deviceId in existingAuthorizedUsersOfAddedDevice.Keys)
                    {
                        await Task.Run(async () =>
                        {
                            var device = (await _deviceService.GetDevice(deviceId, token: token)).Data;

                            var newAuthorizedUsersOfDevice =
                                    (await _deviceService.GetAuthorizedUsersOfDevice(deviceId, token))?.Data?.Data;

                            var usersToAdd = existingAuthorizedUsersOfDeletedDevice.ContainsKey(deviceId) &&
                                                 existingAuthorizedUsersOfDeletedDevice[deviceId]?.Count > 0
                                    ? newAuthorizedUsersOfDevice.ExceptBy(
                                        existingAuthorizedUsersOfAddedDevice[deviceId], user => user.Code)
                                    : newAuthorizedUsersOfDevice;

                            if (usersToAdd is null)
                                return;

                            var task = new TaskInfo
                            {
                                CreatedAt = DateTimeOffset.Now,
                                CreatedBy = creatorUser,
                                TaskType = _taskTypes.SendUsers,
                                Priority = _taskPriorities.Medium,
                                DeviceBrand = device.Brand,
                                TaskItems = new List<TaskItem>(),
                                DueDate = DateTime.Today
                            };

                            foreach (var userId in usersToAdd.Select(user => user.Id))
                            {
                                task.TaskItems.Add(new TaskItem
                                {
                                    Status = _taskStatuses.Queued,
                                    TaskItemType = _taskItemTypes.SendUser,
                                    Priority = _taskPriorities.Medium,
                                    DeviceId = device.DeviceId,
                                    Data = JsonConvert.SerializeObject(new { userId }),
                                    IsParallelRestricted = true,
                                    IsScheduled = false,
                                    OrderIndex = 1,
                                    CurrentIndex = 0,
                                    TotalCount = 1
                                });
                            }

                            await _taskService.InsertTask(task);
                            await _taskService.ProcessQueue(device.Brand, device.DeviceId);

                            var sendUserRestRequest =
                                    new RestRequest($"{device.Brand.Name}/{device.Brand.Name}User/SendUserToDevice",
                                        Method.GET);
                            sendUserRestRequest.AddQueryParameter("code", device.Code.ToString());
                            sendUserRestRequest.AddQueryParameter("userId",
                                JsonConvert.SerializeObject(usersToAdd.Select(user => user.Code)));
                            /*var additionResult =*/
                            sendUserRestRequest.AddHeader("Authorization", token!);
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
        }



        [HttpDelete]
        [Route("{id}")]
        public async Task<ResultViewModel> DeleteDeviceGroup([FromRoute] int id = default)
        {
            return await _deviceGroupService.DeleteDeviceGroup(id, HttpContext.Items["Token"] as string);
        }


        [HttpPost]
        [Route("DeleteDeviceGroups")]
        public async Task<List<ResultViewModel>> DeleteDeviceGroups([FromBody] int[] ids)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return ids.Select(id =>
                            _deviceGroupService.DeleteDeviceGroup(id, HttpContext.Items["Token"] as string).Result)
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
        public async Task<ResultViewModel> DeleteDeviceGroup([FromRoute] uint id = default)
        {
            return await _deviceGroupService.DeleteDeviceGroupMember(id, HttpContext.Items["Token"] as string);
        }
    }
}
