using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        private readonly LogService _logService;

        public LogController(LogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public Task<ResultViewModel<PagingResult<Log>>> Logs(int id = default, int deviceId = default,
                        int userId = default, bool? successTransfer = null, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
                        int pageSize = default, string where = default, string order = default)
        {
            var token = HttpContext.Items["Token"] as string;
            return Task.Run(() => _logService.Logs(id, deviceId, userId, successTransfer, fromDate, toDate, pageNumber, pageSize, where, order, token));
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
        /// <param name="logFilter"></param>
        /// <param name="resendLogs"></param>
        /// <returns></returns>
        //convert offline logs
        [HttpPost]
        [Route("OfflineLogs")]
        public Task<ResultViewModel> TransmitOfflineLogs(long userId = default, string logFilter = default, bool resendLogs = default)
        {
            var token = HttpContext.Items["Token"] as string;
            return Task.Run(async () =>
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<DeviceTraffic>(logFilter);
                    obj.OnlineUserId = userId;
                    obj.State = resendLogs ? (bool?)null : false;
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