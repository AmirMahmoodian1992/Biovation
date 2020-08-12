﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Service;
using Biovation.CommonClasses.Service.RestaurantServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using Method = RestSharp.Method;

namespace Biovation.Gateway.Controllers.v2.Restaurant
{
    [Route("biovation/api/[controller]")]
    public class RestaurantController : Controller
    {
        private readonly RestaurantService _restaurantService;
        private readonly DeviceService _deviceService;
        private readonly TaskService _taskService;

        private readonly RestClient _restClient;

        public RestaurantController(RestaurantService restaurantService, DeviceService deviceService, TaskService taskService)
        {
            _restaurantService = restaurantService;
            _deviceService = deviceService;
            _taskService = taskService;
            _restClient = new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/");
        }

        [HttpGet]
        [Route("{id}")]
        public Task<IActionResult> GetRestaurants(int id = default, int taskItemId = default)
        {
            throw null;
        }

        [HttpPost]
        public Task<IActionResult> AddRestaurants([FromBody]List<CommonClasses.Models.RestaurantModels.Restaurant> restaurants)
        {
            throw null;
        }

        [HttpPut]
        public Task<IActionResult> ModifyRestaurants([FromBody]List<CommonClasses.Models.RestaurantModels.Restaurant> restaurants)
        {
            throw null;
        }

        //batch delete
        [HttpPost]
        [Route("DeleteRestaurantsFromDevice/{deviceId}")]
        public Task<IActionResult> DeleteRestaurantsFromDevice([FromBody]List<int> restaurantIds, int deviceId = default)
        {
            throw null;
        }

        
        [HttpDelete]
        [Route("{deviceId}")]
        public Task<IActionResult> DeleteRestaurantsFromDevice(int restaurantId, int deviceId = default)
        {
            throw null;
        }

        [HttpPut]
        [Route("RestaurantsToDevice/{deviceId}")]
        public Task<IActionResult> SendRestaurantsToDevice([FromBody]List<int> ids, int deviceId = default)
        {
            throw null;
        }
    }
}
