using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Servers;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biovation.CommonClasses.Extension;

namespace Biovation.Server.Controllers.v1
{
    [Route("biovation/api/v1/[controller]")]
    [ApiVersion("1.0")]
    public class LogController : Controller
    {
        private readonly UserService _userService;
        private readonly LogService _logService;
        private readonly DeviceService _commonDeviceService;
        private readonly RestClient _restClient;

        private readonly TaskTypes _taskTypes;
        private readonly TaskPriorities _taskPriorities;
        private readonly TokenGenerator _tokenGenerator;
        private readonly string _kasraAdminToken;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public LogController(DeviceService deviceService, UserService userService, LogService logService, TaskTypes taskTypes, TaskPriorities taskPriorities, RestClient restClient, TokenGenerator tokenGenerator, BiovationConfigurationManager biovationConfigurationManager)
        {
            _userService = userService;
            _logService = logService;
            _taskTypes = taskTypes;
            _taskPriorities = taskPriorities;
            _commonDeviceService = deviceService;
            _restClient = restClient;
            _tokenGenerator = tokenGenerator;
            _biovationConfigurationManager = biovationConfigurationManager;
            _kasraAdminToken = _biovationConfigurationManager.KasraAdminToken;
        }

        [HttpGet]
        [Route("Logs")]
        public Task<List<Log>> Logs()
        {
            return _logService.Logs(token: _kasraAdminToken);
        }

        [HttpGet]
        [Route("Logs")]
        public Task<List<Log>> LogsWithDate(DateTime fromDate, DateTime toDate)
        {
            return _logService.Logs(fromDate: fromDate, toDate: toDate, token: _kasraAdminToken);
        }

        [HttpPost]
        [Route("SelectSearchedOfflineLogs")]
        public Task<List<Log>> SelectSearchedOfflineLogs([FromBody] DeviceTraffic dTraffic)
        {
            return _logService.SelectSearchedOfflineLogs(dTraffic, token: _kasraAdminToken);
        }

        [HttpPost]
        [Route("SelectSearchedOfflineLogsWithPaging")]
        public Task<List<Log>> SelectSearchedOfflineLogsWithPaging([FromBody] DeviceTraffic dTraffic)
        {
            //return _logService.SelectSearchedOfflineLogs(dTraffic);
            return _logService.SelectSearchedOfflineLogsWithPaging(dTraffic, token: _kasraAdminToken);
        }

        //[HttpPost]
        //[Route("LogsOfDevice")]
        //public Task<ResultViewModel> LogsOfDevice(int deviceId)
        //{
        //    return Task.Run(async () =>
        //    {
        //        //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
        //        var creatorUser = HttpContext.GetUser();

        //        var task = new TaskInfo
        //        {
        //            CreatedAt = DateTimeOffset.Now,
        //            CreatedBy = creatorUser,
        //            TaskType = _taskTypes.GetServeLogs,
        //            Priority = _taskPriorities.Medium,
        //            TaskItems = new List<TaskItem>(),

        //        };

        //        var device = _commonDeviceService.GetDevice(deviceId, token: _kasraAdminToken);
        //        var restRequest = new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogs", Method.POST);
        //        restRequest.AddJsonBody(device.Code);
        //        restRequest.AddQueryParameter("taskId", task.Id.ToString());

        //        restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
        //        var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
        //        //_communicationManager.CallRest(
        //        //    $"/biovation/api/{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogs", "Post", null,
        //        //    $"{device.Code}");
        //        return result.IsSuccessful && result.StatusCode == HttpStatusCode.OK ? result.Data : new ResultViewModel { Validate = 0, Message = result.ErrorMessage };
        //    });
        //}

        [HttpPost]
        [Route("LogsOfDevice")]
        public Task<ResultViewModel> LogsOfDevice(int deviceId, DateTime? fromDate, DateTime? toDate)
        {
            return Task.Run(async () =>
            {
                var device = _commonDeviceService.GetDevice(deviceId, token: _kasraAdminToken);

                var restRequest =
                    new RestRequest($"{device.Brand.Name}/{device.Brand.Name}Device/RetrieveLogsOfPeriod",
                        Method.GET);

                if (fromDate is null && toDate is null)
                {
                    restRequest.AddQueryParameter("fromDate", DateTime.Now.AddYears(-15).ToString(CultureInfo.InvariantCulture));
                    restRequest.AddQueryParameter("toDate", DateTime.Now.AddYears(5).ToString(CultureInfo.InvariantCulture));
                }
                else if (fromDate is null)
                {
                    restRequest.AddQueryParameter("fromDate", (DateTime.Now.AddYears(-15).ToString(CultureInfo.InvariantCulture)));
                    restRequest.AddQueryParameter("toDate", ((DateTime)toDate).ToString(CultureInfo.InvariantCulture));
                }
                else if (toDate is null)
                {
                    restRequest.AddQueryParameter("toDate", DateTime.Now.AddYears(5).ToString(CultureInfo.InvariantCulture));
                    restRequest.AddQueryParameter("fromDate", ((DateTime)fromDate).ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    restRequest.AddQueryParameter("fromDate", ((DateTime)fromDate).ToString(CultureInfo.InvariantCulture));
                    restRequest.AddQueryParameter("toDate", ((DateTime)toDate).ToString(CultureInfo.InvariantCulture));
                }

                restRequest.AddQueryParameter("code", device.Code.ToString());

                restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
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

        [HttpGet]
        [Route("OfflineLogsOfDevice")]
        public Task<List<Log>> OfflineLogsOfDevice(uint deviceId)
        {
            return _logService.Logs(deviceId: (int)deviceId, token: _kasraAdminToken);
        }

        [HttpGet]
        [Route("OfflineLogsOfDevice")]
        public Task<List<Log>> OfflineLogsOfDeviceByDate(uint deviceId, DateTime fromDate, DateTime toDate)
        {
            return _logService.Logs(deviceId: (int)deviceId, fromDate: fromDate, toDate: toDate, token: _kasraAdminToken);
        }

        [HttpGet]
        [Route("GetImage")]
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

        [HttpGet]
        [Route("LogsOfUser")]
        public Task<List<Log>> LogsOfUser(int userId)
        {
            return _logService.Logs(userId: userId, token: _kasraAdminToken);
        }

        [HttpGet]
        [Route("LogsOfUser")]
        public Task<List<Log>> LogsOfUserWithDate(int userId, DateTime fromDate, DateTime toDate)
        {
            return _logService.Logs(userId: userId, fromDate: fromDate, toDate: toDate, token: _kasraAdminToken);
        }

        [HttpPost]
        [Route("ConvertOfflineLogs")]
        public Task<ResultViewModel> ConvertOfflineLogs(long userId, string dTraffic)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var token = _tokenGenerator.GenerateToken(_userService.GetUsers(userId, token: _kasraAdminToken)?.FirstOrDefault());
                    var obj = JsonConvert.DeserializeObject<DeviceTraffic>(dTraffic);
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

        [HttpPost]
        [Route("ConvertOfflineAllLogs")]
        public Task<ResultViewModel> ConvertOfflineAllLogs(long userId, string dTraffic)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var token = _tokenGenerator.GenerateToken(_userService.GetUsers(userId, token: _kasraAdminToken).FirstOrDefault());
                    var obj = JsonConvert.DeserializeObject<DeviceTraffic>(dTraffic);
                    obj.OnlineUserId = userId;
                    obj.State = null;
                    var logs = await _logService.SelectSearchedOfflineLogs(obj, token);
                    //var logs = logsAwaiter.ToList();
                    await Task.Run(() => { _logService.TransferLogBulk(logs,token); });

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
                        var device = _commonDeviceService.GetDevice(deviceId[i], token: _kasraAdminToken);
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

                        restRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
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