using System.Collections.Generic;
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

namespace Biovation.Gateway.Controllers.v1.Restaurant
{
    [Route("[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly RestaurantService _restaurantService = new RestaurantService();
        private readonly DeviceService _deviceService = new DeviceService();
        private readonly TaskService _taskService = new TaskService();

        private readonly RestClient _restClient;

        public RestaurantController()
        {
            _restClient = new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/");
        }

        [HttpGet]
        [Route("GetRestaurants")]
        public Task<List<CommonClasses.Models.RestaurantModels.Restaurant>> GetRestaurants(int restaurantId = default, int taskItemId = default)
        {
            if (taskItemId == default)
                return _restaurantService.GetRestaurants(restaurantId);

            return Task.Run(async () =>
            {
                var taskItem = await _taskService.GetTaskItem(taskItemId);
                taskItem.Status = TaskStatuses.Done;
                taskItem.Result = JsonConvert.SerializeObject(new ResultViewModel
                { Validate = 1, Message = $"Restaurants retrieved from server. Request from device: {taskItem.DeviceId}" });
                var unused = _taskService.UpdateTaskStatus(taskItem);

                if (taskItem.Data is null)
                    return await _restaurantService.GetRestaurants();

                var restaurantIds = JsonConvert.DeserializeObject<List<int>>(taskItem.Data);

                if (restaurantIds is null || restaurantIds.Count <= 0)
                    return await _restaurantService.GetRestaurants();

                var restaurants = new List<CommonClasses.Models.RestaurantModels.Restaurant>();
                foreach (var id in restaurantIds)
                    restaurants.Add(_restaurantService.GetRestaurants(id).Result.FirstOrDefault());

                return restaurants;
            });
        }

        [HttpPost]
        [Route("ModifyRestaurants")]
        public Task<List<ResultViewModel>> ModifyRestaurants(List<CommonClasses.Models.RestaurantModels.Restaurant> restaurants)
        {
            return _restaurantService.ModifyRestaurants(restaurants);
        }

        [HttpPost]
        [Route("SendRestaurantsToDevice")]
        public Task<List<ResultViewModel>> SendRestaurantsToDevice([FromBody]List<int> restaurantIds, int deviceId = default)
        {
            return Task.Run(() =>
            {
                var results = new List<ResultViewModel>();
                var deviceBrands = _deviceService.GetDeviceBrands();

                var tasks = new List<Task>();
                foreach (var deviceBrand in deviceBrands)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var restRequest =
                            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Restaurant/SendRestaurantsToDevice", Method.POST);
                        if (deviceId != default)
                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                        if (restaurantIds != null)
                            restRequest.AddJsonBody(restaurantIds);

                        var result = await _restClient.ExecuteTaskAsync<ResultViewModel>(restRequest);
                        lock (results)
                        {
                            if (result.StatusCode == HttpStatusCode.OK && result.Data != null)
                                results.Add(new ResultViewModel
                                {
                                    Id = result.Data.Id,
                                    Code = result.Data.Code,
                                    Validate = result.IsSuccessful && result.Data.Validate == 1 ? 1 : 0,
                                    Message = deviceBrand.Name
                                });
                        }
                    }));
                }

                if (tasks.Count > 0)
                    Task.WaitAll(tasks.ToArray());
                else
                    Logger.Log("Cannot get device brands");

                return results;
            });
        }

        [HttpPost]
        [Route("DeleteRestaurantsFromDevice")]
        public Task<List<ResultViewModel>> DeleteRestaurantsFromDevice([FromBody]List<int> restaurantIds, int deviceId = default)
        {
            return Task.Run(() =>
            {
                var results = new List<ResultViewModel>();
                var deviceBrands = _deviceService.GetDeviceBrands();

                var tasks = new List<Task>();
                foreach (var deviceBrand in deviceBrands)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var restRequest =
                            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Restaurant/DeleteRestaurantsFromDevice", Method.POST);
                        if (deviceId != default)
                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                        if (restaurantIds != null)
                            restRequest.AddJsonBody(restaurantIds);

                        var result = await _restClient.ExecuteTaskAsync<ResultViewModel>(restRequest);
                        lock (results)
                        {
                            if (result.StatusCode == HttpStatusCode.OK && result.Data != null)
                                results.Add(new ResultViewModel
                                {
                                    Id = result.Data.Id,
                                    Code = result.Data.Code,
                                    Validate = result.IsSuccessful && result.Data.Validate == 1 ? 1 : 0,
                                    Message = deviceBrand.Name
                                });
                        }
                    }));
                }

                Task.WaitAll(tasks.ToArray());
                return results;
            });
        }
    }
}
