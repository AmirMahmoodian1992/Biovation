//using System.Collections.Generic;
//using System.Net;
//using System.Threading.Tasks;
//using System.Web.Http;
//using Biovation.CommonClasses.Manager;
//using Biovation.CommonClasses.Models;
//using Biovation.CommonClasses.Models.ConstantValues;
//using Biovation.CommonClasses.Models.RestaurantModels;
//using Biovation.CommonClasses.Service;
//using Biovation.CommonClasses.Service.RestaurantServices;
//using Newtonsoft.Json;
//using RestSharp;

//namespace Biovation.Server.Controllers.v1.Restaurant
//{
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    //[ApiVersion("1.0")]
    //public class ServeLogController : Controller
    //{
    //    private readonly ServeLogService _serveLogService;
    //    private readonly DeviceService _deviceService;
    //    private readonly TaskStatuses _taskStatuses;
    //    private readonly TaskService _taskService;

    //    private readonly RestClient _restClient;

    //    public ServeLogController(ServeLogService serveLogService, DeviceService deviceService, TaskService taskService, TaskStatuses taskStatuses)
    //    {
    //        _serveLogService = serveLogService;
    //        _deviceService = deviceService;
    //        _taskService = taskService;
    //        _taskStatuses = taskStatuses;
    //        _restClient = new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/");
    //    }

//        [HttpGet]
//        public Task<List<ServeLog>> GetServeLogs(int serveLogId = default)
//        {
//            return _serveLogService.GetServeLogs(serveLogId);
//        }

//        [HttpGet]
//        public Task<List<ServeLog>> GetServeLogsByReservationId(int userId = default, int foodId = default, int mealId = default, int deviceId = default)
//        {
//            return _serveLogService.GetServeLogsByReservationId(userId, foodId, mealId, deviceId);
//        }

        //[HttpPost]
        //[Route("AddServeLogs")]
        //public Task<ResultViewModel> AddServeLogs([FromBody]List<ServeLog> serveLogs, int taskItemId = default)
        //{
        //    var result = _serveLogService.AddServeLogs(serveLogs);
        //    if (taskItemId != default)
        //        Task.Run(async () =>
        //        {
        //            var taskItem = await _taskService.GetTaskItem(taskItemId);
        //            taskItem.Result = JsonConvert.SerializeObject(result);
        //            taskItem.Status = _taskStatuses.Done;
        //            await _taskService.UpdateTaskStatus(taskItem);
        //        });

//            return result;
//        }

//        [HttpPost]
//        public Task<List<ResultViewModel>> SendServeLogsDataToDevice(int deviceId = default)
//        {
//            return Task.Run(() =>
//            {
//                var results = new List<ResultViewModel>();
//                var deviceBrands = _deviceService.GetDeviceBrands();

//                var tasks = new List<Task>();
//                foreach (var deviceBrand in deviceBrands)
//                {
//                    tasks.Add(Task.Run(async () =>
//                    {
//                        var restRequest =
//                            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}ServeLog/SendServeLogsDataToDevice");
//                        if (deviceId != default)
//                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
//                        var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
//                        lock (results)
//                        {
//                            if (result.StatusCode == HttpStatusCode.OK && result.Data != null)
//                                results.Add(new ResultViewModel
//                                {
//                                    Id = result.Data.Id,
//                                    Code = result.Data.Code,
//                                    Validate = result.IsSuccessful && result.Data.Validate == 1 ? 1 : 0,
//                                    Message = deviceBrand.Name
//                                });
//                        }
//                    }));
//                }

//                Task.WaitAll(tasks.ToArray());
//                return results;
//            });
//        }
//    }
//}
