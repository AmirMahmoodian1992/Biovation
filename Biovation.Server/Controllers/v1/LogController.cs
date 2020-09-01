using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Constants;
using Biovation.Service;
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

        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;

        public LogController(DeviceService deviceService, UserService userService, LogService logService, TaskTypes taskTypes, TaskPriorities taskPriorities)
        {
            _userService = userService;
            _commonLogService = logService;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _commonDeviceService = deviceService;
            _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/").UseSerializer(() => new RestRequestJsonSerializer());
        }

        //for routing problem(same address)
        //[HttpGet]
        //[Route("Logs")]
        //public Task<List<Log>> Logs()
        //{
        //    return _commonLogService.GetOfflineLogs();
        //}

        [HttpGet]
        [Route("Logs")]
        public Task<List<Log>> Logs(DateTime? fromDate = null , DateTime? toDate = null)
        {
            return fromDate switch
            {
                null when toDate is null => _commonLogService.GetOfflineLogs(),
                null => _commonLogService.GetOfflineLogsOfPeriod(DateTime.MinValue, (DateTime) toDate),
                _ => toDate is null
                    ? _commonLogService.GetOfflineLogsOfPeriod((DateTime) fromDate, DateTime.MaxValue)
                    : _commonLogService.GetOfflineLogsOfPeriod((DateTime) fromDate, (DateTime) toDate)
            };
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

        //for routing problem(same address)
        //[HttpPost]
        //[Route("LogsOfDevice")]
        //public Task<ResultViewModel> LogsOfDevice(int deviceId)
        //{
        //    return Task.Run(async () =>
        //    {
        //        var creatorUser = _userService.GetUser(123456789, false);

        //        var task = new TaskInfo
        //        {
        //            CreatedAt = DateTimeOffset.Now,
        //            CreatedBy = creatorUser,
        //            TaskType = TaskTypes.GetServeLogs,
        //            Priority = TaskPriorities.Medium,
        //            TaskItems = new List<TaskItem>(),

        //        };

        //        var device = _commonDeviceService.GetDeviceInfo(deviceId);
        //        var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogs", Method.POST);
        //        restRequest.AddJsonBody(device.Code);
        //        restRequest.AddQueryParameter("taskId", task.Id.ToString());


        //        var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
        //        //_communicationManager.CallRest(
        //        //    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogs", "Post", null,
        //        //    $"{device.Code}");
        //        return result.IsSuccessful && result.StatusCode == HttpStatusCode.OK ? result.Data : new ResultViewModel { Validate = 0, Message = result.ErrorMessage };
        //    });
        //}

        [HttpPost]
        [Route("LogsOfDevice")]
        public Task<ResultViewModel> LogsOfDevice(int deviceId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate is null && toDate is null)
            {
                return Task.Run(async () =>
                {
                    var creatorUser = _userService.GetUser(123456789, false);

                    var task = new TaskInfo
                    {
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = creatorUser,
                        TaskType = _taskTypes.GetServeLogs,
                        Priority = _taskPriorities.Medium,
                        TaskItems = new List<TaskItem>(),

                    };

                    var device = _commonDeviceService.GetDeviceInfo(deviceId);
                    var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogs", Method.POST);
                    restRequest.AddJsonBody(device.Code);
                    restRequest.AddQueryParameter("taskId", task.Id.ToString());


                    var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                    //_communicationManager.CallRest(
                    //    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogs", "Post", null,
                    //    $"{device.Code}");
                    return result.IsSuccessful && result.StatusCode == HttpStatusCode.OK ? result.Data : new ResultViewModel { Validate = 0, Message = result.ErrorMessage };
                });
            }

            return Task.Run(async () =>
            {
                var device = _commonDeviceService.GetDeviceInfo(deviceId);

                var restRequest =
                    new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogsOfPeriod",
                        Method.GET);

                if (fromDate is null && toDate is null)
                {
                    restRequest.AddQueryParameter("fromDate", DateTime.MinValue.ToString(CultureInfo.InvariantCulture));
                    restRequest.AddQueryParameter("toDate", DateTime.MaxValue.ToString(CultureInfo.InvariantCulture));
                }
                else if (fromDate is null)
                {
                    restRequest.AddQueryParameter("fromDate", (DateTime.MinValue.ToString(CultureInfo.InvariantCulture)));
                    restRequest.AddQueryParameter("toDate", ((DateTime)toDate).ToString(CultureInfo.InvariantCulture)); restRequest.AddQueryParameter("toDate", ((DateTime)toDate).ToString(CultureInfo.InvariantCulture));
                }
                else if (toDate is null)
                {
                    restRequest.AddQueryParameter("toDate", DateTime.MaxValue.ToString(CultureInfo.InvariantCulture));
                    restRequest.AddQueryParameter("fromDate", ((DateTime)fromDate).ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    restRequest.AddQueryParameter("fromDate", ((DateTime)fromDate).ToString(CultureInfo.InvariantCulture));
                    restRequest.AddQueryParameter("toDate", ((DateTime)toDate).ToString(CultureInfo.InvariantCulture));
                }

                restRequest.AddQueryParameter("code", device.Code.ToString());
                    

                var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
                //var parameters = new List<object> { $"code={device.Code}", $"fromDate={fromDate}", $"toDate={toDate}" };
                //        _communicationManager.CallRest($"/biovation/api/{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogsOfPeriod", "Post", null,
                //JsonConvert.SerializeObject(parameters));

                //_communicationManager.CallRest($"/biovation/api/{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogsOfPeriod", "Get", parameters);

                return result.IsSuccessful && result.StatusCode == HttpStatusCode.OK
                    ? result.Data
                    : new ResultViewModel {Validate = 0, Message = result.ErrorMessage};
            });
        }

        //for routing problem(same address)
        //[HttpGet]
        //[Route("OfflineLogsOfDevice")]
        //public Task<List<Log>> OfflineLogsOfDevice(uint deviceId)
        //{
        //    return _commonLogService.GetOfflineLogsByDeviceId(deviceId);
        //}

        [HttpGet]
        [Route("OfflineLogsOfDevice")]
        public Task<List<Log>> OfflineLogsOfDevice(uint deviceId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return fromDate switch
            {
                null when toDate is null => _commonLogService.GetOfflineLogsByDeviceId(deviceId),
                null => _commonLogService.GetOfflineLogsOfPeriodByDeviceId(deviceId, DateTime.MinValue, (DateTime)toDate),
                _ => toDate is null
                    ? _commonLogService.GetOfflineLogsOfPeriodByDeviceId(deviceId, (DateTime)fromDate, DateTime.MaxValue)
                    : _commonLogService.GetOfflineLogsOfPeriodByDeviceId(deviceId ,(DateTime)fromDate, (DateTime)toDate)
            };
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

        //for routing problem(same address)
        //[HttpGet]
        //[Route("LogsOfUser")]
        //public Task<List<Log>> LogsOfUser(int userId)
        //{
        //    return _commonLogService.GetOfflineLogsByUserId(userId);
        //}

        [HttpGet]
        [Route("LogsOfUser")]
        public Task<List<Log>> LogsOfUser(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {

            return fromDate switch
            {
                null when toDate is null => _commonLogService.GetOfflineLogsByUserId(userId),
                null => _commonLogService.GetOfflineLogsOfPeriodByUserId(userId, DateTime.MinValue, (DateTime)toDate),
                _ => toDate is null
                    ? _commonLogService.GetOfflineLogsOfPeriodByUserId(userId, (DateTime)fromDate, DateTime.MaxValue)
                    : _commonLogService.GetOfflineLogsOfPeriodByUserId(userId, (DateTime)fromDate, (DateTime)toDate)
            };
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
    }
}