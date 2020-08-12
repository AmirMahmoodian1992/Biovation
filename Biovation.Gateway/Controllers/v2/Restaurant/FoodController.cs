using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Models.RestaurantModels;
using Biovation.CommonClasses.Service;
using Biovation.CommonClasses.Service.RestaurantServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using Method = RestSharp.Method;

namespace Biovation.Gateway.Controllers.v2.Restaurant
{
    [Route("biovation/api/[controller]")]
    public class FoodController : Controller
    {
        private readonly FoodService _foodService;
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;

        private readonly RestClient _restClient;

        public FoodController(FoodService foodService, TaskService taskService, DeviceService deviceService)
        {
            _foodService = foodService;
            _taskService = taskService;
            _deviceService = deviceService;
            _restClient = new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/");
        }

        [HttpGet]
        [Route("{id}")]
        public Task<List<Food>> GetFoods(int id = default, int taskItemId = default)
        {
            throw null;
        }

        [HttpPost]
        public Task<IActionResult> AddFoods([FromBody]List<Food> foods)
        {
            throw null;
        }

        [HttpPut]
        [Route("{deviceId}")]
        public Task<IActionResult> SendFoodsToDevice([FromBody]List<int> ids, int deviceId = default)
        {
            throw null;
        }

        [HttpDelete]
        [Route("{deviceId}")]
        public Task<IActionResult> DeleteFoodsFromDevice([FromBody]List<int> ids, int deviceId = default)
        {
            throw null;
        }
    }
}
