using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain.RestaurantModels;
using Biovation.Service;
using Biovation.Service.RestaurantServices;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Server.Controllers.v2.Restaurant
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
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
