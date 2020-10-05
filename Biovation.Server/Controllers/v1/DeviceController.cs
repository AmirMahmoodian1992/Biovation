using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v1
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class DeviceController : Controller
    {
        private readonly Lookups _lookups;
        private readonly RestClient _restClient;
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;
        private readonly SystemInfo _systemInformation;

        public DeviceController(RestClient restClient, DeviceService deviceService, UserService userService, Lookups lookups, SystemInfo systemInformation)
        {
            _lookups = lookups;
            _restClient = restClient;
            _userService = userService;
            _deviceService = deviceService;
            _systemInformation = systemInformation;
        }

        //[HttpGet]
        //[Route("Devices")]
        //public Task<List<DeviceBasicInfo>> Devices(long userId)
        //{
        //    return Task.Run(() => _deviceService.GetDevices(adminUserId: userId));
        //}

        [HttpGet]
        [Route("DevicesByFilter")]
        public Task<List<DeviceBasicInfo>> DevicesByFilter(long adminUserId = 0, int deviceGroupId = 0, uint code = 0, int deviceId = 0, int brandId = 0, string deviceName = null, int deviceModelId = 0)
        {
            return Task.Run(() => _deviceService.GetDevices(adminUserId: adminUserId, deviceGroupId: deviceGroupId, code: code,
                brandId: brandId, deviceName: deviceName, deviceModelId: deviceModelId));
        }
        [HttpGet]
        [Route("Devices")]
        public Task<List<DeviceBasicInfo>> DevicesFilter(long userId, string deviceName = default, int deviceModelId = default, int deviceTypeId = default)
        {
            return Task.Run(() => _deviceService.GetDevices(deviceName: deviceName, deviceModelId: deviceModelId, brandId: deviceTypeId, adminUserId: userId));
        }

        ////ToDo: Check and fix route duplicate
        //[HttpGet]
        //[Route("Devices")]
        //public Task<List<DeviceBasicInfo>> GetDevices(string deviceName, int deviceModelId, int deviceTypeId, long userId)
        //{
        //    return Task.Run(() => _deviceService.GetDevicesBasicInfosByFilter(deviceName, deviceModelId, deviceTypeId, userId));
        //}

        [HttpGet]
        [Route("Device")]
        public Task<DeviceBasicInfo> Device(int deviceId, int userId)
        {
            return Task.Run(() => _deviceService.GetDevice(adminUserId: userId, id: deviceId));
        }

        [HttpGet]
        [Route("DevicesList")]
        public Task<List<DeviceBasicInfo>> DevicesList([FromBody] List<int> deviceIds)
        {
            return Task.Run(() =>
            {
                var devices = new List<DeviceBasicInfo>();
                foreach (var deviceId in deviceIds)
                {
                    var device = _deviceService.GetDevice(id: deviceId);
                    if (device != null)
                        devices.Add(device);
                }

                return devices;
            });
        }

        [HttpGet]
        [Route("DevicesListByName")]
        public Task<List<DeviceBasicInfo>> DevicesListByName(string deviceName, int userId = 0)
        {
            return Task.Run(() => _deviceService.GetDevices(deviceName: deviceName, adminUserId: userId));
        }


        [HttpPost]
        [Route("ReadOfflineLog")]
        public Task<List<ResultViewModel>> ReadOfflineLog(string deviceIds, string fromDate, string toDate)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var deviceId = JsonConvert.DeserializeObject<int[]>(deviceIds);

                    var result = new List<ResultViewModel>();
                    for (var i = 0; i < deviceId.Length; i++)
                    {
                        var device = _deviceService.GetDevice(deviceId[i]);
                        if (device == null)
                        {
                            Logger.Log($"DeviceId {deviceId[i]} does not exist.");
                            result.Add(new ResultViewModel
                            { Validate = 0, Message = $"DeviceId {deviceId[i]} does not exist.", Id = deviceIds[i] });
                            continue;
                        }

                        var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/ReadOfflineOfDevice");
                        restRequest.AddQueryParameter("code", device.Code.ToString());
                        restRequest.AddQueryParameter("fromDate", fromDate);
                        restRequest.AddQueryParameter("toDate", toDate);

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
            });
        }

        [HttpGet]
        [Route("DevicesListByBrandId")]
        public Task<List<DeviceBasicInfo>> DevicesListByBrandId(string brandCode, int userId = 0)
        {
            return Task.Run(() => _deviceService.GetDevices(code: uint.Parse(brandCode), adminUserId: userId));
        }

        [HttpGet]
        [Route("DevicesListByModelId")]
        public Task<List<DeviceBasicInfo>> DevicesListByModelId(int modelId, int userId = 0)
        {
            return Task.Run(() => _deviceService.GetDevices(deviceModelId: modelId, adminUserId: userId));
        }

        [HttpGet]
        [Route("DeviceByIdAndBrandId")]
        public Task<DeviceBasicInfo> DeviceByIdAndBrandId(int deviceId, string brandCode, int userId = 0)
        {
            return Task.Run(() => _deviceService.GetDevices(code: uint.Parse(brandCode), adminUserId: userId).Find(d => d.DeviceId == deviceId));
        }

        [HttpGet]
        [Route("DeviceByIdAndModelId")]
        public Task<DeviceBasicInfo> DeviceByIdAndModelId(int deviceId, int modelId, int userId = 0)
        {
            return Task.Run(() => _deviceService.GetDevices(deviceModelId: modelId, adminUserId: userId).Find(d => d.DeviceId == deviceId));
        }

        [HttpGet]
        [Route("DeviceBrands")]
        public async Task<List<Lookup>> DeviceBrands(bool loadedOnly = true)
        {
            if (!loadedOnly) return await Task.Run(() => _deviceService.GetDeviceBrands());
            return _systemInformation.Services.Select(brand => _lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Name, brand.Name))).ToList();
        }

        [HttpGet]
        [Route("DeviceModels")]
        public Task<List<DeviceModel>> DeviceModels(string brandCode = default, bool loadedBrandsOnly = true)
        {
            return Task.Run(() =>
            {
                var deviceModels = _deviceService.GetDeviceModels(brandId: brandCode is null ? 0 : Convert.ToInt32(brandCode));
                if (!loadedBrandsOnly) return deviceModels;

                return deviceModels.Where(dm => _systemInformation.Services.Any(db =>
                    string.Equals(dm.Brand.Name, db.Name, StringComparison.InvariantCultureIgnoreCase))).ToList();
            });
        }

        [HttpGet]
        [Route("GetDeviceModelsByFilter")]
        public Task<List<DeviceModel>> GetDeviceModelsByFilter(string brandCode = default, string name = default, bool loadedBrandsOnly = true)
        {
            return Task.Run(() =>
            {
                var deviceModels = _deviceService.GetDeviceModels(brandId: int.Parse(brandCode ?? string.Empty), deviceName: name);
                if (!loadedBrandsOnly) return deviceModels;
                var restRequest = new RestRequest("SystemInfo/LoadedBrand", Method.GET);
                var requestResult = _restClient.Execute<ResultViewModel<SystemInfo>>(restRequest);
                if (requestResult.StatusCode != HttpStatusCode.OK || requestResult.Data.Validate == 0) return null;

                return deviceModels.Where(dm => requestResult.Data.Data.Services.Any(db =>
                    string.Equals(dm.Brand.Name, db.Name, StringComparison.InvariantCultureIgnoreCase))).ToList();
            });
        }


        [HttpPost]
        [Route("ModifyDeviceInfo")]
        public Task<ResultViewModel> ModifyDeviceInfo([FromBody] DeviceBasicInfo device)
        {
            return Task.Run(async () =>
            {
                var result = _deviceService.ModifyDevice(device);
                if (result.Validate != 1) return result;

                device = _deviceService.GetDevice(id: device.DeviceId);

                var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/ModifyDevice", Method.POST);
                restRequest.AddJsonBody(device);
                await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                return result;
            });
        }

        [HttpGet]
        [Route("RemoveDevice")]
        public Task<ResultViewModel> RemoveDevice(uint deviceId)
        {
            return Task.Run(() => _deviceService.DeleteDevice(deviceId));
        }

        [HttpPost]
        [Route("DeleteDevices")]
        public Task<List<ResultViewModel>> DeleteDevices([FromBody] List<uint> deviceIds)
        {
            return Task.Run(async () =>
            {
                //var resultList = new Dictionary<uint, bool>();
                var resultList = new List<ResultViewModel>();

                var brands = _deviceService.GetDeviceBrands();
                foreach (var deviceBrand in brands)
                {
                    try
                    {
                        var restRequest = new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Device/DeleteDevices", Method.POST);
                        restRequest.AddJsonBody(deviceIds);
                        await _restClient.ExecuteAsync<Dictionary<uint, bool>>(restRequest);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }

                foreach (var deviceId in deviceIds)
                {
                    var result = _deviceService.DeleteDevice(deviceId);
                    resultList.Add(result);
                    //resultList.Add(deviceId, result.Validate == 1);
                }

                return resultList;
            });
        }

        [HttpGet]
        [Route("GetOnlineDevices")]
        public Task<List<DeviceBasicInfo>> GetOnlineDevices()
        {
            return Task.Run(() =>
            {
                var resultList = new List<DeviceBasicInfo>();
                //var deviceBrands = _deviceService.GetDeviceBrands();
                var deviceBrands = _systemInformation.Services;

                Parallel.ForEach(deviceBrands, deviceBrand =>
                {
                    var restRequest = new RestRequest(
                        $"{deviceBrand.Name}/{deviceBrand.Name}Device/GetOnlineDevices");
                    var result = _restClient.Execute<List<DeviceBasicInfo>>(restRequest);

                    if (result.StatusCode == HttpStatusCode.OK)
                        resultList.AddRange(result.Data);
                });

                return resultList;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="userId">Json list of userIds</param>
        /// <returns></returns>
        [HttpPost]
        [Route("RetrieveUserFromDevice")]
        public Task<List<ResultViewModel>> RetrieveUserFromDevice([FromQuery] int deviceId, [FromBody] List<int> userId)
        {
            return Task.Run(async () =>
            {
                var device = _deviceService.GetDevice(deviceId);

                var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveUserFromDevice", Method.POST);
                restRequest.AddQueryParameter("code", device.Code.ToString());
                //restRequest.AddQueryParameter("userId", userId.ToString());
                restRequest.AddJsonBody(userId);
                var result = await _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);

                return result.StatusCode == HttpStatusCode.OK ? result.Data : new List<ResultViewModel> { new ResultViewModel { Id = deviceId, Validate = 0, Message = result.ErrorMessage } };
            });
        }

        [HttpPost]
        [Route("RemoveUserFromDevice")]
        public Task<List<ResultViewModel>> RemoveUserFromDevice(int deviceId, [FromBody] JArray userId)
        {
            return Task.Run(async () =>
            {
                var result = new List<ResultViewModel>();
                if (userId == null || userId.Count == 0)
                    return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = "No users selected." } };

                //var users = JsonConvert.DeserializeObject<long[]>(userId.ToString());
                var device = _deviceService.GetDevice(deviceId);

                //foreach (var user in users)
                //{
                var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/DeleteUserFromDevice", Method.POST);
                restRequest.AddQueryParameter("code", device.Code.ToString());
                //restRequest.AddQueryParameter("userId", user.ToString());
                restRequest.AddJsonBody(userId);

                var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                if (requestResult.StatusCode == HttpStatusCode.OK)
                    result.Add(requestResult.Data);
                //}

                return result;
            });
        }


        [HttpGet]
        [Route("RetrieveUsersListFromDevice")]
        public Task<List<User>> RetrieveUsersListFromDevice(int deviceId)
        {
            return Task.Run(async () =>
            {
                var device = _deviceService.GetDevice(deviceId);
                var userAwaiter = _userService.GetUsers();

                var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveUsersListFromDevice");
                restRequest.AddQueryParameter("code", device.Code.ToString());
                var restAwaiter = _restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest);

                var result = await restAwaiter;
                var users = userAwaiter;

                var lstResult = (from r in result.Data?.Data
                                 join u in users on r.Id equals u.Id
                                     into ps
                                 from u in ps.DefaultIfEmpty()
                                 select new User
                                 {
                                     Type = u == null ? 0 : 1,
                                     IsActive = r.IsActive,
                                     Id = r.Id,
                                     FullName = u != null ? u.FirstName + " " + u.SurName : r.UserName,
                                     StartDate = u?.StartDate ?? new DateTime(1990, 1, 1),
                                     EndDate = u?.EndDate ?? new DateTime(2050, 1, 1)
                                 }).ToList();

                return lstResult;
            });
        }

        [HttpPost]
        [Route("SendUsersOfDevice")]
        public Task<ResultViewModel> SendUsersOfDevice(int deviceId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var device = _deviceService.GetDevice(deviceId);
                    if (device == null)
                    {
                        Logger.Log($"DeviceId {deviceId} does not exist.");
                        return new ResultViewModel { Validate = 0, Message = $"DeviceId {deviceId} does not exist." };
                    }

                    var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/SendUsersOfDevice", Method.POST);
                    restRequest.AddJsonBody(device);

                    var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                    return new ResultViewModel { Validate = result.StatusCode == HttpStatusCode.OK ? 1 : 0, Id = deviceId };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = $"SendUserToDevice Failed. DeviceId: {deviceId}" };
                }
            });
        }

        [HttpGet]
        [Route("GetAdditionalData")]
        public Task<Dictionary<string, string>> GetAdditionalData(int deviceId)
        {
            return Task.Run(async () =>
            {
                var device = _deviceService.GetDevice(deviceId);

                var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/GetAdditionalData");
                restRequest.AddQueryParameter("code", device.Code.ToString());
                var result = await _restClient.ExecuteAsync<Dictionary<string, string>>(restRequest);

                return result.StatusCode == HttpStatusCode.OK ? result.Data : new Dictionary<string, string>();
            });
        }

        [HttpPost]
        [Route("SendDevicesDataToDevice")]
        public Task<List<ResultViewModel>> SendDevicesDataToDevice([FromBody] List<int> deviceIds, int deviceId = default)
        {
            return Task.Run(() =>
            {
                var results = new List<ResultViewModel>();
                var deviceBrands = _deviceService.GetDeviceBrands();

                var tasks = new List<Task>();
                foreach (var deviceBrand in deviceBrands)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var restRequest =
                            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Device/SendDevicesDataToDevice");
                        if (deviceId != default)
                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                        if (deviceIds != null)
                            restRequest.AddJsonBody(deviceIds);

                        var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                        lock (results)
                        {
                            results.Add(new ResultViewModel
                            {
                                Id = result.Data.Id,
                                Code = result.Data.Code,
                                Validate = result.IsSuccessful && result.Data.Validate == 1 ? 1 : 0,
                                Message = deviceBrand.Name
                            });
                        }
                    }));
                }

                Task.WaitAll(tasks.ToArray());
                return results;
            });
        }

        //[HttpPost]
        //public Task<ResultViewModel> UpgradeFirmware(int deviceId)
        //{
        //    return Task.Run(async () =>
        //    {
        //        if (!Request.Content.IsMimeMultipartContent())
        //            return new ResultViewModel { Validate = 0, Code = 415, Message = "UnsupportedMediaType" };

        //        try
        //        {
        //            var device = _deviceService.GetDevice(deviceId);

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
    }
}