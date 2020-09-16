using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service;
using Microsoft.AspNetCore.Mvc;
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


        //TODO loaded brand
        [HttpGet]
        public Task<List<DeviceBasicInfo>> Devices(/*long id = 0,*/ long adminUserId = 0, int groupId = 0, uint code = 0,
            int brandId = 0, string name = null, int modelId = 0, int typeId = 0)
        {
            //Task.Run(() => _deviceService.GetDevicesByfilter(/*id,*/ adminUserId, groupId, code, brandId, name, modelId, typeId));
            throw null;
        }

        [HttpPost]
        public Task<JsonResult> AddDeviceInfo([FromBody]DeviceBasicInfo device)
        {

            Task.Run(() => _deviceService.AddDevice(device));
            throw null;
        }

        [HttpDelete]
        [Route("{id?}")]
        public Task<JsonResult> DeleteDevice(uint id = default)
        {

            Task.Run(() => _deviceService.DeleteDevice(id));
            throw null;
        }

        [HttpGet]
        [Route("OfflineLogs")]
        public Task<JsonResult> OfflineLogs([FromQuery] JArray ids = default, string fromDate = default, string toDate = default)
        {
            throw null;
        }


        [HttpPost]
        [Route("DeleteDevice")]
        public Task<JsonResult> DeleteDevice([FromBody]List<uint> ids)
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
        [Route("UserFromDevice/{id?}")]
        public async Task<IActionResult> RetrieveUserDevice(int id = default, [FromBody]JArray userId = default)
        {
            //return NotFound();
            throw null;
        }

        [HttpPut]
        [Route("UsersListFromDevice/{id?}")]
        public Task<IActionResult> RetrieveUsersOfDevice(int id = default)
        {
            throw null;
        }

        [HttpPost]
        [Route("DeleteUserFromDevice/{id?}/{userId?}")]
        public Task<JsonResult> UsersFromDevice(int id = default, [FromBody]JArray userId = default)
        {
            throw null;
        }

        [HttpPut]
        [Route("SendUsersToDevice/{id?}")]
        public Task<JsonResult> SendUsersToDevice(int id = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("DeviceInfo/{id?}")]
        public Task<JsonResult> DeviceInfo(int id = default)
        {
            throw null;
        }


        //TODO check it wtf?
        [HttpPost]
        [Route("DevicesDataToDevice/{id?}")]
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