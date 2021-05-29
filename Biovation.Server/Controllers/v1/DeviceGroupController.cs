using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Server.Managers;
using Biovation.Service.Api.v1;
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

namespace Biovation.Server.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class DeviceGroupController : ControllerBase
    {
        private readonly DeviceService _deviceService;
        private readonly DeviceGroupService _deviceGroupService;
        private readonly string _kasraAdminToken;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        private readonly TokenGenerator _tokenGenerator;
        private readonly UserService _userService;
        private readonly RestClient _restClient;

        private readonly TaskTypes _taskTypes;
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;

        public DeviceGroupController(DeviceService deviceService, DeviceGroupService deviceGroupService, BiovationConfigurationManager biovationConfigurationManager, RestClient restClient, TokenGenerator tokenGenerator, UserService userService, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, TaskService taskService)
        {
            _deviceService = deviceService;
            _deviceGroupService = deviceGroupService;
            _biovationConfigurationManager = biovationConfigurationManager;
            _kasraAdminToken = _biovationConfigurationManager.KasraAdminToken;
            _restClient = restClient;
            _tokenGenerator = tokenGenerator;
            _userService = userService;

            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _taskService = taskService;
        }

        [HttpGet]
        [Route("GetDeviceGroup")]
        public List<DeviceGroup> GetDeviceGroup(int? id, long userId)
        {
            var token = _tokenGenerator.GenerateToken(userId == 0 || userId == 123456789 ? _biovationConfigurationManager.KasraAdminUser : _userService.GetUsers(code: userId).FirstOrDefault());
            return id == null ? _deviceGroupService.GetDeviceGroups(token: token) : _deviceGroupService.GetDeviceGroups((int)id, token: token);
        }

        [HttpPost]
        [Route("ModifyDeviceGroup")]
        public Task<ResultViewModel> ModifyDeviceGroup([FromBody] DeviceGroup deviceGroup)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var creatorUser = HttpContext.GetUser();
                    var existingDeviceGroup = deviceGroup.Id == 0 ? null : _deviceGroupService.GetDeviceGroups(deviceGroup.Id, token: _kasraAdminToken).FirstOrDefault();
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
                               _deviceService.GetAuthorizedUsersOfDevice(device.DeviceId, _kasraAdminToken);

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
                                _deviceService.GetAuthorizedUsersOfDevice(device.DeviceId, _kasraAdminToken);

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

                    var result = await _deviceGroupService.ModifyDeviceGroup(deviceGroup, _kasraAdminToken);

                    if (result.Validate == 1)
                    {
                        foreach (var deviceId in existingAuthorizedUsersOfDeletedDevice.Keys)
                        {
                            await Task.Run(async () =>
                            {
                                var device = _deviceService.GetDevice(deviceId, token: _kasraAdminToken);

                                var newAuthorizedUsersOfDevice =
                                    _deviceService.GetAuthorizedUsersOfDevice(deviceId);

                                var usersToDelete = newAuthorizedUsersOfDevice?.Count > 0
                                    ? existingAuthorizedUsersOfDeletedDevice[deviceId].ExceptBy(
                                        newAuthorizedUsersOfDevice,
                                        user => user.Id)
                                    : existingAuthorizedUsersOfDeletedDevice[deviceId];

                                var task = new TaskInfo
                                {
                                    Status = _taskStatuses.Queued,
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
                                _taskService.InsertTask(task);
                                await _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);

                                var deleteUserRestRequest =
                                    new RestRequest(
                                        $"{device.Brand.Name}/{device.Brand.Name}Device/DeleteUserFromDevice",
                                        Method.POST);
                                deleteUserRestRequest.AddQueryParameter("code", device.Code.ToString());
                                deleteUserRestRequest.AddJsonBody(usersToDelete.Select(user => user.Id));
                                /*var deletionResult =*/

                                deleteUserRestRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
                                await _restClient.ExecuteAsync<ResultViewModel>(deleteUserRestRequest);
                            });
                        }


                        foreach (var deviceId in existingAuthorizedUsersOfAddedDevice.Keys)
                        {
                            await Task.Run(async () =>
                            {
                                var device = _deviceService.GetDevice(deviceId, token: _kasraAdminToken);

                                var newAuthorizedUsersOfDevice =
                                        _deviceService.GetAuthorizedUsersOfDevice(deviceId, _kasraAdminToken);

                                var usersToAdd = existingAuthorizedUsersOfDeletedDevice.ContainsKey(deviceId) &&
                                                     existingAuthorizedUsersOfDeletedDevice[deviceId]?.Count > 0
                                        ? newAuthorizedUsersOfDevice.ExceptBy(
                                            existingAuthorizedUsersOfAddedDevice[deviceId], user => user.Id)
                                        : newAuthorizedUsersOfDevice;
                                
                                var task = new TaskInfo
                                {
                                    Status = _taskStatuses.Queued,
                                    CreatedAt = DateTimeOffset.Now,
                                    CreatedBy = creatorUser,
                                    TaskType = _taskTypes.SendUsers,
                                    Priority = _taskPriorities.Medium,
                                    DeviceBrand = device.Brand,
                                    TaskItems = new List<TaskItem>(),
                                    DueDate = DateTime.Today
                                };

                                foreach (var id in usersToAdd.Select(user=> user.Code))
                                {
                                    task.TaskItems.Add(new TaskItem
                                    {
                                        Status = _taskStatuses.Queued,
                                        TaskItemType = _taskItemTypes.SendUser,
                                        Priority = _taskPriorities.Medium,
                                        DeviceId = device.DeviceId,
                                        Data = JsonConvert.SerializeObject(new { userId = id }),
                                        IsParallelRestricted = true,
                                        IsScheduled = false,
                                        OrderIndex = 1,
                                        CurrentIndex = 0,
                                        TotalCount = 1
                                    });
                                }

                                _taskService.InsertTask(task);
                                await _taskService.ProcessQueue(device.Brand, device.DeviceId);

                                var sendUserRestRequest =
                                        new RestRequest($"{device.Brand.Name}/{device.Brand.Name}User/SendUserToDevice",
                                            Method.GET);
                                sendUserRestRequest.AddQueryParameter("code", device.Code.ToString());
                                sendUserRestRequest.AddQueryParameter("userId", JsonConvert.SerializeObject(usersToAdd.Select(user => user.Id)));
                                /*var additionResult =*/
                                sendUserRestRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
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
                return ids.Select(id => _deviceGroupService.DeleteDeviceGroup(id, _kasraAdminToken)).ToList();
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
            return _deviceGroupService.GetAccessControlDeviceGroup(id, token: _kasraAdminToken);
        }
    }
}
