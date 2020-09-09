using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Server.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class LogController : Controller
    {
        private readonly UserService _userService;
        private readonly LogService _logService;
        private readonly DeviceService _commonDeviceService;
        private readonly RestClient _restClient;

        public LogController(DeviceService deviceService, UserService userService, LogService logService)
        {
            _userService = userService;
            _logService = logService;
            _commonDeviceService = deviceService;
            _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }

        //we should consider the without parameter input version of log
        // and handle searchOfflineLogs with paging or not with  [FromBody]DeviceTraffic dTraffic
        [HttpGet]
        public Task<ResultViewModel<PagingResult<Domain.Log>>> Logs(int id = default, int deviceId = default,
            int userId = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _logService.Logs(id,deviceId,userId,fromDate,toDate,pageNumber,pageSize));
        }

        [HttpDelete]
        [Route("{deviceId}")]
        public Task<IActionResult> ClearLogOfDevice(int deviceId = default, string fromDate = default, string toDate = default)
        {
            throw null;
        }


        //batch delete
        [HttpPost]
        [Route("ClearLogsOfDevices")]
        public Task<IActionResult> ClearLogOfDevice([FromBody]List<int> deviceIds = default, string fromDate = default, string toDate = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("LogsOfDevice/{deviceId}")]
        public Task<IActionResult> LogsOfDevice(int deviceId = default, DateTime? fromDate = null, DateTime? toDate = null, bool offline = default)
        {
            throw null;
        }


        [HttpGet]
        [Route("Image/{id}")]
        public Task<IActionResult> GetImage(long id = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("LogsOfUser{userId}")]
        public Task<IActionResult> LogsOfUser(int userId = default, DateTime? fromDate = null, DateTime? toDate = null, bool offline = default)
        {
            throw null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dTraffic"></param>
        /// <param name="resendLogs"></param>
        /// <returns></returns>
        //convert offline logs
        [HttpPost]
        [Route("OfflineLogs")]
        public Task<IActionResult> TransmitOfflineLogs(long userId = default, string dTraffic = default , bool resendLogs = default)
        {
            throw null;
        }


    }
}