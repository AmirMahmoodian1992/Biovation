using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Biovation.Gateway.Controllers.v2
{
    [Route("biovation/api/[controller]")]
    public class LogController : Controller
    {
        private readonly UserService _userService;
        private readonly LogService _commonLogService;
        private readonly DeviceService _commonDeviceService;
        private readonly RestClient _restClient;

        public LogController(DeviceService deviceService, UserService userService, LogService logService)
        {
            _userService = userService;
            _commonLogService = logService;
            _commonDeviceService = deviceService;
            _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }

        //we should consider the without parameter input version of log
        // and handle searchOfflineLogs with paging or not with  [FromBody]DeviceTraffic dTraffic
        [HttpGet]
        public Task<IActionResult> Logs(DateTime? fromDate = null, DateTime? toDate = null)
        {
            throw null;
        }

        [HttpDelete]
        public Task<IActionResult> ClearLogOfDevice(string deviceIds = default, string fromDate = default, string toDate = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("LogsOfDevice")]
        public Task<IActionResult> LogsOfDevice(int deviceId = default, DateTime? fromDate = null, DateTime? toDate = null, bool offline = default)
        {
            throw null;
        }


        [HttpGet]
        [Route("Image")]
        public Task<IActionResult> GetImage(long id = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("LogsOfUser")]
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
        public Task<IActionResult> transmitOfflineLogs(long userId, string dTraffic , bool resendLogs = default)
        {
            throw null;
        }


    }
}