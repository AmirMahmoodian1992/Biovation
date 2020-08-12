using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Biovation.Gateway.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("[controller]")]
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


        //TODO compatible with paging(az yaser bepors)
        [HttpGet]
        [Route("{id}")]
        public async Task<JsonResult> Devices(long id =0, long adminUserId = 0, int deviceGroupId = 0, uint code = 0, 
            int deviceId = 0, int brandId = 0, string deviceName = null, int deviceModelId = 0, int deviceTypeId = 0, 
            string brandCode = null, bool loadedBrandsOnly = true)
        {
            throw null;
        }

        [HttpPost]
        public Task<JsonResult> DeviceInfo([FromBody]DeviceBasicInfo device =default)
        {
            throw null;
        }

        [HttpDelete]
        [Route("{id}")]
        public Task<JsonResult> DeleteDevice(uint id = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("OfflineLogs/{ids}")]
        public Task<JsonResult> OfflineLogs(string ids = default, string fromDate = default, string toDate = default)
        {
            throw null;
        }


        [HttpPost]
        [Route("DeleteDevice")]
        public Task<JsonResult> DeleteDevice([FromBody]List<uint> ids = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("OnlineDevices")]
        public Task<JsonResult> OnlineDevices()
        {
            throw null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="userId">Json list of userIds</param>
        /// <returns></returns>
        [HttpPut]
        [Route("UserFromDevice/{id}")]
        public async Task<IActionResult> UserDevice(int id = default, [FromBody]JArray userId = default)
        {
            //return NotFound();
            throw null;
        }

        [HttpPut]
        [Route("UsersListFromDevice/{id}")]
        public Task<IActionResult> UsersOfDevice(int id = default)
        {
            throw null;
        }

        [HttpPost]
        [Route("DeleteUserFromDevice/{id}/{userId}")]
        public Task<JsonResult> UsersFromDevice(int id = default, [FromBody]JArray userId = default)
        {
            throw null;
        }

        [HttpPut]
        [Route("SendUsersToDevice/{id}")]
        public Task<JsonResult> SendUsersToDevice(int id = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("DeviceInfo/{id}")]
        public Task<JsonResult> DeviceInfo(int id = default)
        {
            throw null;
        }


        //TODO check it wtf?
        [HttpPost]
        [Route("DevicesDataToDevice/{id}")]
        public Task<JsonResult> DevicesDataToDevice([FromBody]List<int> ids = default, int id = default)
        {
            throw null;
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
        //                    var result = await _restClient.ExecuteTaskAsync<ResultViewModel>(restRequest);
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