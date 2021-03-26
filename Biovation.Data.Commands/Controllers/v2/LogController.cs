using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Data.Commands.Sinks;
using Biovation.Domain;
using Biovation.Repository.MessageBus;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly LogApiSink _logApiSink;
        private readonly LogRepository _logRepository;
        private readonly DeviceRepository _deviceRepository;
        private readonly LogMessageBusRepository _logMessageBusRepository;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public LogController(LogRepository logRepository, LogMessageBusRepository logMessageBusRepository, LogApiSink logApiSink, DeviceRepository deviceRepository, BiovationConfigurationManager biovationConfigurationManager)
        {
            _logApiSink = logApiSink;
            _logRepository = logRepository;
            _deviceRepository = deviceRepository;
            _logMessageBusRepository = logMessageBusRepository;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        [HttpPost]
        [Authorize]
        [Route("AddLog")]
        public async Task<ResultViewModel> AddLog([FromBody] Log log)
        {
            var filePath = log.PicByte is null
                ? string.Empty
                : await SaveImage(log.PicByte, log.UserId, log.LogDateTime, log.DeviceCode, log.DeviceId.ToString(), log.Time);
            log.Image = filePath;

            var logInsertionAwaiter = _logRepository.AddLog(log);
            //integration
            var broadcastMessageBusAwaiter = _logMessageBusRepository.SendLog(new List<Log> { log });
            var broadcastApiAwaiter = Task.CompletedTask;
            if (log.EventLog.Code == LogEvents.AuthorizedCode || log.EventLog.Code == LogEvents.UnAuthorizedCode)
                broadcastApiAwaiter = _logApiSink.TransferLog(log);

            await Task.WhenAll(logInsertionAwaiter, broadcastApiAwaiter, broadcastMessageBusAwaiter);
            return await logInsertionAwaiter;
        }

        [HttpPost]
        [Authorize]
        [Route("AddLogBulk")]
        public async Task<ResultViewModel> AddLog([FromBody] List<Log> logs)
        {
            var json = JsonConvert.SerializeObject(logs.Select(s => new
            {
                s.Id,
                s.DeviceId,
                s.DeviceCode,
                EventId = s.EventLog.Code,
                s.UserId,
                datetime = s.LogDateTime,
                Ticks = s.DateTimeTicks,
                SubEvent = s.SubEvent?.Code ?? LogSubEvents.NormalCode,
                TNAEvent = s.TnaEvent,
                s.InOutMode,
                MatchingType = s.MatchingType?.Code,
                s.SuccessTransfer
            }));

            var logsDataTable = JsonConvert.DeserializeObject<DataTable>(json);
            var logInsertionResult = _logRepository.AddLog(logsDataTable);

            if (!_biovationConfigurationManager.BroadcastToApi && !_biovationConfigurationManager.BroadcastToMessageBus)
                return await logInsertionResult;

            foreach (var deviceId in logs.GroupBy(g => g.DeviceId).Select(s => s.Key).Where(s => s > 0))
            {
                await Task.Run(async () =>
                {
                    var tasks = new List<Task>();
                    var device = _deviceRepository.GetDevice(deviceId)?.Data;
                    if (device is null) return;

                    var logsToTransfer = await _logRepository.Logs(deviceId: device.DeviceId, successTransfer: false);
                    tasks.Add(_logApiSink.TransferLogBulk(logsToTransfer));
                    tasks.Add(_logMessageBusRepository.SendLog(logsToTransfer));

                    var logsWithImages = logs.Where(log => logsToTransfer.Any(newLog =>
                        log.UserId == newLog.UserId && log.LogDateTime == newLog.LogDateTime &&
                        log.EventLog.Code == newLog.EventLog.Code && log.DeviceId == newLog.DeviceId) && log.PicByte?.Length > 0);

                    foreach (var log in logsWithImages)
                    {
                        var filePath = log.PicByte is null
                            ? string.Empty
                            : await SaveImage(log.PicByte, log.UserId, log.LogDateTime, log.DeviceCode, log.DeviceId.ToString(), log.Time);
                        log.Image = filePath;

                        if (filePath != null)
                            tasks.Add(_logRepository.AddLogImage(log));
                    }

                    await Task.WhenAll(tasks);
                });
            }

            return await logInsertionResult;
        }

        [HttpPost]
        [Authorize]
        [Route("UpdateLog")]
        public async Task<ResultViewModel> UpdateLog([FromBody] List<Log> logs)
        {
            return await _logRepository.UpdateLog(logs);
        }

        [HttpPatch]
        [Authorize]
        [Route("AddLogImage")]
        public async Task<ResultViewModel> AddLogImage([FromBody] Log log)
        {
            return await _logRepository.AddLogImage(log);
        }

        [HttpPut]
        [Authorize]
        [Route("UpdateLog")]
        public async Task<ResultViewModel> UpdateLog([FromBody] Log log)
        {
            return await _logRepository.UpdateLog(log);
        }

        [HttpPost]
        [Authorize]
        [Route("CheckLogInsertion")]
        public async Task<List<Log>> CheckLogInsertion([FromBody] List<Log> logs)
        {
            return await _logRepository.CheckLogInsertion(logs);
        }

        [HttpPost]
        [Authorize]
        [Route("BroadcastLogs")]
        public async Task<ResultViewModel> BroadcastLogs([FromBody] List<Log> logs)
        {
            var broadcastToApiAwaiter = _logApiSink.TransferLogBulk(logs);
            var broadcastToMessageBusAwaiter = _logMessageBusRepository.SendLog(logs);
            await Task.WhenAll(broadcastToApiAwaiter, broadcastToMessageBusAwaiter);

            var broadcastToApiResult = broadcastToApiAwaiter.Result;
            var broadcastToMessageBusResult = broadcastToMessageBusAwaiter.Result;
            return new ResultViewModel
            {
                Success = broadcastToApiResult.Success && broadcastToMessageBusResult.Success,
                Message = !broadcastToApiResult.Success ? broadcastToApiResult.Message : (!broadcastToMessageBusResult.Success ? broadcastToMessageBusResult.Message : "Logs are sent successfully")
            };
        }

        private Task<string> SaveImage(byte[] image, long userId, DateTime logDatetime, uint deviceCode, string deviceId, string logTime)
        {
            return Task.Run(() =>
            {
                try
                {
                    var directory = Path.Combine(Directory.GetCurrentDirectory() ?? string.Empty,
                       $@"LogPic\{deviceId}\{logTime}\{logDatetime.Year}\{logDatetime.Month}\{logDatetime.Day}");
                    //var startupPath = System.IO.Directory.GetCurrentDirectory();
                    //var directory = Path.Combine(startupPath, path);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var fileName =
                        $"{userId}-{logDatetime:HH-mm-ss}({deviceCode}).jpg";


                    //TODO:For Linux?
                    //var fs = new FileStream(fileName, FileMode.Create);
                    //foreach (var t in image)
                    //{
                    //    fs.WriteByte(t);
                    //}


                    using (var ms = new MemoryStream(image))
                    {
                        using var img = Image.FromStream(ms);
                        img.Save(Path.Combine(directory, fileName), ImageFormat.Jpeg);
                    }

                    return Path.Combine(directory, fileName);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            });
        }
    }
}