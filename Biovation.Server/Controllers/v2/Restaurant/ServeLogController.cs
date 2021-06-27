//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Biovation.CommonClasses.Manager;
//using Biovation.Domain.RestaurantModels;
//using Biovation.Service;
//using Biovation.Service.RestaurantServices;
//using Biovation.Service.SQL.v1;
//using Microsoft.AspNetCore.Mvc;
//using RestSharp;

//namespace Biovation.Server.Controllers.v2.Restaurant
//{
//    [Route("biovation/api/v{version:apiVersion}/[controller]")]
//    [ApiVersion("2.0")]
//    public class ServeLogController : ControllerBase
//    {
//        private readonly ServeLogService _serveLogService;
//        private readonly DeviceService _deviceService;
//        private readonly TaskService _taskService;

//        private readonly RestClient _restClient;

//        public ServeLogController(ServeLogService serveLogService, DeviceService deviceService, TaskService taskService)
//        {
//            _serveLogService = serveLogService;
//            _deviceService = deviceService;
//            _taskService = taskService;
//            _restClient = new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/");
//        }

//        [HttpGet]
//        [Route("{id}")]
//        public Task<IActionResult> GetServeLogs(int id = default, int userId = default, int foodId = default, int mealId = default, int deviceId = default)
//        {
//            throw null;
//        }


//        [HttpPost]
//        public Task<IActionResult> AddServeLogs([FromBody]List<ServeLog> serveLogs, int taskItemId = default)
//        {
//            throw null;
//        }

//        [HttpPut]
//        [Route("{deviceId}")]
//        public Task<IActionResult> SendServeLogsDataToDevice(int deviceId = default)
//        {
//            throw null;

//        }
//    }
//}
