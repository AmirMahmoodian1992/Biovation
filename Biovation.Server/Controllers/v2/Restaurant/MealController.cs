using System.Collections.Generic;
using System.Linq;
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
    public class MealController : Controller
    {
        private readonly MealService _mealService;
        private readonly TaskService _taskService;
        private readonly DeviceService _deviceService;

        private readonly RestClient _restClient;

        public MealController(MealService mealService, TaskService taskService, DeviceService deviceService)
        {
            _mealService = mealService;
            _taskService = taskService;
            _deviceService = deviceService;
            _restClient = new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/");
        }

        [HttpGet]
        [Route("{id}")]
        public Task<IActionResult> GetMeals(int id = default, int taskItemId = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("MealTimings/{id}")]
        public Task<IActionResult> GetMealTimings(int id, int timingId = default, int taskItemId = default)
        {
            throw null;
        }

        [HttpPost]
        public Task<IActionResult> AddMeals([FromBody]List<Meal> meals)
        {
            throw null;
        }

        [HttpPut]
        [Route("{deviceId}")]
        public Task<IActionResult> SendMealsToDevice([FromBody]List<int> ids, int deviceId = default)
        {
            throw null;
            ;
        }

        [HttpDelete]
        [Route("{deviceId}")]
        public Task<IActionResult> DeleteMealsFromDevice([FromBody]List<int> ids, int deviceId = default)
        {
            throw null;
        }

        [HttpPut]
        [Route("MealTimingsToDevice/{deviceId}")]
        public Task<IActionResult> SendMealTimingsToDevice([FromBody]List<int> timingIds, int deviceId = default)
        {
            throw null;
        }


        //batch delete
        [HttpPost]
        [Route("MealTimingsFromDevice/{deviceId}")]
        public Task<IActionResult> DeleteMealTimingsFromDevice([FromBody]List<int> timingIds, int deviceId = default)
        {
            throw null;
        }


        [HttpDelete]
        [Route("MealTimingsFromDevice/{timingId}/{deviceId}")]
        public Task<IActionResult> DeleteMealTimingsFromDevice(int timingId, int deviceId = default)
        {
            throw null;
        }
    }
}
