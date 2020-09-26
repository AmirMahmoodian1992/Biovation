//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Biovation.CommonClasses.Manager;
//using Biovation.Service;
//using Biovation.Service.RestaurantServices;
//using Biovation.Service.SQL.v1;
//using Microsoft.AspNetCore.Mvc;
//using RestSharp;

//namespace Biovation.Server.Controllers.v2.Restaurant
//{
//    [Route("biovation/api/v{version:apiVersion}/[controller]")]
//    [ApiVersion("2.0")]
//    public class RestaurantController : Controller
//    {
//        private readonly RestaurantService _restaurantService;
//        private readonly DeviceService _deviceService;
//        private readonly TaskService _taskService;

//        private readonly RestClient _restClient;

//        public RestaurantController(RestaurantService restaurantService, DeviceService deviceService, TaskService taskService)
//        {
//            _restaurantService = restaurantService;
//            _deviceService = deviceService;
//            _taskService = taskService;
//            _restClient = new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/");
//        }

//        [HttpGet]
//        [Route("{id}")]
//        public Task<IActionResult> GetRestaurants(int id = default, int taskItemId = default)
//        {
//            throw null;
//        }

//        [HttpPost]
//        public Task<IActionResult> AddRestaurants([FromBody]List<Domain.RestaurantModels.Restaurant> restaurants)
//        {
//            throw null;
//        }

//        [HttpPut]
//        public Task<IActionResult> ModifyRestaurants([FromBody]List<Domain.RestaurantModels.Restaurant> restaurants)
//        {
//            throw null;
//        }

//        //batch delete
//        [HttpPost]
//        [Route("DeleteRestaurantsFromDevice/{deviceId}")]
//        public Task<IActionResult> DeleteRestaurantsFromDevice([FromBody]List<int> restaurantIds, int deviceId = default)
//        {
//            throw null;
//        }

        
//        [HttpDelete]
//        [Route("{deviceId}")]
//        public Task<IActionResult> DeleteRestaurantsFromDevice(int restaurantId, int deviceId = default)
//        {
//            throw null;
//        }

//        [HttpPut]
//        [Route("RestaurantsToDevice/{deviceId}")]
//        public Task<IActionResult> SendRestaurantsToDevice([FromBody]List<int> ids, int deviceId = default)
//        {
//            throw null;
//        }
//    }
//}
