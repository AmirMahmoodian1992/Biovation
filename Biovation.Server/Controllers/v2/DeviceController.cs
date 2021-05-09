using Biovation.CommonClasses.Extension;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public DeviceController(DeviceService deviceService, UserService userService, SystemInfo systemInformation, Lookups lookups, RestClient restClient, UserCardService userCardService, TaskTypes taskTypes, TaskStatuses taskStatuses, TaskItemTypes taskItemTypes, TaskPriorities taskPriorities, TaskService taskService)
        {
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
            var token = HttpContext.Items["Token"] as string;
            var result = await _deviceService.ModifyDevice(device, token);
            if (result.Validate != 1) return result;

            await Task.Run(async () =>
            {
                device = (await _deviceService.GetDevice(device.DeviceId, token)).Data;

                var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/ModifyDevice",
                    Method.POST);
                restRequest.AddJsonBody(device);
                restRequest.AddHeader("Authorization", token!);
                await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
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

        [HttpPost]
        [Authorize]
        [Route("{id}/RetrieveLogs")]
        public async Task<ResultViewModel> ReadOfflineLog([FromRoute] int id, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var token = HttpContext.Items["Token"] as string;
                var device = (await _deviceService.GetDevice(id, token)).Data;
                if (device == null)
                {
                    Logger.Log($"DeviceId {id} does not exist.");
                    return new ResultViewModel { Validate = 0, Message = $"DeviceId {id} does not exist.", Id = id };
                }

                var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/ReadOfflineOfDevice");
                restRequest.AddQueryParameter("code", device.Code.ToString());
                restRequest.AddQueryParameter("fromDate", fromDate.ToString(CultureInfo.InvariantCulture));
                restRequest.AddQueryParameter("toDate", toDate.ToString(CultureInfo.InvariantCulture));
                restRequest.AddHeader("Authorization", token!);

                var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                if (requestResult.StatusCode != HttpStatusCode.OK)
                    return new ResultViewModel
                    { Id = device.DeviceId, Validate = 0, Message = requestResult.ErrorMessage };

                var resultData = requestResult.Data;
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

        [HttpPost]
        [Authorize]
        [Route("RetrieveLogs")]
        public async Task<List<ResultViewModel>> ReadOfflineLog(string deviceIds, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var token = HttpContext.Items["Token"] as string;
            try
            {
                var deviceId = JsonConvert.DeserializeObject<int[]>(deviceIds);

                var result = new List<ResultViewModel>();
                for (var i = 0; i < deviceId.Length; i++)
                {
                    var device = (await _deviceService.GetDevice(deviceId[i], token)).Data;
                    if (device == null)
                    {
                        Logger.Log($"DeviceId {deviceId[i]} does not exist.");
                        result.Add(new ResultViewModel
                        { Validate = 0, Message = $"DeviceId {deviceId[i]} does not exist.", Id = deviceIds[i] });
                        continue;
                    }

                    var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/ReadOfflineOfDevice");
                    restRequest.AddQueryParameter("code", device.Code.ToString());
                    switch (fromDate.HasValue)
                    {
                        case false when !toDate.HasValue:
                            restRequest.AddQueryParameter("fromDate", new DateTime(1970, 1, 1).ToString(CultureInfo.InvariantCulture));
                            restRequest.AddQueryParameter("toDate", DateTime.Now.AddYears(5).ToString(CultureInfo.InvariantCulture));
                            break;
                        case false:
                            restRequest.AddQueryParameter("fromDate", new DateTime(1970, 1, 1).ToString(CultureInfo.InvariantCulture));
                            restRequest.AddQueryParameter("toDate", toDate.Value.ToString(CultureInfo.InvariantCulture));
                            break;
                        default:
                            {
                                if (toDate is null)
                                {
                                    restRequest.AddQueryParameter("toDate", DateTime.Now.AddYears(5).ToString(CultureInfo.InvariantCulture));
                                    restRequest.AddQueryParameter("fromDate", fromDate.Value.ToString(CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    restRequest.AddQueryParameter("fromDate", fromDate.Value.ToString(CultureInfo.InvariantCulture));
                                    restRequest.AddQueryParameter("toDate", toDate.Value.ToString(CultureInfo.InvariantCulture));
                                }

                                break;
                            }
                    }
                    restRequest.AddHeader("Authorization", token!);

                    var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                    if (requestResult.StatusCode == HttpStatusCode.OK)
                    {
                        var resultData = requestResult.Data;
                        resultData.Id = device.DeviceId;
                        resultData.Validate = string.IsNullOrEmpty(resultData.Message) ? 1 : resultData.Validate;
                        result.Add(resultData);
                    }
                    else
                        result.Add(new ResultViewModel { Id = device.DeviceId, Validate = 0, Message = requestResult.ErrorMessage });
                }

                return result;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = exception.Message } };
            }
        }

        [HttpPost]
        [Authorize]
        [Route("{id}/ClearLogs")]
        public async Task<ResultViewModel> ClearLogOfDevice([FromRoute] int id, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var token = HttpContext.Items["Token"] as string;
                var device = (await _deviceService.GetDevice(id, token)).Data;
                if (device == null)
                {
                    Logger.Log($"DeviceId {id} does not exist.");
                    return new ResultViewModel
                    { Validate = 0, Message = $"DeviceId {id} does not exist.", Id = id };
                }

                var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Log/ClearLog", Method.POST);
                restRequest.AddQueryParameter("code", device.Code.ToString());
                if (fromDate.HasValue) restRequest.AddQueryParameter("fromDate", fromDate.Value.ToString(CultureInfo.InvariantCulture));
                if (toDate.HasValue) restRequest.AddQueryParameter("toDate", toDate.Value.ToString(CultureInfo.InvariantCulture));
                restRequest.AddHeader("Authorization", token!);

                var restResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                if (!restResult.IsSuccessful || restResult.StatusCode != HttpStatusCode.OK)
                    return new ResultViewModel { Validate = 0, Message = "error", Id = id };
                return restResult.Data;
            }
            catch (Exception)
            {
                return new ResultViewModel { Validate = 0, Message = "error", Id = id };
            }
        }

        [HttpPost]
        [Authorize]
        [Route("ClearLogsOfDevices")]
        public async Task<List<ResultViewModel>> ClearLogOfDevice(string deviceIds, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var token = HttpContext.Items["Token"] as string;
            try
            {
                var deviceId = JsonConvert.DeserializeObject<int[]>(deviceIds);
                var result = new List<ResultViewModel>();
                for (var i = 0; i < deviceId.Length; i++)
                {
                    var device = (await _deviceService.GetDevice(deviceId[i], token)).Data;
                    if (device == null)
                    {
                        Logger.Log($"DeviceId {deviceId[i]} does not exist.");
                        result.Add(new ResultViewModel
                        { Validate = 0, Message = $"DeviceId {deviceId[i]} does not exist.", Id = deviceIds[i] });
                        continue;
                    }

                    var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Log/ClearLog", Method.POST);
                    restRequest.AddQueryParameter("code", device.Code.ToString());
                    if (fromDate.HasValue) restRequest.AddQueryParameter("fromDate", fromDate.Value.ToString(CultureInfo.InvariantCulture));
                    if (toDate.HasValue) restRequest.AddQueryParameter("toDate", toDate.Value.ToString(CultureInfo.InvariantCulture));

                    var restResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                    //var address = _localBioAddress +
                    //              $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}Log/ClearLog?code={device.Code}&fromDate={fromDate}&toDate={toDate}";
                    //var data = _restCall.CallRestAsync(address, null, null, "POST");
                    //var res = JsonConvert.DeserializeObject<ResultViewModel>(data);
                    if (!restResult.IsSuccessful || restResult.StatusCode != HttpStatusCode.OK) continue;
                    restResult.Data.Id = deviceId[i];
                    result.Add(restResult.Data);
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
        public Task<ResultViewModel> DeleteDevices([FromBody] List<uint> ids = default)
        {
            var token = HttpContext.Items["Token"] as string;
            return Task.Run(() => _deviceService.DeleteDevices(ids, token));
        }

        [HttpPost]
        [Authorize]
        [Route("{Id}/readCardNumber")]
        public async Task<ResultViewModel<int>> ReadCardNumber([FromRoute] int id = default)
        {
            return await _userCardService.ReadCardNumber(id, HttpContext.Items["Token"] as string);
        }

        [HttpGet]
        [Authorize]
        [Route("OnlineDevices")]
        public async Task<List<DeviceBasicInfo>> OnlineDevices()
        {
            var token = HttpContext.Items["Token"] as string;
            return await Task.Run(async () =>
           {
               var onlineDevices = new List<DeviceBasicInfo>();
               var deviceBrands = _systemInformation.Services;

               Parallel.ForEach(deviceBrands, async deviceBrand =>
               {
                   var restRequest =
                       new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Device/GetOnlineDevices");

                   var result = await _restClient.ExecuteAsync<List<DeviceBasicInfo>>(restRequest);

                   if (result.StatusCode != HttpStatusCode.OK) return;
                   lock (onlineDevices)
                       onlineDevices.AddRange(result.Data);
               });

               var permissibleDevices = (await _deviceService.GetDevices(token: token))?.Data?.Data;

               if (permissibleDevices != null)
               {
                   permissibleDevices = onlineDevices.Where(item => permissibleDevices.Any(dev => dev.DeviceId.Equals(item.DeviceId))).ToList();
               }

               return new List<DeviceBasicInfo>();
           });
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="deviceId"></param>
        ///// <param name="userId">Json list of userIds</param>
        ///// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("{id}/RetrieveUsers")]
        public async Task<ResultViewModel> RetrieveUserDevice([FromRoute] int id = default, [FromBody] JArray userIds = default)
        {
            if (userIds is null)
                return new ResultViewModel { Success = false, Code = 404, Message = "Empty user list provided" };

            var token = HttpContext.Items["Token"] as string;
            var device = (await _deviceService.GetDevice(id, token)).Data;

            var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveUserFromDevice", Method.POST);
            restRequest.AddHeader("Authorization", token!);
            restRequest.AddQueryParameter("code", device.Code.ToString());
            restRequest.AddJsonBody(userIds);
            var restResult = await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
            var result = restResult.Data.Any(e => e.Validate == 0)
                ? new ResultViewModel { Validate = 0, Id = id }
                : new ResultViewModel { Validate = 1, Id = id };
            return result;

            //return result.StatusCode == HttpStatusCode.OK ? result.Data : new List<ResultViewModel> { new ResultViewModel { Id = id, Validate = 0, Message = result.ErrorMessage } };
        }

        [HttpPost]
        [Authorize]
        [Route("{id}/FetchUsersList")]
        public async Task<List<User>> RetrieveUsersOfDevice([FromRoute] int id = default)
        {
            var token = HttpContext.Items["Token"] as string;
            var device = (await _deviceService.GetDevice(id, token)).Data;
            var userAwaiter = _userService.GetUsers(token: token);

            var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveUsersListFromDevice");
            restRequest.AddQueryParameter("code", device.Code.ToString());
            restRequest.AddHeader("Authorization", token!);

            var restAwaiter = _restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest);

            var result = await restAwaiter;
            var users = (await userAwaiter)?.Data?.Data;

            var usersOfDevice = (from r in result.Data?.Data
                                 join u in users on r.Code equals u.Code
                                     into ps
                                 from u in ps.DefaultIfEmpty()
                                 select new User
                                 {
                                     Type = u == null ? 0 : 1,
                                     IsActive = r.IsActive,
                                     Id = r.Id,
                                     Code = r.Code,
                                     FullName = u != null ? u.FirstName + " " + u.SurName : r.UserName,
                                     StartDate = u?.StartDate ?? new DateTime(1990, 1, 1),
                                     EndDate = u?.EndDate ?? new DateTime(2050, 1, 1)
                                 }).ToList();

            return usersOfDevice;
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}/RemoveUser/{userId}")]
        public async Task<ResultViewModel> RemoveUserFromDevice([FromRoute] int id = default, [FromRoute] int userId = default)
        {
            var token = HttpContext.Items["Token"] as string;
               if (userId == default)
                   return new ResultViewModel { Validate = 0, Message = "No users selected." };

               var device = (await _deviceService.GetDevice(id, token)).Data;

               var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/DeleteUserFromDevice", Method.POST);
               restRequest.AddQueryParameter("code", device.Code.ToString());
               restRequest.AddJsonBody(userId);
               restRequest.AddHeader("Authorization", token!);
               return (await _restClient.ExecuteAsync<ResultViewModel>(restRequest)).Data;
        }

        [HttpPost]
        [Authorize]
        [Route("{id}/SendUsers")]
        public async Task<ResultViewModel> SendUsersToDevice([FromRoute] int id = default)
        {
            var token = HttpContext.Items["Token"] as string;
            try
            {
                var device = (await _deviceService.GetDevice(id, token)).Data;
                if (device == null)
                {
                    Logger.Log($"DeviceId {id} does not exist.");
                    return new ResultViewModel { Validate = 0, Message = $"DeviceId {id} does not exist." };
                }

                var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/SendUsersOfDevice", Method.POST);
                restRequest.AddJsonBody(device);
                restRequest.AddHeader("Authorization", token!);
                var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                return new ResultViewModel { Validate = result.StatusCode == HttpStatusCode.OK ? 1 : 0, Id = id };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Validate = 0, Message = $"SendUserToDevice Failed. DeviceId: {id}" };
            }
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("{id}/DeviceInfo")]
        public async Task<ResultViewModel<Dictionary<string, string>>> DeviceInfo([FromRoute] int id = default)
        {
            var token = HttpContext.Items["Token"] as string;
            var device = (await _deviceService.GetDevice(id, token)).Data;

            var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/GetAdditionalData");
            restRequest.AddQueryParameter("code", device.Code.ToString());
            restRequest.AddHeader("Authorization", token!);
            var result = await _restClient.ExecuteAsync<Dictionary<string, string>>(restRequest);

            return new ResultViewModel<Dictionary<string, string>>
            {
                Success = result.StatusCode == HttpStatusCode.OK,
                Data = result.Data,
                Id = id
            };
        }

        [HttpGet]
        [Route("DeviceBrands")]
        public async Task<ResultViewModel<PagingResult<Lookup>>> DeviceBrands(bool loadedOnly = true)
        {
            if (!loadedOnly) return await _deviceService.GetDeviceBrands();
            var loadedServices = _systemInformation.Services.Select(brand => _lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Name, brand.Name))).ToList();
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
        [Route("DeviceModels")]
        public Task<ResultViewModel<PagingResult<DeviceModel>>> DeviceModels(int brandCode = default, string name = default, bool loadedBrandsOnly = true)
        {
            return Task.Run(() =>
            {
                var deviceModels = _deviceService.GetDeviceModels(brandId: brandCode, name: name);
                if (!loadedBrandsOnly) return deviceModels;

                var loadedDeviceModels = deviceModels.Data.Data?.Where(dm => _systemInformation.Services.Any(db =>
                    string.Equals(dm.Brand.Name, db.Name, StringComparison.InvariantCultureIgnoreCase))).ToList();

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
            });
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
        //                    var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/UpgradeFirmware", Method.POST, DataFormat.Json);
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

                _taskService.InsertTask(task);

                var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Task/RunProcessQueue", Method.POST);
                await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

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