using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using MoreLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2
{
    public class UserGroupService
    {
        private readonly UserGroupRepository _userGroupRepository;
        private readonly TaskService _taskService;
        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly UserGroupService _userGroupService;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        public UserGroupService(UserService userService, DeviceService deviceService, UserGroupRepository userGroupRepository, TaskService taskService, TaskTypes taskTypes, TaskPriorities taskPriorities, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, UserGroupService userGroupService)
        {
            _userService = userService;
            _deviceService = deviceService;
            _userGroupRepository = userGroupRepository;
            _taskService = taskService;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _userGroupService = userGroupService;
        }

        public async Task<ResultViewModel<PagingResult<UserGroup>>> UserGroups(int userGroupId = default, string token = default)
        {
            return await _userGroupRepository.UserGroups(userGroupId, token);
        }


        public async Task<ResultViewModel<List<UserGroup>>> GetAccessControlUserGroup(int id = default, string token = default)
        {
            return await _userGroupRepository.GetAccessControlUserGroup(id, token);
        }

        public async Task<ResultViewModel> SyncUserGroupMember(string lstUser = default, string token = default)
        {
            return await _userGroupRepository.SyncUserGroupMember(lstUser, token);
        }

        public async Task<ResultViewModel> AddUserGroup(UserGroupMember userGroupMember = default, string token = default)
        {
            return await _userGroupRepository.AddUserGroup(userGroupMember, token);
        }

        public async Task<ResultViewModel> ModifyUserGroup(UserGroup userGroup = default, string token = default)
        {
            return await _userGroupRepository.ModifyUserGroup(userGroup, token);
        }

        public async Task<ResultViewModel> DeleteUserGroup(int groupId = default, string token = default)
        {
            return await _userGroupRepository.DeleteUserGroup(groupId, token);
        }
        public ResultViewModel ModifyUserGroupMember(List<UserGroupMember> member, int userGroupId, string token = default)
        {
            return _userGroupRepository.ModifyUserGroupMember(member, userGroupId, token);
        }

        public Task<ResultViewModel<PagingResult<UserGroup>>> GetUsersGroupBase(string token, int id)
        {
            return Task.Run(() => _userGroupService.UserGroups(id, token));
        }

        public async Task<ResultViewModel> ModifyUserGroupBase(string token, User creatorUser, UserGroup userGroup)
        {
            try
            {
                List<DeviceBasicInfo> allChangedDevices = new List<DeviceBasicInfo>();

                if (userGroup is null)
                    return new ResultViewModel
                    { Success = false, Validate = 0, Code = 404, Message = "Null user group is provided!" };

                var existingUserGroup = userGroup.Id == 0 ? null : (await _userGroupService.UserGroups(userGroup.Id, token: token))?.Data?.Data.FirstOrDefault();
                if (existingUserGroup is null && userGroup.Id != 0)
                {
                    return new ResultViewModel
                    {
                        Validate = 0,
                        Code = 400,
                        Message = "Provided user group id is wrong, the user group does not exist."
                    };
                }

                var usersToDelete = existingUserGroup?.Users.ExceptBy(userGroup.Users, member => member.UserId).ToList() ?? new List<UserGroupMember>();

                var usersToAdd = (existingUserGroup is null ? userGroup.Users :
                    userGroup.Users?.ExceptBy(existingUserGroup.Users, member => member.UserId).ToList()) ?? new List<UserGroupMember>();

                var existingAuthorizedDevicesOfUserToDelete = new Dictionary<long, List<DeviceBasicInfo>>();
                var existingAuthorizedDevicesOfUserToAdd = new Dictionary<long, List<DeviceBasicInfo>>();

                var existingAuthorizedUsersOfDevicesToDelete = new Dictionary<int, List<UserGroupMember>>();
                var newAuthorizedUsersOfDevicesToDelete = new Dictionary<int, List<UserGroupMember>>();

                var computeExistingDeletion = Task.Run(() => Parallel.ForEach(usersToDelete, user =>
                {
                    var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser((int)(user.UserId), token).Data;
                    if (!existingAuthorizedDevicesOfUserToDelete.ContainsKey(user.UserId))
                        existingAuthorizedDevicesOfUserToDelete.Add(user.UserId, new List<DeviceBasicInfo>());

                    existingAuthorizedDevicesOfUserToDelete[user.UserId].AddRange(authorizedDevicesOfUser);

                })).ContinueWith(_ =>
                    Parallel.For(0, existingAuthorizedDevicesOfUserToDelete.Count, index =>
                    {
                        var element = existingAuthorizedDevicesOfUserToDelete.ElementAt(index);
                        var devices = element.Value.DistinctBy(device => device.DeviceId).ToList();
                        existingAuthorizedDevicesOfUserToDelete[element.Key] = devices;
                    }));

                var existingAuthorizedUsersOfDevicesToAdd = new Dictionary<int, List<UserGroupMember>>();
                var newAuthorizedUsersOfDevicesToAdd = new Dictionary<int, List<UserGroupMember>>();

                var computeExistingAddition = Task.Run(() => Parallel.ForEach(usersToAdd, user =>
                {
                    var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser((int)(user.UserId), token).Data;
                    if (!existingAuthorizedDevicesOfUserToAdd.ContainsKey(user.UserId))
                        existingAuthorizedDevicesOfUserToAdd.Add(user.UserId, new List<DeviceBasicInfo>());

                    existingAuthorizedDevicesOfUserToAdd[user.UserId].AddRange(authorizedDevicesOfUser);
                })).ContinueWith(_ =>
                    Parallel.For(0, existingAuthorizedDevicesOfUserToAdd.Count, index =>
                    {
                        var element = existingAuthorizedDevicesOfUserToAdd.ElementAt(index);
                        var devices = element.Value.DistinctBy(device => device.DeviceId).ToList();
                        existingAuthorizedDevicesOfUserToAdd[element.Key] = devices;
                    }));

                Task.WaitAll(computeExistingAddition, computeExistingDeletion);

                var result = await _userGroupService.ModifyUserGroup(userGroup, token);
                if (result.Validate != 1) return result;

                var computeNewDeletion = Task.Run(() => Parallel.ForEach(usersToDelete, user =>
                {
                    var oldAuthorizedDevicesOfUser =
                        existingAuthorizedDevicesOfUserToDelete.ContainsKey(user.UserId)
                            ? existingAuthorizedDevicesOfUserToDelete[user.UserId]
                            : new List<DeviceBasicInfo>();

                    var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser((int)(user.UserId), token).Data;

                    var computeNewStateTask = Task.Run(() => Parallel.ForEach(authorizedDevicesOfUser, device =>
                    {
                        lock (newAuthorizedUsersOfDevicesToDelete)
                            if (!newAuthorizedUsersOfDevicesToDelete.ContainsKey(device.DeviceId))
                                newAuthorizedUsersOfDevicesToDelete.Add(device.DeviceId,
                                    new List<UserGroupMember>());

                        newAuthorizedUsersOfDevicesToDelete[device.DeviceId].Add(user);
                    }));

                    var computeOldStateTask = Task.Run(() => Parallel.ForEach(oldAuthorizedDevicesOfUser, device =>
                    {
                        lock (existingAuthorizedUsersOfDevicesToDelete)
                            if (!existingAuthorizedUsersOfDevicesToDelete.ContainsKey(device.DeviceId))
                                existingAuthorizedUsersOfDevicesToDelete.Add(device.DeviceId,
                                    new List<UserGroupMember>());

                        existingAuthorizedUsersOfDevicesToDelete[device.DeviceId].Add(user);
                    }));

                    Task.WaitAll(computeOldStateTask, computeNewStateTask);

                })).ContinueWith(_ =>
                {
                    var computeNewStateTask = Task.Run(() => Parallel.For(0, newAuthorizedUsersOfDevicesToDelete.Count,
                        index =>
                        {
                            var element = newAuthorizedUsersOfDevicesToDelete.ElementAt(index);
                            var users = element.Value.DistinctBy(user => user.UserId).ToList();
                            newAuthorizedUsersOfDevicesToDelete[element.Key] = users;
                        }));

                    var computeOldStateTask = Task.Run(() => Parallel.For(0, existingAuthorizedUsersOfDevicesToDelete.Count,
                            index =>
                            {
                                var element = existingAuthorizedUsersOfDevicesToDelete.ElementAt(index);
                                var users = element.Value.DistinctBy(user => user.UserId).ToList();
                                existingAuthorizedUsersOfDevicesToDelete[element.Key] = users;
                            }));

                    Task.WaitAll(computeOldStateTask, computeNewStateTask);
                });

                var computeNewAddition = Task.Run(() => Parallel.ForEach(usersToAdd, user =>
                {
                    var oldAuthorizedDevicesOfUser =
                        existingAuthorizedDevicesOfUserToAdd.ContainsKey(user.UserId)
                            ? existingAuthorizedDevicesOfUserToAdd[user.UserId]
                            : new List<DeviceBasicInfo>();
                    var authorizedDevicesOfUser = _userService.GetAuthorizedDevicesOfUser((int)(user.UserId), token).Data;
                    var computeNewStateTask = Task.Run(() => Parallel.ForEach(authorizedDevicesOfUser, device =>
                    {
                        lock (newAuthorizedUsersOfDevicesToAdd)
                            if (!newAuthorizedUsersOfDevicesToAdd.ContainsKey(device.DeviceId))
                                newAuthorizedUsersOfDevicesToAdd.Add(device.DeviceId, new List<UserGroupMember>());

                        newAuthorizedUsersOfDevicesToAdd[device.DeviceId].Add(user);
                    }));

                    var computeOldStateTask = Task.Run(() => Parallel.ForEach(oldAuthorizedDevicesOfUser, device =>
                    {
                        lock (existingAuthorizedUsersOfDevicesToAdd)
                            if (!existingAuthorizedUsersOfDevicesToAdd.ContainsKey(device.DeviceId))
                                existingAuthorizedUsersOfDevicesToAdd.Add(device.DeviceId,
                                    new List<UserGroupMember>());

                        existingAuthorizedUsersOfDevicesToAdd[device.DeviceId].Add(user);
                    }));

                    Task.WaitAll(computeOldStateTask, computeNewStateTask);
                })).ContinueWith(_ =>
                {
                    var computeNewStateTask = Task.Run(() => Parallel.For(0, newAuthorizedUsersOfDevicesToAdd.Count,
                        index =>
                        {
                            var element = newAuthorizedUsersOfDevicesToAdd.ElementAt(index);
                            var users = element.Value.DistinctBy(user => user.UserId).ToList();
                            newAuthorizedUsersOfDevicesToAdd[element.Key] = users;
                        }));

                    var computeOldStateTask = Task.Run(() =>
                        Parallel.For(0, existingAuthorizedUsersOfDevicesToAdd.Count,
                            index =>
                            {
                                var element = existingAuthorizedUsersOfDevicesToAdd.ElementAt(index);
                                var users = element.Value.DistinctBy(user => user.UserId).ToList();
                                existingAuthorizedUsersOfDevicesToAdd[element.Key] = users;
                            }));

                    Task.WaitAll(computeOldStateTask, computeNewStateTask);
                });

                Task.WaitAll(computeNewAddition, computeNewDeletion);

                foreach (var deviceKey in existingAuthorizedUsersOfDevicesToDelete.Keys)
                {
                    await Task.Run(async () =>
                    {
                        var device = (await _deviceService.GetDevice(deviceKey, token: token))?.Data;
                        var usersToDeleteFromDevice = (newAuthorizedUsersOfDevicesToDelete.ContainsKey(deviceKey) && newAuthorizedUsersOfDevicesToDelete[deviceKey]?.Count > 0
                            ? existingAuthorizedUsersOfDevicesToDelete[deviceKey]
                                .ExceptBy(newAuthorizedUsersOfDevicesToDelete[deviceKey], member => member.UserId)
                            : existingAuthorizedUsersOfDevicesToDelete[deviceKey]).Select(user =>
                            new User { Code = user.UserCode, UserName = user.UserName });


                        var task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = _taskTypes.DeleteUsers,
                            Priority = _taskPriorities.Medium,
                            DeviceBrand = device.Brand,
                            TaskItems = new List<TaskItem>()
                        };

                        foreach (var userCode in usersToDeleteFromDevice.Select(user => user.Code))
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
                                OrderIndex = 1
                            });
                        }

                        await _taskService.InsertTask(task);
                        if (!allChangedDevices.Exists(deviceTemp => deviceTemp.Brand.Code == device.Brand.Code)) allChangedDevices.Add(device);

                    });
                }

                foreach (var deviceKey in newAuthorizedUsersOfDevicesToAdd.Keys)
                {
                    await Task.Run(async () =>
                    {
                        var device = (await _deviceService.GetDevice(deviceKey, token: token))?.Data;
                        var usersToAddToDevice = (existingAuthorizedUsersOfDevicesToAdd.ContainsKey(deviceKey) && existingAuthorizedUsersOfDevicesToAdd[deviceKey]?.Count > 0
                            ? newAuthorizedUsersOfDevicesToAdd[deviceKey]
                                .ExceptBy(existingAuthorizedUsersOfDevicesToAdd[deviceKey], member => member.UserId)
                            : newAuthorizedUsersOfDevicesToAdd[deviceKey]).Select(user =>
                            new User { Code = user.UserCode, UserName = user.UserName });

                        var task = new TaskInfo
                        {
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = _taskTypes.SendUsers,
                            Priority = _taskPriorities.Medium,
                            DeviceBrand = device.Brand,
                            TaskItems = new List<TaskItem>()
                        };

                        foreach (var id in usersToAddToDevice.Select(user => user.Code))
                        {
                            task.TaskItems.Add(new TaskItem
                            {
                                Status = _taskStatuses.Queued,
                                TaskItemType = _taskItemTypes.SendUser,
                                Priority = _taskPriorities.Medium,
                                DeviceId = device.DeviceId,
                                Data = JsonConvert.SerializeObject(new { UserId = id }),
                                IsParallelRestricted = true,
                                IsScheduled = false,
                                OrderIndex = 1
                            });

                        }

                        await _taskService.InsertTask(task);
                        if (!allChangedDevices.Exists(deviceTemp => deviceTemp.Brand.Code == device.Brand.Code)) allChangedDevices.Add(device);

                    });
                }
                allChangedDevices.ForEach((device) =>
                {
                    _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);
                });
                return result;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Message = exception.ToString() };
            }
        }

        public async Task<ResultViewModel> SyncUserGroupMemberBase(string listUsers, string token)
        {
            try
            {
                var xml = $"{{Users: {listUsers} }}";

                var xmlObject = JsonConvert.DeserializeXmlNode(xml, "Root");
                var firstStep = await SyncUserGroupMember(xmlObject?.OuterXml, token);

                //if (firstStep.Validate == 1)
                //{
                //    var groupUser = JsonConvert.DeserializeObject<List<UserGroupMember>>(lstUsers).GroupBy(g => g.GroupId).ToList();
                //    var count = groupUser.Count;
                //    for (int i = 0; i < count; i++)
                //    {
                //        var member = _userGroupService.GetUserGroup(groupUser[i].Key);

                //    }
                //}

                return new ResultViewModel { Validate = firstStep.Validate };
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                return new ResultViewModel { Validate = 0, Message = "SyncUserGroupMember Failed." };
            }
        }

    }
}
