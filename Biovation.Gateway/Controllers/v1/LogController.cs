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

namespace Biovation.Gateway.Controllers.v1
{
    public class LogController : ControllerBase
    {
        private readonly DeviceService _commonDeviceService = new DeviceService();
        private readonly UserService _userService = new UserService();

        private readonly LogService _commonLogService = new LogService();
        private readonly RestClient _restClient;

        public LogController()
        {
            _restClient = (RestClient)new RestClient($"http://localhost:{ConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }

        [HttpGet]
        [Route("Logs")]
        public Task<List<Log>> Logs()
        {

            return _commonLogService.GetOfflineLogs();
        }

        [HttpGet]
        [Route("Logs")]///////////////////////////////
        public Task<List<Log>> Logs(DateTime fromDate, DateTime toDate)
        {
            return _commonLogService.GetOfflineLogsOfPeriod(fromDate, toDate);
        }

        [HttpPost]
        [Route("SelectSearchedOfflineLogs")]
        public Task<List<Log>> SelectSearchedOfflineLogs([FromBody]DeviceTraffic dTraffic)
        {
            return _commonLogService.SelectSearchedOfflineLogs(dTraffic);
        }

        [HttpPost]
        [Route("SelectSearchedOfflineLogsWithPaging")]
        public Task<List<Log>> SelectSearchedOfflineLogsWithPaging([FromBody]DeviceTraffic dTraffic)
        {
            //return _commonLogService.SelectSearchedOfflineLogs(dTraffic);
            return _commonLogService.SelectSearchedOfflineLogsWithPaging(dTraffic);
        }

        [HttpPost]
        [Route("LogsOfDevice")]
        public Task<ResultViewModel> LogsOfDevice(int deviceId)
        {
            return Task.Run(async () =>
            {
                var creatorUser = _userService.GetUser(123456789, false);

                var task = new TaskInfo
                {
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = creatorUser,
                    TaskType = TaskTypes.GetServeLogs,
                    Priority = TaskPriorities.Medium,
                    TaskItems = new List<TaskItem>(),

                };

                var device = _commonDeviceService.GetDeviceInfo(deviceId);
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
        [Route("LogsOfDevice")]////////////////////////////
        public Task<ResultViewModel> LogsOfDevice(int deviceId, DateTime fromDate, DateTime toDate)
        {
            return Task.Run(async () =>
            {
                var device = _commonDeviceService.GetDeviceInfo(deviceId);

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
        [Route("OfflineLogsOfDevice")]
        public Task<List<Log>> OfflineLogsOfDevice(uint deviceId)
        {
            return _commonLogService.GetOfflineLogsByDeviceId(deviceId);
        }

        [HttpGet]
        [Route("OfflineLogsOfDevice")]///////////////////////////
        public Task<List<Log>> OfflineLogsOfDevice(uint deviceId, DateTime fromDate, DateTime toDate)
        {
            return _commonLogService.GetOfflineLogsOfPeriodByDeviceId(deviceId, fromDate, toDate);
        }

        [HttpGet]
        [Route("GetImage")]public Task<byte[]> GetImage(long id)
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
        [Route("LogsOfUser")]
        public Task<List<Log>> LogsOfUser(int userId)
        {
            return _commonLogService.GetOfflineLogsByUserId(userId);
        }

        [HttpGet]
        [Route("LogsOfUser")]/////////////////////////////
        public Task<List<Log>> LogsOfUser(int userId, DateTime fromDate, DateTime toDate)
        {
            return _commonLogService.GetOfflineLogsOfPeriodByUserId(userId, fromDate, toDate);
        }

        [HttpPost]
        [Route("ConvertOfflineLogs")]
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
        [Route("ConvertOfflineAllLogs")]
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
        [Route("ClearLogOfDevice")]
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
                        var device = _commonDeviceService.GetDeviceInfo(deviceId[i]);
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