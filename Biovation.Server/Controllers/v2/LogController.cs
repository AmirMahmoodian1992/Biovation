using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Servers;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Biovation.Server.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [Authorize]
    public class LogController : Controller
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

        //we should consider the without parameter input version of log
        // and handle searchOfflineLogs with paging or not with  [FromBody]DeviceTraffic dTraffic


        [HttpGet]
        public Task<ResultViewModel<PagingResult<Log>>> Logs(int id = default, int deviceId = default,
                        int userId = default, bool successTransfer = default, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = default,
                        int pageSize = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _logService.Logs(id, deviceId, userId, successTransfer, fromDate, toDate, pageNumber, pageSize, token));
        }

        [HttpGet]
        [Route("Image")]
        public Task<byte[]> GetImage(long id)
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

        [HttpDelete]
        public Task<List<ResultViewModel>> ClearLogOfDevice(string deviceIds, string fromDate, string toDate)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(async () =>
            {
                try
                {
                    var deviceId = JsonConvert.DeserializeObject<int[]>(deviceIds);
                    var result = new List<ResultViewModel>();
                    for (var i = 0; i < deviceId.Length; i++)
                    {
                        var device = _deviceService.GetDevice(deviceId[i],token:token).Data;
                        if (device == null)
                        {
                            Logger.Log($"DeviceId {deviceId[i]} does not exist.");
                            result.Add(new ResultViewModel
                            { Validate = 0, Message = $"DeviceId {deviceId[i]} does not exist.", Id = deviceIds[i] });
                            continue;
                        }

                        var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Log/ClearLog", Method.POST);
                        restRequest.AddQueryParameter("code", device.Code.ToString());
                        restRequest.AddQueryParameter("fromDate", fromDate);
                        restRequest.AddQueryParameter("toDate", toDate);

                        var restResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

                        //var address = _localBioAddress +
                        //              $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}Log/ClearLog?code={device.Code}&fromDate={fromDate}&toDate={toDate}";
                        //var data = _restCall.CallRestAsync(address, null, null, "POST");
                        //var res = JsonConvert.DeserializeObject<ResultViewModel>(data);
                        if (!restResult.IsSuccessful || restResult.StatusCode != HttpStatusCode.OK) continue;
                        restResult.Data.Id = deviceId[i];
                        result.Add(restResult.Data);
                    }

                    return result;
                }
                catch (Exception)
                {
                    return new List<ResultViewModel> { new ResultViewModel { Validate = 0, Message = "error" } };
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