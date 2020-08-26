using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service;
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
        private readonly DeviceService _deviceService;
        private readonly UserService _userService;
        private readonly RestClient _restClient;


        public DeviceController(DeviceService deviceService, UserService userService)
        {
            _deviceService = deviceService;
            _userService = userService;
            _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }

        [HttpGet]
        [Route("Devices")]
        public Task<List<DeviceBasicInfo>> Devices(long userId)
        {
            return Task.Run(() => _deviceService.GetAllDevicesBasicInfos(userId));
        }

        [HttpGet]
        [Route("DevicesByFilter")]
        public Task<List<DeviceBasicInfo>> DevicesByFilter(long adminUserId = 0, int deviceGroupId = 0, uint code = 0, int deviceId = 0, int brandId = 0, string deviceName = null, int deviceModelId = 0)
        {
            return Task.Run(() => _deviceService.GetAllDevicesBasicInfosByfilter(adminUserId, deviceGroupId, code, deviceId, brandId, deviceName, deviceModelId));
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
            return Task.Run(() => _deviceService.GetDeviceInfo(deviceId, userId));
        }

        [HttpGet]
        [Route("DevicesList")]
        public Task<List<DeviceBasicInfo>> DevicesList(List<int> deviceIds)
        {
            return Task.Run(() =>
            {
                var devices = new List<DeviceBasicInfo>();
                foreach (var deviceId in deviceIds)
                {
                    var device = _deviceService.GetDeviceInfo(deviceId);
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
            return Task.Run(() => _deviceService.GetDevicesBasicInfosByName(deviceName, userId));
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
                        var device = _deviceService.GetDeviceInfo(deviceId[i]);
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
            return Task.Run(() => _deviceService.GetAllDevicesBasicInfosByBrandId(brandCode, userId));
        }

        [HttpGet]
        [Route("DevicesListByModelId")]
        public Task<List<DeviceBasicInfo>> DevicesListByModelId(int modelId, int userId = 0)
        {
            return Task.Run(() => _deviceService.GetAllDevicesBasicInfosByDeviceModelId(modelId, userId));
        }

        [HttpGet]
        [Route("DeviceByIdAndBrandId")]
        public Task<DeviceBasicInfo> DeviceByIdAndBrandId(int deviceId, string brandCode, int userId = 0)
        {
            return Task.Run(() => _deviceService.GetDeviceBasicInfoByIdAndBrandId(deviceId, brandCode, userId));
        }

        [HttpGet]
        [Route("DeviceByIdAndModelId")]
        public Task<DeviceBasicInfo> DeviceByIdAndModelId(int deviceId, int modelId, int userId = 0)
        {
            return Task.Run(() => _deviceService.GetDeviceBasicInfoByIdAndDeviceModelId(deviceId, modelId, userId));
        }

        [HttpGet]
        [Route("DeviceBrands")]
        public async Task<List<Lookup>> DeviceBrands(bool loadedOnly = true)
        {
            if (!loadedOnly) return await Task.Run(() => _deviceService.GetDeviceBrands());
            var restRequest = new RestRequest("SystemInfo/LoadedBrand", Method.GET);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<SystemInfo>>(restRequest);
            return requestResult.StatusCode != HttpStatusCode.OK || requestResult.Data.Validate == 0 ? null : requestResult.Data.Data.Modules.Select(brand => Lookups.DeviceBrands.FirstOrDefault(lookup => string.Equals(lookup.Name, brand.Name))).ToList();
        }

        [HttpGet]
        [Route("DeviceModels")]
        public Task<List<DeviceModel>> DeviceModels(string brandCode = default, bool loadedBrandsOnly = true)
        {
            return Task.Run(() =>
            {
                var deviceModels = _deviceService.GetDeviceModelsByBrandCode(brandCode);
                if (!loadedBrandsOnly) return deviceModels;
                var restRequest = new RestRequest("SystemInfo/LoadedBrand", Method.GET);
                var requestResult = _restClient.Execute<ResultViewModel<SystemInfo>>(restRequest);
                if (requestResult.StatusCode != HttpStatusCode.OK || requestResult.Data.Validate == 0) return null;

                return deviceModels.Where(dm => requestResult.Data.Data.Modules.Any(db =>
                    string.Equals(dm.Brand.Name, db.Name, StringComparison.InvariantCultureIgnoreCase))).ToList();
            });
        }

        [HttpGet]
        [Route("GetDeviceModelsByFilter")]
        public Task<List<DeviceModel>> GetDeviceModelsByFilter(string brandCode = default, string name = default, bool loadedBrandsOnly = true)
        {
            return Task.Run(() =>
            {
                var deviceModels = _deviceService.GetDeviceModelsByFilter(brandCode, name);
                if (!loadedBrandsOnly) return deviceModels;
                var restRequest = new RestRequest("SystemInfo/LoadedBrand", Method.GET);
                var requestResult = _restClient.Execute<ResultViewModel<SystemInfo>>(restRequest);
                if (requestResult.StatusCode != HttpStatusCode.OK || requestResult.Data.Validate == 0) return null;

                return deviceModels.Where(dm => requestResult.Data.Data.Modules.Any(db =>
                    string.Equals(dm.Brand.Name, db.Name, StringComparison.InvariantCultureIgnoreCase))).ToList();
            });
        }


        [HttpPost]
        [Route("ModifyDeviceInfo")]
        public Task<ResultViewModel> ModifyDeviceInfo(DeviceBasicInfo device)
        {
            return Task.Run(async () =>
            {
                var result = _deviceService.ModifyDeviceBasicInfoByID(device);
                if (result.Validate != 1) return result;

                device = _deviceService.GetDeviceBasicInfoWithCode(device.Code, device.Brand.Code);

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
        public Task<Dictionary<uint, bool>> DeleteDevices([FromBody] List<uint> deviceIds)
        {
            return Task.Run(async () =>
            {
                var resultList = new Dictionary<uint, bool>();

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
                    resultList.Add(deviceId, result.Validate == 1);
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
                var deviceBrands = _deviceService.GetDeviceBrands();

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
        public Task<List<ResultViewModel>> RetrieveUserFromDevice([FromQuery]int deviceId, [FromBody] List<int> userId)
        {
            return Task.Run(async () =>
            {
                var device = _deviceService.GetDeviceInfo(deviceId);

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
                var device = _deviceService.GetDeviceInfo(deviceId);

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
                var device = _deviceService.GetDeviceInfo(deviceId);
                var userAwaiter = _userService.GetUsers(getTemplatesData: false);

                var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveUsersListFromDevice");
                restRequest.AddQueryParameter("code", device.Code.ToString());
                var restAwaiter = _restClient.ExecuteAsync<ResultViewModel<List<User>>>(restRequest);

                var result = await restAwaiter;
                var users = await userAwaiter;

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
                    var device = _deviceService.GetDeviceInfo(deviceId);
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
                var device = _deviceService.GetDeviceInfo(deviceId);

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


        //TODO make compatible with .net core
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


    }
}