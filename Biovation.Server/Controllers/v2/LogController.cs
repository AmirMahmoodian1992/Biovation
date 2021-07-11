using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
        public async Task<ResultViewModel<PagingResult<Log>>> Logs(int id = default,int deviceGroupId = default, int deviceId = default,
                        int userId = default, bool? successTransfer = null, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
                        int pageSize = default, string where = default, string order = default)
        {
            return await _logService.Logs(id, deviceGroupId, deviceId, userId, successTransfer, fromDate, toDate, pageNumber, pageSize, where, order, HttpContext.Items["Token"] as string);
        }

        [HttpGet]
        [Route("{id}/Image")]
        public async Task<byte[]> GetImage([FromRoute] long id)
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
        }

        /// <summary>
        /// Convert offline logs
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("TransmitLogs")]
        public async Task<ResultViewModel> TransmitOfflineLogs([FromQuery] int deviceId = default, [FromQuery] int userId = default,
            [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null, [FromQuery] string where = default, [FromQuery] bool resendLogs = false)
        {
            try
            {
                var token = HttpContext.Items["Token"] as string;
                var logs = (await _logService.Logs(default,default, deviceId, userId, resendLogs ? (bool?)null : false,
                    fromDate, toDate, default, default, where, default, token))?.Data?.Data;
                await _logService.TransferLogBulk(logs, token);
                return new ResultViewModel { Validate = 1, Code = logs?.Count ?? 0, Message = logs?.Count.ToString() };
            }
            catch (Exception exception)
            {
                Logger.Log(exception.Message);
                return new ResultViewModel { Validate = 0, Message = exception.ToString() };
            }
        }
    }
}