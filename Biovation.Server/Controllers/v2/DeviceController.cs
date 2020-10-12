using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Server.Model;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Biovation.Server.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
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
        [Route("{id}")]
        public Task<ResultViewModel<DeviceBasicInfo>> Device(long id = default, long adminUserId = default)
        {
            return Task.Run(() => _deviceService.GetDevice(id, adminUserId));
        }


        //TODO loaded brand
        [HttpGet]
        public  Task<ResultViewModel<PagingResult<DeviceBasicInfo>>> Devices(long adminUserId = default, int groupId = default, uint code = default,
            int brandId = default, string name = null, int modelId = default, int typeId = default, int pageNumber = default, int PageSize = default)
        {
            var result = Task.Run(() => _deviceService.GetDevices(adminUserId, groupId, code, brandId.ToString(), name, modelId, typeId, pageNumber, PageSize));
            return result;
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




        [HttpPost]
        public Task<ResultViewModel> AddDevice([FromBody]DeviceBasicInfo device = default)
        {

            return Task.Run(() => _deviceService.AddDevice(device));
        }

        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteDevice(uint id = default)
        {

           return Task.Run(() => _deviceService.DeleteDevice(id));
        }

        [HttpGet]
        [Route("OfflineLogs/{ids}")]
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
                        //var restRequest = new RestRequest($"Queries/v2/Device/{deviceId[i]}", Method.GET);
                        //var device = (_restClient.ExecuteAsync<ResultViewModel<DeviceBasicInfo>>(restRequest)).Result.Data.Data;
                        var device = _deviceService.GetDevice(id: deviceId[i]).Data;
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


        [HttpPost]
        [Route("DeleteDevice")]
        public Task<ResultViewModel> DeleteDevice([FromBody]List<uint> ids = default)
        {
            return Task.Run(() => _deviceService.DeleteDevices(ids));
        }

        [HttpGet]
        [Route("OnlineDevices")]
        public Task<List<DeviceBasicInfo>> OnlineDevices()
        {
            return Task.Run(async () =>
            {
                var resultList = new List<DeviceBasicInfo>();
                var deviceBrands = _deviceService.GetDeviceBrands().Data;

                foreach (var deviceBrand in deviceBrands)
                {
                    var restRequest = new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Device/GetOnlineDevices");
                    var result = await _restClient.ExecuteAsync<List<DeviceBasicInfo>>(restRequest);

                    if (result.StatusCode == HttpStatusCode.OK)
                        resultList.AddRange(result.Data);
                }

                return resultList;
            });
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="deviceId"></param>
        ///// <param name="userId">Json list of userIds</param>
        ///// <returns></returns>
        [HttpPut]
        [Route("UserFromDevice/{id}")]
        public Task<ResultViewModel> RetrieveUserDevice(int id = default, [FromBody]JArray userId = default)
        {
            return Task.Run( () =>
            {
                var device = _deviceService.GetDevice(id).Data;

                var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveUserFromDevice", Method.POST);
                restRequest.AddQueryParameter("code", device.Code.ToString());
                //restRequest.AddQueryParameter("userId", userId.ToString());
                restRequest.AddJsonBody(userId);
                var restResult =  _restClient.ExecuteAsync<List<ResultViewModel>>(restRequest);
                var result = restResult.Result.Data.Any(e => e.Validate == 0)
                    ? new ResultViewModel(){ Validate = 0, Id = id}
                    : new ResultViewModel() { Validate = 1, Id = id };
                return result;

                //return result.StatusCode == HttpStatusCode.OK ? result.Data : new List<ResultViewModel> { new ResultViewModel { Id = id, Validate = 0, Message = result.ErrorMessage } };
            });
        }

        [HttpPut]
        [Route("UsersListFromDevice/{id}")]
        public Task<List<User>> RetrieveUsersOfDevice(int id = default)
        {
            return Task.Run(async () =>
            {
                var device = _deviceService.GetDevice(id).Data;
                var userAwaiter = Task.Run(() => _userService.GetUsers(getTemplatesData: false).Data.Data);

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

        [HttpDelete]
        [Route("UserFromDevice/{id}/{userId}")]
        public Task<ResultViewModel> UsersFromDevice(int id = default, int userId = default)
        {
            return Task.Run( () =>
            {
                ResultViewModel result;
                if (userId == default)
                    return new ResultViewModel  { Validate = 0, Message = "No users selected." };

                var device = _deviceService.GetDevice(id).Data;

                var restRequest = new RestRequest($"{device.Brand?.Name}/{device.Brand?.Name}Device/DeleteUserFromDevice", Method.POST);
                restRequest.AddQueryParameter("code", device.Code.ToString());
                restRequest.AddJsonBody(userId);

                return _restClient.ExecuteAsync<ResultViewModel>(restRequest).Result.Data;
            });
        }

        [HttpPut]
        [Route("SendUsersToDevice/{id}")]
        public Task<ResultViewModel> SendUsersToDevice(int id = default)
        {
            return Task.Run( () =>
            {
                try
                {
                    var device = _deviceService.GetDevice(id).Data;
                    if (device == null)
                    {
                        Logger.Log($"DeviceId {id} does not exist.");
                        return new ResultViewModel { Validate = 0, Message = $"DeviceId {id} does not exist." };
                    }

                    var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/SendUsersOfDevice", Method.POST);
                    restRequest.AddJsonBody(device);

                    var result =  _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                    return new ResultViewModel { Validate = result.Result.StatusCode == HttpStatusCode.OK ? 1 : 0, Id = id };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = $"SendUserToDevice Failed. DeviceId: {id}" };
                }
            });
        }

        [HttpGet]
        [Route("DeviceInfo/{id}")]
        public Task<ResultViewModel<Dictionary<string, string>>> DeviceInfo(int id = default)
        {
            return Task.Run( () =>
            {
                var device = _deviceService.GetDevice(id).Data;

                var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/GetAdditionalData");
                restRequest.AddQueryParameter("code", device.Code.ToString());
                var result =  _restClient.ExecuteAsync<Dictionary<string, string>>(restRequest);

                return new ResultViewModel<Dictionary<string, string>>()
                {
                    Success = result.Result.StatusCode ==HttpStatusCode.OK,
                    Data = result.Result.Data,
                    Id = id
                };
            });
        }


        ////TODO check it wtf?
        //[HttpPost]
        //[Route("DevicesDataToDevice/{id}")]
        //public Task<JsonResult> DevicesDataToDevice([FromBody]List<int> ids = default, int id = default)
        //{
        //    throw null;
        //}


        ////TODO make compatible with .net core
        ////[HttpPost]
        ////[Route("UpgradeFirmware")]
        ////public Task<ResultViewModel> UpgradeFirmware(int deviceId)
        ////{
        ////    return Task.Run(async () =>
        ////    {
        ////        if (!Request.Content.IsMimeMultipartContent())
        ////            return new ResultViewModel { Validate = 0, Code = 415, Message = "UnsupportedMediaType" };

        ////        try
        ////        {
        ////            var device = _deviceService.GetDeviceInfo(deviceId);

        ////            if (device is null)
        ////                return new ResultViewModel
        ////                { Validate = 0, Code = 400, Id = deviceId, Message = "Wrong device id provided" };

        ////            var multipartMemory = await Request.Content.ReadAsMultipartAsync();

        ////            foreach (var multipartContent in multipartMemory.Contents)
        ////            {
        ////                try
        ////                {
        ////                    var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/UpgradeFirmware", Method.POST, DataFormat.Json);
        ////                    restRequest.AddHeader("Content-Type", "multipart/form-data");
        ////                    restRequest.AddQueryParameter("deviceCode", device.Code.ToString());
        ////                    restRequest.AddFile(multipartContent.Headers.ContentDisposition.Name.Trim('\"'),
        ////                        multipartContent.ReadAsByteArrayAsync().Result,
        ////                        multipartContent.Headers.ContentDisposition.FileName.Trim('\"'),
        ////                        multipartContent.Headers.ContentType.MediaType);
        ////                    var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
        ////                    if (!result.IsSuccessful || result.Data.Validate == 0)
        ////                        return result.Data;
        ////                }
        ////                catch (Exception exception)
        ////                {
        ////                    Logger.Log(exception, logType: LogType.Debug);
        ////                }
        ////            }
        ////        }
        ////        catch (Exception exception)
        ////        {
        ////            Logger.Log(exception, logType: LogType.Debug);
        ////            throw;
        ////        }

        ////        return new ResultViewModel { Validate = 1, Code = 200, Id = deviceId, Message = "Files uploaded and upgrading firmware started." };
        ////    });
        ////}


    }
}