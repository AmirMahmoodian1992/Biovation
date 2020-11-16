using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;
using Biovation.Server.Attribute;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class LogController : ControllerBase
    {

        private readonly UserService _userService;
        private readonly LogService _logService;
        private readonly DeviceService _deviceService;
        private readonly RestClient _restClient;

        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;

        public LogController(DeviceService deviceService, UserService userService, LogService logService, RestClient restClient, TaskTypes taskTypes, TaskPriorities taskPriorities)
        {
            _userService = userService;
            _logService = logService;
            _deviceService = deviceService;
            _restClient = restClient;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
        }

        [HttpGet]
        public Task<ResultViewModel<PagingResult<Log>>> Logs(int id = default, int deviceId = default,
                        int userId = default, bool successTransfer = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
                        int pageSize = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _logService.Logs(id, deviceId, userId, successTransfer, fromDate, toDate, pageNumber, pageSize, token));
        }

        [HttpGet]
        [Route("{id}/Image")]
        public Task<byte[]> GetImage([FromRoute] long id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var result = await _logService.GetImage(id);
                    return result;
                }
                catch (Exception)
                {
                    return new byte[0];
                }
            });
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
        public Task<ResultViewModel> TransmitOfflineLogs(long userId = default, string logFilter = default, bool resendLogs = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () =>
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<DeviceTraffic>(logFilter);
                    obj.OnlineUserId = userId;
                    obj.State = false;
                    var logs = await _logService.SelectSearchedOfflineLogs(obj, token);
                    //var logs = logsAwaiter.Where(w => !w.SuccessTransfer).ToList();
                    await Task.Run(() => { _logService.TransferLogBulk(logs, token); });
                    return new ResultViewModel { Validate = 1, Code = logs.Count, Message = logs.Count.ToString() };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception.Message);
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }

    }
}