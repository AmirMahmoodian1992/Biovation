using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Models.RestaurantModels;
using Biovation.CommonClasses.Service;
using Biovation.CommonClasses.Service.RestaurantServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Biovation.Gateway.Controllers.v2.Restaurant
{
    [Route("biovation/api/[controller]")]
    public class ServeLogController : Controller
    {
        private readonly ServeLogService _serveLogService;
        private readonly DeviceService _deviceService;
        private readonly TaskService _taskService;

        private readonly RestClient _restClient;

        public ServeLogController(ServeLogService serveLogService, DeviceService deviceService, TaskService taskService)
        {
            _serveLogService = serveLogService;
            _deviceService = deviceService;
            _taskService = taskService;
            _restClient = new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/");
        }

        [HttpGet]
        [Route("{id}")]
        public Task<IActionResult> GetServeLogs(int id = default, int userId = default, int foodId = default, int mealId = default, int deviceId = default)
        {
            throw null;
        }


        [HttpPost]
        public Task<IActionResult> AddServeLogs([FromBody]List<ServeLog> serveLogs, int taskItemId = default)
        {
            throw null;
        }

        [HttpPut]
        [Route("{deviceId}")]
        public Task<IActionResult> SendServeLogsDataToDevice(int deviceId = default)
        {
            throw null;

        }
    }
}
