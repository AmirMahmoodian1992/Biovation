﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Biovation.Server.Controllers.v1
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
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
        [HttpGet]
        public Task<List<Log>> Logs()
        {

            return _commonLogService.Logs();
        }

        [HttpGet]
        public Task<List<Log>> Logs(DateTime fromDate, DateTime toDate)
        {
            return _commonLogService.Logs(fromDate:fromDate, toDate:toDate);
        }

        [HttpPost]
        public Task<List<Log>> SelectSearchedOfflineLogs([FromBody]DeviceTraffic dTraffic)
        {
            return _commonLogService.SelectSearchedOfflineLogs(dTraffic);
        }

        [HttpPost]
        public Task<List<Log>> SelectSearchedOfflineLogsWithPaging([FromBody]DeviceTraffic dTraffic)
        {
            //return _commonLogService.SelectSearchedOfflineLogs(dTraffic);
            return _commonLogService.SelectSearchedOfflineLogsWithPaging(dTraffic);
        }

        [HttpPost]
        public Task<ResultViewModel> LogsOfDevice(int deviceId)
        {
            return Task.Run(async () =>
            {
                var creatorUser = _userService.GetUsers(userId:123456789, withPicture:false).FirstOrDefault();

                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = TaskTypes.GetServeLogs,
                    Priority = TaskPriorities.Medium,
                    TaskItems = new List<TaskItem>(),

                };

                var device = _commonDeviceService.GetDevice(deviceId);
                var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogs", Method.POST);
                restRequest.AddJsonBody(device.Code);
                restRequest.AddQueryParameter("taskId", task.Id.ToString());


                var result = await _restClient.ExecuteTaskAsync<ResultViewModel>(restRequest);
                //_communicationManager.CallRest(
                //    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogs", "Post", null,
                //    $"{device.Code}");
                return result.IsSuccessful && result.StatusCode == HttpStatusCode.OK ? result.Data : new ResultViewModel { Validate = 0, Message = result.ErrorMessage };
            });
        }

        [HttpPost]
        public Task<ResultViewModel> LogsOfDevice(int deviceId, DateTime fromDate, DateTime toDate)
        {
            return Task.Run(async () =>
            {
                var device = _commonDeviceService.GetDevice(deviceId);

                var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogsOfPeriod", Method.GET);
                restRequest.AddQueryParameter("code", device.Code.ToString());
                restRequest.AddQueryParameter("fromDate", fromDate.ToString(CultureInfo.InvariantCulture));
                restRequest.AddQueryParameter("toDate", toDate.ToString(CultureInfo.InvariantCulture));

                var result = await _restClient.ExecuteTaskAsync<ResultViewModel>(restRequest);
                //var parameters = new List<object> { $"code={device.Code}", $"fromDate={fromDate}", $"toDate={toDate}" };
                //        _communicationManager.CallRest($"/biovation/api/{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogsOfPeriod", "Post", null,
                //JsonConvert.SerializeObject(parameters));

                //_communicationManager.CallRest($"/biovation/api/{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogsOfPeriod", "Get", parameters);

                return result.IsSuccessful && result.StatusCode == HttpStatusCode.OK ? result.Data : new ResultViewModel { Validate = 0, Message = result.ErrorMessage };
            });
        }

        [HttpGet]
        public Task<List<Log>> OfflineLogsOfDevice(uint deviceId)
        {
            return _commonLogService.Logs(deviceId: (int)deviceId);
        }

        [HttpGet]
        public Task<List<Log>> OfflineLogsOfDevice(uint deviceId, DateTime fromDate, DateTime toDate)
        {
            return _commonLogService.Logs(deviceId:(int)deviceId, fromDate:fromDate, toDate:toDate);
        }

        [HttpGet]
        public Task<byte[]> GetImage(long id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var result = await _commonLogService.GetImage(id);
                    return result;
                }
                catch (Exception)
                {
                    return new byte[0];
                }
            });
        }

        [HttpGet]
        public Task<List<Log>> LogsOfUser(int userId)
        {
            return _commonLogService.Logs(userId:userId);
        }

        [HttpGet]
        public Task<List<Log>> LogsOfUser(int userId, DateTime fromDate, DateTime toDate)
        {
            return _commonLogService.Logs(userId:userId, fromDate:fromDate, toDate:toDate);
        }

        [HttpPost]
        public Task<ResultViewModel> ConvertOfflineLogs(long userId, string dTraffic)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<DeviceTraffic>(dTraffic);
                    obj.OnlineUserId = userId;
                    obj.State = false;
                    var logs = await _commonLogService.SelectSearchedOfflineLogs(obj);
                    //var logs = logsAwaiter.Where(w => !w.SuccessTransfer).ToList();
                    await Task.Run(() => { _commonLogService.TransferLogBulk(logs); });
                    return new ResultViewModel { Validate = 1, Code = logs.Count, Message = logs.Count.ToString() };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception.Message);
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }

        [HttpPost]
        public Task<ResultViewModel> ConvertOfflineAllLogs(long userId, string dTraffic)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<DeviceTraffic>(dTraffic);
                    obj.OnlineUserId = userId;
                    obj.State = null;
                    var logs = await _commonLogService.SelectSearchedOfflineLogs(obj);
                    //var logs = logsAwaiter.ToList();
                    await Task.Run(() => { _commonLogService.TransferLogBulk(logs); });

                    return new ResultViewModel { Validate = 1, Message = logs.Count.ToString() };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception.Message);
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };
                }
            });
        }

        [HttpPost]
        public Task<List<ResultViewModel>> ClearLogOfDevice(string deviceIds, string fromDate, string toDate)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var deviceId = JsonConvert.DeserializeObject<int[]>(deviceIds);
                    var result = new List<ResultViewModel>();
                    for (var i = 0; i < deviceId.Length; i++)
                    {
                        var device = _commonDeviceService.GetDevice(deviceId[i]);
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

                        var restResult = await _restClient.ExecuteTaskAsync<ResultViewModel>(restRequest);

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
    }
}