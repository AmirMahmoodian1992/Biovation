using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Logger = Biovation.CommonClasses.Logger;

namespace Biovation.Server.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly AccessGroupService _accessGroupService;
        private readonly DeviceService _deviceService;
        private readonly UserCardService _userCardService;
        private readonly UserService _userService;
        private readonly RestClient _restClient;
        private readonly SystemInfo _systemInformation;
        private readonly Lookups _lookups;

        private readonly TaskTypes _taskTypes;
        private readonly TaskService _taskService;
        private readonly TaskStatuses _taskStatuses;
        private readonly TaskItemTypes _taskItemTypes;
        private readonly TaskPriorities _taskPriorities;

        public DeviceController(AccessGroupService accessGroupService, DeviceService deviceService, UserService userService, SystemInfo systemInformation, Lookups lookups, RestClient restClient, UserCardService userCardService, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, TaskService taskService)
        {
            _accessGroupService = accessGroupService;
            _deviceService = deviceService;
            _userService = userService;
            _systemInformation = systemInformation;
            _lookups = lookups;
            _restClient = restClient;
            _userCardService = userCardService;

            _taskTypes = taskTypes;
            _taskStatuses = taskStatuses;
            _taskItemTypes = taskItemTypes;
            _taskPriorities = taskPriorities;
            _taskService = taskService;
        }


        [HttpGet]
        [Route("{id:int}")]
        [Authorize]
        public async Task<ResultViewModel<DeviceBasicInfo>> Device([FromRoute] long id = default)
        {
            return await _deviceService.GetDevice(id, HttpContext.Items["Token"] as string);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> Devices(int groupId = default, uint code = default,
            int brandId = default, string name = null, int modelId = default, int deviceIoTypeId = default, int pageNumber = default, int pageSize = default)
        {
            return await _deviceService.GetDevices(groupId, code, brandId.ToString(), name,
                modelId, deviceIoTypeId, pageNumber, pageSize, HttpContext.Items["Token"] as string);
        }

        [HttpPut]
        [Authorize]
        public async Task<ResultViewModel> ModifyDeviceInfo([FromBody] DeviceBasicInfo device)
        {
            var creatorUser = HttpContext.GetUser();
            var token = HttpContext.Items["Token"] as string;
            var result = await _deviceService.ModifyDevice(device, token);
            if (result.Validate != 1) return result;

            _ = Task.Run(async () =>
            {
                device = (await _deviceService.GetDevice(device.DeviceId, token)).Data;
                if (device.Active)
                {
                    var task = new TaskInfo
                    {
                        Status = _taskStatuses.Queued,
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.UnlockDevice,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = device.Brand,
                        TaskItems = new List<TaskItem>()
                    };
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.UnlockDevice,
                        Priority = _taskPriorities.Medium,

                        DeviceId = device.DeviceId,
                        Data = JsonConvert.SerializeObject(device.DeviceId),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1
                    });
                    await _taskService.InsertTask(task);
                    _ = _taskService.ProcessQueue(device.Brand, device.DeviceId).ConfigureAwait(false);
                }
                else
                {
                    var task = new TaskInfo
                    {
                        Status = _taskStatuses.Queued,
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.LockDevice,
                        Priority = _taskPriorities.Medium,
                        TaskItems = new List<TaskItem>(),
                        DeviceBrand = device.Brand
                    };
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.LockDevice,
                        Priority = _taskPriorities.Medium,

                        DeviceId = device.DeviceId,
                        Data = JsonConvert.SerializeObject(device.DeviceId),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1,

                    });
                    await _taskService.InsertTask(task);
                    _ = _taskService.ProcessQueue(device.Brand, device.DeviceId).ConfigureAwait(false);
                }

                _deviceService.ModifyDeviceInfo(device, token);
                //var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/ModifyDevice",
                //    Method.POST);
                //restRequest.AddJsonBody(device);
                //restRequest.AddHeader("Authorization", token!);
                //await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            }).ConfigureAwait(false);

            return result;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> AddDevice([FromBody] DeviceBasicInfo device = default)
        {
            return await _deviceService.AddDevice(device, HttpContext.Items["Token"] as string);
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public async Task<ResultViewModel> DeleteDevice([FromRoute] uint id = default)
        {
            return await _deviceService.DeleteDevice(id, HttpContext.Items["Token"] as string);
        }

        // TODO - Verify Method.
        [HttpPost]
        [Authorize]
        [Route("{id}/RetrieveLogs")]
        public async Task<ResultViewModel> ReadOfflineLog([FromRoute] int id, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var creatorUser = HttpContext.GetUser();
                var token = HttpContext.Items["Token"] as string;
                var device = (await _deviceService.GetDevice(id, token)).Data;
                if (device == null)
                {
                    Logger.Log($"DeviceId {id} does not exist.");
                    return new ResultViewModel { Validate = 0, Message = $"DeviceId {id} does not exist.", Id = id };
                }

                if (fromDate.HasValue && toDate.HasValue)
                {
                    var task = new TaskInfo
                    {
                        Status = _taskStatuses.Queued,
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.GetLogsInPeriod,
                        Priority = _taskPriorities.Medium,
                        TaskItems = new List<TaskItem>(),
                        DeviceBrand = device.Brand
                    };

                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.GetLogsInPeriod,
                        Priority = _taskPriorities.Medium,
                        DeviceId = device.DeviceId,
                        Data = JsonConvert.SerializeObject(new { fromDate, toDate }),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1,
                    });
                    await _taskService.InsertTask(task);
                    _ = _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);
                }
                else
                {
                    var task = new TaskInfo
                    {
                        Status = _taskStatuses.Queued,
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.GetLogs,
                        Priority = _taskPriorities.Medium,
                        TaskItems = new List<TaskItem>(),
                        DeviceBrand = device.Brand,
                    };

                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.GetLogs,
                        Priority = _taskPriorities.Medium,

                        DeviceId = device.DeviceId,
                        Data = JsonConvert.SerializeObject(device.DeviceId),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1
                    });
                    await _taskService.InsertTask(task);
                    _ = _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);
                }

                var readOfflineLogResult = _deviceService.ReadOfflineOfDevice(device, fromDate, toDate, token);

                if (readOfflineLogResult.StatusCode != HttpStatusCode.OK)
                    return new ResultViewModel
                    { Id = device.DeviceId, Validate = 0, Message = readOfflineLogResult.ErrorMessage };

                var resultData = readOfflineLogResult.Data;
                resultData.Id = device.DeviceId;
                resultData.Validate = string.IsNullOrEmpty(resultData.Message) ? 1 : resultData.Validate;
                return resultData;

            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Message = exception.Message };
            }
        }


        // TODO - Verify method.
        [HttpPost]
        [Authorize]
        [Route("RetrieveLogs")]
        public async Task<List<ResultViewModel>> ReadOfflineLog(string ids, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var creatorUser = HttpContext.GetUser();
                var token = HttpContext.Items["Token"] as string;
                var deviceIds = JsonConvert.DeserializeObject<int[]>(ids);

                var result = new List<ResultViewModel>();
                for (var i = 0; i < deviceIds.Length; i++)
                {
                    var device = (await _deviceService.GetDevice(deviceIds[i], token)).Data;
                    if (device == null)
                    {
                        Logger.Log($"DeviceId {deviceIds[i]} does not exist.");
                        result.Add(new ResultViewModel
                        { Validate = 0, Message = $"DeviceId {deviceIds[i]} does not exist.", Id = ids[i] });
                        continue;
                    }

                    if (fromDate.HasValue && toDate.HasValue)
                    {
                        var task = new TaskInfo
                        {
                            Status = _taskStatuses.Queued,
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = _taskTypes.GetLogsInPeriod,
                            Priority = _taskPriorities.Medium,
                            TaskItems = new List<TaskItem>(),
                            DeviceBrand = device.Brand,
                        };

                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.GetLogsInPeriod,
                            Priority = _taskPriorities.Medium,
                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(new { fromDate, toDate }),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1,
                        });
                        await _taskService.InsertTask(task);
                        _ = _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);
                    }
                    else
                    {
                        var task = new TaskInfo
                        {
                            Status = _taskStatuses.Queued,
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = creatorUser,
                            TaskType = _taskTypes.GetLogs,
                            Priority = _taskPriorities.Medium,
                            TaskItems = new List<TaskItem>(),
                            DeviceBrand = device.Brand,
                        };

                        task.TaskItems.Add(new TaskItem
                        {
                            Status = _taskStatuses.Queued,
                            TaskItemType = _taskItemTypes.GetLogs,
                            Priority = _taskPriorities.Medium,

                            DeviceId = device.DeviceId,
                            Data = JsonConvert.SerializeObject(device.DeviceId),
                            IsParallelRestricted = true,
                            IsScheduled = false,
                            OrderIndex = 1
                        });
                        await _taskService.InsertTask(task);
                        _ = _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);
                    }

                    var readOfflineLogResult = _deviceService.ReadOfflineOfDevice(device, fromDate, toDate, token);

                    //var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/ReadOfflineOfDevice");
                    //restRequest.AddQueryParameter("code", device.Code.ToString());
                    //restRequest.AddQueryParameter("fromDate", fromDate);
                    //restRequest.AddQueryParameter("toDate", toDate);
                    //if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
                    //{
                    //    restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                    //}
                    //var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                    if (readOfflineLogResult.StatusCode == HttpStatusCode.OK)
                    {
                        var resultData = readOfflineLogResult.Data;
                        resultData.Id = device.DeviceId;
                        resultData.Validate = string.IsNullOrEmpty(resultData.Message) ? 1 : resultData.Validate;
                        result.Add(resultData);
                    }
                    else
                        result.Add(new ResultViewModel { Id = device.DeviceId, Validate = 0, Message = readOfflineLogResult.ErrorMessage });
                }

                return result;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = exception.Message } };
            }
        }


        // TODO - Verify Method.
        [HttpPost]
        [Authorize]
        [Route("{id}/ClearLogs")]
        public async Task<ResultViewModel> ClearLogOfDevice([FromRoute] int id, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var creatorUser = HttpContext.GetUser();
                var token = HttpContext.Items["Token"] as string;
                var device = (await _deviceService.GetDevice(id, token)).Data;
                if (device == null)
                {
                    Logger.Log($"DeviceId {id} does not exist.");
                    return new ResultViewModel
                    { Validate = 0, Message = $"DeviceId {id} does not exist.", Id = id };
                }

                var task = new TaskInfo
                {
                    Status = _taskStatuses.Queued,
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = _taskTypes.ClearLog,
                    Priority = _taskPriorities.Medium,
                    DeviceBrand = device.Brand,
                    TaskItems = new List<TaskItem>()
                };
                task.TaskItems.Add(new TaskItem
                {
                    Status = _taskStatuses.Queued,
                    TaskItemType = _taskItemTypes.ClearLog,
                    Priority = _taskPriorities.Medium,
                    DeviceId = device.DeviceId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        fromDate,
                        toDate
                    }),
                    IsParallelRestricted = true,
                    IsScheduled = false,
                    OrderIndex = 1
                });

                await _taskService.InsertTask(task);
                _ = _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);

                var clearLogOfDeviceResult = _deviceService.ClearLogOfDevice(device, fromDate, toDate, token);

                if (!clearLogOfDeviceResult.IsSuccessful || clearLogOfDeviceResult.StatusCode != HttpStatusCode.OK)
                    return new ResultViewModel { Validate = 0, Message = "error", Id = id };
                return clearLogOfDeviceResult.Data;
            }
            catch (Exception)
            {
                return new ResultViewModel { Validate = 0, Message = "error", Id = id };
            }
        }


        // TODO - Verify Method.
        [HttpPost]
        [Authorize]
        [Route("ClearLogsOfDevices")]
        public async Task<List<ResultViewModel>> ClearLogOfDevice(string ids, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var creatorUser = HttpContext.GetUser();
                var token = HttpContext.Items["Token"] as string;
                var deviceIds = JsonConvert.DeserializeObject<int[]>(ids);
                var result = new List<ResultViewModel>();
                for (var i = 0; i < deviceIds.Length; i++)
                {
                    var device = (await _deviceService.GetDevice(deviceIds[i], token)).Data;
                    if (device == null)
                    {
                        Logger.Log($"DeviceId {deviceIds[i]} does not exist.");
                        result.Add(new ResultViewModel
                        { Validate = 0, Message = $"DeviceId {deviceIds[i]} does not exist.", Id = ids[i] });
                        continue;
                    }

                    var task = new TaskInfo
                    {
                        Status = _taskStatuses.Queued,
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.ClearLog,
                        Priority = _taskPriorities.Medium,
                        DeviceBrand = device.Brand,
                        TaskItems = new List<TaskItem>()
                    };
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.ClearLog,
                        Priority = _taskPriorities.Medium,
                        DeviceId = device.DeviceId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            fromDate,
                            toDate
                        }),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1
                    });

                    await _taskService.InsertTask(task);
                    _ = _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);

                    var clearLogResult = _deviceService.ClearLogOfDevice(device, fromDate, toDate, token);

                    //var restResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                    //var address = _localBioAddress +
                    //              $"/biovation/api/{device.ServiceInstance.Id}/Log/ClearLog?code={device.Code}&fromDate={fromDate}&toDate={toDate}";
                    //var data = _restCall.CallRestAsync(address, null, null, "POST");
                    //var res = JsonConvert.DeserializeObject<ResultViewModel>(data);
                    if (!clearLogResult.IsSuccessful || clearLogResult.StatusCode != HttpStatusCode.OK) continue;
                    clearLogResult.Data.Id = ids[i];
                    result.Add(clearLogResult.Data);
                }

                return result;
            }
            catch (Exception)
            {
                return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = "error" } };
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("DeleteDevices")]
        public async Task<ResultViewModel> DeleteDevices([FromBody] List<uint> ids = default)
        {
            return await _deviceService.DeleteDevices(ids, HttpContext.Items["Token"] as string);
        }

        [HttpPost]
        [Authorize]
        [Route("{Id}/ReadCardNumber")]
        public async Task<ResultViewModel<int>> ReadCardNumber([FromRoute] int id = default)
        {
            return await _userCardService.ReadCardNumber(id, HttpContext.Items["Token"] as string);
        }


        // TODO - Verify the method.
        [HttpGet]
        [Authorize]
        [Route("OnlineDevices")]
        public async Task<List<DeviceBasicInfo>> OnlineDevices()
        {
            var token = (string)HttpContext.Items["Token"];
            return await Task.Run(() => _deviceService.GetOnlineDevices(token));
        }

        // TODO - Verify method.
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="deviceId"></param>
        ///// <param name="userId">Json list of userIds</param>
        ///// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("{id}/RetrieveUsers")]
        public async Task<List<ResultViewModel>> RetrieveUserDevice([FromRoute] int id = default, [FromBody] List<uint> userIds = default)
        {
            var creatorUser = HttpContext.GetUser();
            if (userIds is null)
                return new List<ResultViewModel>
                    {new ResultViewModel {Success = false, Code = 404, Message = "Empty user list provided"}};

            var token = HttpContext.Items["Token"] as string;
            var device = (await _deviceService.GetDevice(id, token)).Data;

            var task = new TaskInfo
            {
                Status = _taskStatuses.Queued,
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = creatorUser,
                DeviceBrand = device.Brand,
                TaskType = _taskTypes.RetrieveUserFromTerminal,
                Priority = _taskPriorities.Medium,
                TaskItems = new List<TaskItem>()
            };
            foreach (var numericUserId in userIds)
            {
                task.TaskItems.Add(new TaskItem
                {
                    Status = _taskStatuses.Queued,
                    TaskItemType = _taskItemTypes.RetrieveUserFromTerminal,
                    Priority = _taskPriorities.Medium,
                    DeviceId = device.DeviceId,
                    Data = JsonConvert.SerializeObject(new { userId = numericUserId }),
                    IsParallelRestricted = true,
                    IsScheduled = false,
                    OrderIndex = 1
                });
            }
            await _taskService.InsertTask(task);
            _ = _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);

            var restResult = _deviceService.RetrieveUsers(device, userIds, token);

            //var result = restResult.Data.Any(e => e.Validate == 0)
            //    ? new ResultViewModel { Validate = 0, Id = id }
            //    : new ResultViewModel { Validate = 1, Id = id };
            //return result;

            return restResult.StatusCode == HttpStatusCode.OK ? restResult.Data : new List<ResultViewModel> { new ResultViewModel { Id = id, Validate = 0, Message = restResult.ErrorMessage } };
        }

        //TODO - Verify the method.
        [HttpPost]
        [Authorize]
        [Route("{id}/FetchUsersList")]
        public async Task<List<User>> RetrieveUsersOfDevice([FromRoute] int id = default)
        {
            var token = HttpContext.Items["Token"] as string;
            var device = (await _deviceService.GetDevice(id, token)).Data;
            var userAwaiter = _userService.GetUsers(token: token);

            //var restRequest = new RestRequest($"{device.ServiceInstance.Id}/Device/RetrieveUsersListFromDevice");
            //restRequest.AddQueryParameter("code", device.Code.ToString());
            //if (HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
            //{
            //    restRequest.AddHeader("Authorization", HttpContext.Request.Headers["Authorization"].FirstOrDefault());
            //}
            //var restAwaiter = _restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest);

            //var result = await restAwaiter;

            var users = await userAwaiter;

            var lastResult = _deviceService.RetrieveUsersOfDevice(device, users?.Data?.Data, token);

            //var lstResult = (from r in usersResult?.Data
            //                 join u in users on r.Code equals u.Code
            //                     into ps
            //                 from u in ps.DefaultIfEmpty()
            //                 select new User
            //                 {
            //                     Type = u == null ? 0 : 1,
            //                     IsActive = r.IsActive,
            //                     Id = r.Id,
            //                     Code = r.Code,
            //                     FullName = u != null ? u.FirstName + " " + u.SurName : r.UserName,
            //                     StartDate = u?.StartDate ?? new DateTime(1990, 1, 1),
            //                     EndDate = u?.EndDate ?? new DateTime(2050, 1, 1)
            //                 }).ToList();

            return lastResult?.Data;
        }


        // TODO - Verify the method.
        [HttpPost]
        [Authorize]
        [Route("{id}/SendUsers")]
        public async Task<ResultViewModel> SendUsersToDevice([FromRoute] int id, [FromBody] List<long> userIds)
        {
            try
            {
                var creatorUser = HttpContext.GetUser();
                var token = HttpContext.Items["Token"] as string;
                if (userIds == null || !userIds.Any())
                {
                    return new ResultViewModel { Validate = 0, Message = "User list is empty" };
                }

                var result = new List<ResultViewModel>();
                if (id == 0)
                    return
                        result.Any(e => e.Success == false)
                            ? new ResultViewModel { Validate = 0, Message = "" }
                            : new ResultViewModel { Validate = 1, Message = "Failed to send all of them" };

                var device = (await _deviceService.GetDevice(id, token))?.Data;
                if (device == null)
                {
                    var msg = "DeviceId " + id + " does not exist.";
                    Logger.Log(msg);
                    return new ResultViewModel { Validate = 0, Message = msg };
                }

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

                foreach (var userId in userIds)
                {
                    task.TaskItems.Add(new TaskItem
                    {
                        Status = _taskStatuses.Queued,
                        TaskItemType = _taskItemTypes.SendUser,
                        Priority = _taskPriorities.Medium,
                        DeviceId = device.DeviceId,
                        Data = JsonConvert.SerializeObject(new { UserId = userId }),
                        IsParallelRestricted = true,
                        IsScheduled = false,
                        OrderIndex = 1,
                        CurrentIndex = 0,
                        TotalCount = 1
                    });
                }

                await _taskService.InsertTask(task);
                _ = _taskService.ProcessQueue(device.Brand, device.DeviceId);

                var requestResult = await _deviceService.SendUserToDevice(device, userIds, token);

                //var restRequest =
                //    new RestRequest(
                //        $"/{device.ServiceInstance.Id}/User/SendUserToDevice",
                //        Method.GET);
                //restRequest.AddQueryParameter("code", device.Code.ToString());
                //restRequest.AddQueryParameter("userId", JsonSerializer.Serialize(userIds));
                //restRequest.AddHeader("Authorization", token!);
                //var requestResult = await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);

                if (requestResult.IsSuccessful && requestResult.StatusCode == HttpStatusCode.OK && requestResult.Data != null)
                    result.AddRange(requestResult.Data);

                return result.Any(e => e.Success == false) ? new ResultViewModel { Validate = 0, Message = "" } : new ResultViewModel { Validate = 1, Message = "Failed to send all of them" };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Message = "SendUserToDevice Failed." };
            }
        }


        // TODO - Verify Method
        [HttpDelete]
        [Authorize]
        [Route("{id}/RemoveUser/{userId}")]
        public async Task<ResultViewModel> RemoveUserFromDevice([FromRoute] int id = default, [FromRoute] long userId = default)
        {
            var creatorUser = HttpContext.GetUser();
            var token = HttpContext.Items["Token"] as string;
            if (userId == default)
                return new ResultViewModel { Validate = 0, Message = "No users selected." };

            var device = (await _deviceService.GetDevice(id, token)).Data;

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

            task.TaskItems.Add(new TaskItem
            {
                Status = _taskStatuses.Queued,
                TaskItemType = _taskItemTypes.DeleteUserFromTerminal,
                Priority = _taskPriorities.Medium,
                DeviceId = device.DeviceId,
                Data = JsonConvert.SerializeObject(new { userId }),
                IsParallelRestricted = true,
                IsScheduled = false,
                OrderIndex = 1,
                CurrentIndex = 0,
                TotalCount = 1
            });

            await _taskService.InsertTask(task);
            _ = _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);

            var restResult = _deviceService.RemoveUserFromDeviceById(device, userId, token);
            return restResult;
        }

        // TODO - Verify the method.
        [HttpPost]
        [Authorize]
        [Route("{id}/RemoveUsers")]
        public async Task<ResultViewModel> RemoveUsersFromDevice([FromBody] List<long> userIds, [FromRoute] int id = default)
        {
            var creatorUser = HttpContext.GetUser();
            var token = HttpContext.Items["Token"] as string;
            if (userIds is null || !userIds.Any())
                return new ResultViewModel { Validate = 0, Message = "No users selected." };

            var device = (await _deviceService.GetDevice(id, token)).Data;

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

            foreach (var userCode in userIds)
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
            _ = _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);

            var res = _deviceService.DeleteUserFromDevice(device, userIds, token);

            return res;

            //var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/DeleteUserFromDevice", Method.POST);
            //restRequest.AddQueryParameter("code", device.Code.ToString());
            //restRequest.AddJsonBody(userIds);
            //restRequest.AddHeader("Authorization", token!);
            //return (await _restClient.ExecuteAsync<ResultViewModel>(restRequest)).Data;
        }


        // TODO - Verify the method.
        [HttpPost]
        [Authorize]
        [Route("{id}/SyncUsers")]
        public async Task<ResultViewModel> SendUsersToDevice([FromRoute] int id = default)
        {
            try
            {
                var creatorUser = HttpContext.GetUser();
                var token = HttpContext.Items["Token"] as string;
                var device = (await _deviceService.GetDevice(id, token)).Data;
                if (device == null)
                {
                    Logger.Log($"DeviceId {id} does not exist.");
                    return new ResultViewModel { Validate = 0, Message = $"DeviceId {id} does not exist." };
                }

                var task = new TaskInfo
                {
                    Status = _taskStatuses.Queued,
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    DeviceBrand = device.Brand,
                    TaskType = _taskTypes.SendUsers,
                    Priority = _taskPriorities.Medium,
                    TaskItems = new List<TaskItem>(),
                    DueDate = DateTime.Today
                };
                var accessGroups = (await _accessGroupService.GetAccessGroups(deviceId: device.DeviceId))?.Data?.Data ?? new List<AccessGroup>();

                foreach (var accessGroup in accessGroups)
                {
                    foreach (var userGroup in accessGroup.UserGroup)
                    {
                        foreach (var userGroupMember in userGroup.Users)
                        {
                            task.TaskItems.Add(new TaskItem
                            {
                                Status = _taskStatuses.Queued,
                                TaskItemType = _taskItemTypes.SendUser,
                                Priority = _taskPriorities.Medium,
                                DeviceId = device.DeviceId,
                                Data = JsonConvert.SerializeObject(new { userId = userGroupMember.UserCode }),
                                IsParallelRestricted = true,
                                IsScheduled = false,
                                OrderIndex = 1,
                                TotalCount = 1,
                                CurrentIndex = 0
                            });
                        }
                    }
                }

                await _taskService.InsertTask(task);
                _ = _taskService.ProcessQueue(device.Brand).ConfigureAwait(false);

                var result = await _deviceService.SendUsersOfDevice(device, token);

                //var restRequest = new RestRequest($"{device.ServiceInstance.Id}/Device/SendUsersOfDevice", Method.POST);
                //restRequest.AddJsonBody(device);
                //restRequest.AddHeader("Authorization", token!);
                //var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                return new ResultViewModel { Validate = result.StatusCode == HttpStatusCode.OK ? 1 : 0, Id = id };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Message = $"SendUserToDevice Failed. DeviceId: {id}" };
            }
        }


        // TODO - Verify the method.
        [HttpGet]
        [AllowAnonymous]
        [Route("{id}/DeviceInfo")]
        public async Task<ResultViewModel<Dictionary<string, string>>> DeviceInfo([FromRoute] int id = default)
        {
            var token = HttpContext.Items["Token"] as string;
            var device = (await _deviceService.GetDevice(id, token)).Data;

            var result = await _deviceService.GetAdditionalData(device, token);

            //var restRequest = new RestRequest($"{device.ServiceInstance.Id}/Device/GetAdditionalData");
            //restRequest.AddQueryParameter("code", device.Code.ToString());
            //restRequest.AddHeader("Authorization", token!);
            //var result = await _restClient.ExecuteAsync<Dictionary<string, string>>(restRequest);

            return new ResultViewModel<Dictionary<string, string>>
            {
                Success = result.StatusCode == HttpStatusCode.OK,
                Data = result.Data,
                Id = id
            };
        }

        [HttpGet]
        [Authorize]
        [Route("DeviceBrands")]
        public async Task<ResultViewModel<PagingResult<Lookup>>> DeviceBrands(bool loadedOnly = true)
        {
            if (!loadedOnly) return await _deviceService.GetDeviceBrands();
            //var loadedServices = _systemInformation.Services.Select(brand => _lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Name, brand.Name))).ToList();
            var loadedServices = _lookups.DeviceBrands;
            return new ResultViewModel<PagingResult<Lookup>>
            {
                Success = true,
                Validate = 1,
                Code = 200,
                Data = new PagingResult<Lookup>
                {
                    Count = loadedServices.Count,
                    From = 0,
                    PageNumber = 0,
                    PageSize = loadedServices.Count,
                    Data = loadedServices
                }
            };
        }

        [HttpGet]
        [Authorize]
        [Route("DeviceModels")]
        public async Task<ResultViewModel<PagingResult<DeviceModel>>> DeviceModels(int brandCode = default, string name = default, bool loadedBrandsOnly = true)
        {
            var deviceModels = await _deviceService.GetDeviceModels(brandId: brandCode, name: name);
            if (!loadedBrandsOnly) return deviceModels;

            var loadedDeviceModels = deviceModels.Data.Data?.Where(dm => _systemInformation.Services.Any(db =>
                string.Equals(dm.Brand.Name, db.Name, StringComparison.InvariantCultureIgnoreCase))).ToList();

            if (loadedDeviceModels != null && loadedDeviceModels.Count == 0)
            {
                loadedDeviceModels = deviceModels?.Data?.Data;
            }
            return new ResultViewModel<PagingResult<DeviceModel>>
            {
                Success = true,
                Validate = 1,
                Code = 200,
                Data = new PagingResult<DeviceModel>
                {
                    Count = loadedDeviceModels?.Count ?? 0,
                    From = 0,
                    PageNumber = 0,
                    PageSize = loadedDeviceModels?.Count ?? 0,
                    Data = loadedDeviceModels
                }
            };
        }

        //TODO make compatible with.net core
        //[HttpPost]
        //[Route("UpgradeFirmware")]
        //public Task<ResultViewModel> UpgradeFirmware(int deviceId)
        //{
        //    return Task.Run(async () =>
        //    {
        //        if (!Request.Content.IsMimeMultipartContent())
        //            return new ResultViewModel { Validate = 0, Code = 415, Message = "UnsupportedMediaType" };

        //        try
        //        {
        //            var device = _deviceService.GetDeviceInfo(deviceId);

        //            if (device is null)
        //                return new ResultViewModel
        //                { Validate = 0, Code = 400, Id = deviceId, Message = "Wrong device id provided" };

        //            var multipartMemory = await Request.Content.ReadAsMultipartAsync();

        //            foreach (var multipartContent in multipartMemory.Contents)
        //            {
        //                try
        //                {
        //                    var restRequest = new RestRequest($"{device.ServiceInstance.Id}/Device/UpgradeFirmware", Method.POST, DataFormat.Json);
        //                    restRequest.AddHeader("Content-Type", "multipart/form-data");
        //                    restRequest.AddQueryParameter("deviceCode", device.Code.ToString());
        //                    restRequest.AddFile(multipartContent.Headers.ContentDisposition.Name.Trim('\"'),
        //                        multipartContent.ReadAsByteArrayAsync().Result,
        //                        multipartContent.Headers.ContentDisposition.FileName.Trim('\"'),
        //                        multipartContent.Headers.ContentType.MediaType);
        //                    var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
        //                    if (!result.IsSuccessful || result.Data.Validate == 0)
        //                        return result.Data;
        //                }
        //                catch (Exception exception)
        //                {
        //                    Logger.Log(exception, logType: LogType.Debug);
        //                }
        //            }
        //        }
        //        catch (Exception exception)
        //        {
        //            Logger.Log(exception, logType: LogType.Debug);
        //            throw;
        //        }

        //        return new ResultViewModel { Validate = 1, Code = 200, Id = deviceId, Message = "Files uploaded and upgrading firmware started." };
        //    });
        //}

        // TODO - Verify the method.
        [HttpPost]
        [Authorize]
        //[AllowAnonymous]
        [Route("{id}/UserAdaptation")]
        public Task<ResultViewModel> UserAdaptation([FromRoute] int id, [FromBody] object equivalentCodesObject)
        {
            var token = HttpContext.Items["Token"] as string;
            var creatorUser = HttpContext.GetUser();

            return Task.Run(async () =>
            {
                var serializedEquivalentCodes = JsonSerializer.Serialize(equivalentCodesObject);
                var device = (await _deviceService.GetDevice(id)).Data;


                var task = new TaskInfo
                {
                    Status = _taskStatuses.Queued,
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = _taskTypes.UserAdaptation,
                    Priority = _taskPriorities.Medium,
                    DeviceBrand = device.Brand,
                    TaskItems = new List<TaskItem>(),
                    DueDate = DateTime.Today
                };
                task.TaskItems.Add(new TaskItem
                {
                    Status = _taskStatuses.Queued,
                    TaskItemType = _taskItemTypes.UserAdaptation,
                    Priority = _taskPriorities.Medium,
                    DeviceId = device.DeviceId,
                    Data = JsonConvert.SerializeObject(new { serializedEquivalentCodes, token, creatorUserId = creatorUser.Id }),
                    IsParallelRestricted = true,
                    IsScheduled = false,
                    OrderIndex = 1,
                    CurrentIndex = 0,
                    TotalCount = 1
                });

                await _taskService.InsertTask(task);
                _ = _taskService.ProcessQueue(device.Brand, device.DeviceId).ConfigureAwait(false);

                //var restRequest = new RestRequest($"{device.ServiceInstance.Id}/Task/RunProcessQueue", Method.POST);
                //await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                return new ResultViewModel { Success = true, Message = "The requested operation successfully started" };
            });
        }


        ///////////////////////////////////
        //[HttpGet]
        //[Route("GetDeviceModels/{id}")]
        //public Task<PagingResult<DeviceModel>> DeviceModels(int id = default, int brandId = default, string name = default, int pageNumber = default, int PageSize = default)
        //{
        //    var result = Task.Run(() => _deviceService.GetDeviceModels(id, brandId, name, pageNumber, PageSize));
        //    return result;
        //}


        //[HttpGet]
        //[Route("BioAuthMode/{id}")]
        //public Task<ResultViewModel<AuthModeMap>> GetBioAuthModeWithDeviceId(int id = default, int authMode = default)
        //{
        //    var result = Task.Run(() => _deviceService.GetBioAuthModeWithDeviceId(id, authMode));
        //    return result;
        //}
        //////////////////////////////////////////
    }
}