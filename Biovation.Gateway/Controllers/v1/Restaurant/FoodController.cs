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

namespace Biovation.Gateway.Controllers.v1.Restaurant
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
        [Route("GetFoods")]
        public Task<List<Food>> GetFoods(int foodId = default, int taskItemId = default)
        {
            if (taskItemId == default)
                return _foodService.GetFoods(foodId);

            return Task.Run(async () =>
            {
                var taskItem = await _taskService.GetTaskItem(taskItemId);
                taskItem.Status = TaskStatuses.Done;
                taskItem.Result = JsonConvert.SerializeObject(new ResultViewModel
                { Validate = 1, Message = $"Foods retrieved from server. Request from device: {taskItem.DeviceId}" });
                var unused = _taskService.UpdateTaskStatus(taskItem);

                if (taskItem.Data is null)
                    return await _foodService.GetFoods();

                var foodIds = JsonConvert.DeserializeObject<List<int>>(taskItem.Data);

                if (foodIds is null || foodIds.Count <= 0)
                    return await _foodService.GetFoods();

                var foods = new List<Food>();
                foreach (var id in foodIds)
                    foods.Add(_foodService.GetFoods(id).Result.FirstOrDefault());

                return foods;
            });
        }

        [HttpPost]
        [Route("AddFoods")]
        public Task<List<ResultViewModel>> AddFoods(List<Food> foods)
        {
            return _foodService.ModifyFoods(foods);
        }

        [HttpPost]
        [Route("SendFoodsToDevice")]
        public Task<List<ResultViewModel>> SendFoodsToDevice([FromBody]List<int> foodIds, int deviceId = default)
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
                            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Food/SendFoodsToDevice", Method.POST);
                        if (deviceId != default)
                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                        if (foodIds != null)
                            restRequest.AddJsonBody(foodIds);

                        var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

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
        [Route("DeleteFoodsFromDevice")]
        public Task<List<ResultViewModel>> DeleteFoodsFromDevice([FromBody]List<int> foodIds, int deviceId = default)
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
                            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Food/DeleteFoodsFromDevice", Method.POST);
                        if (deviceId != default)
                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                        if (foodIds != null)
                            restRequest.AddJsonBody(foodIds);

                        var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);

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
