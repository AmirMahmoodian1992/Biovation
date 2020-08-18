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

namespace Biovation.Server.Controllers.v1.Restaurant
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
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
        [Route("GetMeals")]
        public Task<List<Meal>> GetMeals(int mealId = default, int taskItemId = default)
        {
            if (taskItemId == default)
                return _mealService.GetMeals(mealId);

            return Task.Run(async () =>
            {
                var taskItem = await _taskService.GetTaskItem(taskItemId);
                taskItem.Status = TaskStatuses.Done;
                taskItem.Result = JsonConvert.SerializeObject(new ResultViewModel
                { Validate = 1, Message = $"Meals retrieved from server. Request from device: {taskItem.DeviceId}" });
                var unused = _taskService.UpdateTaskStatus(taskItem);

                if (taskItem.Data is null)
                    return await _mealService.GetMeals();

                var mealIds = JsonConvert.DeserializeObject<List<int>>(taskItem.Data);

                if (mealIds is null || mealIds.Count <= 0)
                    return await _mealService.GetMeals();

                var meals = new List<Meal>();
                foreach (var id in mealIds)
                    meals.Add(_mealService.GetMeals(id).Result.FirstOrDefault());

                return meals;
            });
        }

        [HttpGet]
        [Route("GetMealTimings")]
        public Task<List<MealTiming>> GetMealTimings(int mealTimingId = default, int taskItemId = default)
        {
            if (taskItemId == default)
                return _mealService.GetMealTimings(mealTimingId);

            return Task.Run(async () =>
            {
                var taskItem = await _taskService.GetTaskItem(taskItemId);
                taskItem.Status = TaskStatuses.Done;
                taskItem.Result = JsonConvert.SerializeObject(new ResultViewModel
                { Validate = 1, Message = $"Meal timings retrieved from server. Request from device: {taskItem.DeviceId}" });
                var unused = _taskService.UpdateTaskStatus(taskItem);

                if (taskItem.Data is null)
                    return await _mealService.GetMealTimings();

                var mealTimingIds = JsonConvert.DeserializeObject<List<int>>(taskItem.Data);

                if (mealTimingIds is null || mealTimingIds.Count <= 0)
                    return await _mealService.GetMealTimings();

                var meals = new List<MealTiming>();
                foreach (var id in mealTimingIds)
                {
                    var mealTiming = _mealService.GetMealTimings(id).Result.FirstOrDefault();
                    if (mealTiming != null)
                        meals.Add(mealTiming);
                }

                return meals;
            });
        }

        [HttpGet]
        [Route("GetMealTimingsByMealId")]
        public Task<List<MealTiming>> GetMealTimingsByMealId(int mealId)
        {
            return _mealService.GetMealTimingsByMealId(mealId);
        }

        [HttpPost]
        [Route("AddMeals")]
        public Task<List<ResultViewModel>> AddMeals(List<Meal> meals)
        {
            return _mealService.ModifyMeals(meals);
        }

        [HttpPost]
        [Route("SendMealsToDevice")]
        public Task<List<ResultViewModel>> SendMealsToDevice([FromBody]List<int> mealIds, int deviceId = default)
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
                            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Meal/SendMealsToDevice", Method.POST);
                        if (deviceId != default)
                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                        if (mealIds != null)
                            restRequest.AddJsonBody(mealIds);

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

        [HttpPost]
        [Route("DeleteMealsFromDevice")]
        public Task<List<ResultViewModel>> DeleteMealsFromDevice([FromBody]List<int> mealIds, int deviceId = default)
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
                            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Meal/DeleteMealsFromDevice", Method.POST);
                        if (deviceId != default)
                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                        if (mealIds != null)
                            restRequest.AddJsonBody(mealIds);

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

        [HttpPost]
        [Route("SendMealTimingsToDevice")]
        public Task<List<ResultViewModel>> SendMealTimingsToDevice([FromBody]List<int> mealTimingIds, int deviceId = default)
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
                            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Meal/SendMealTimingsToDevice", Method.POST);
                        if (deviceId != default)
                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                        if (mealTimingIds != null)
                            restRequest.AddJsonBody(mealTimingIds);

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

        [HttpPost]
        [Route("DeleteMealTimingsFromDevice")]
        public Task<List<ResultViewModel>> DeleteMealTimingsFromDevice([FromBody]List<int> mealTimingIds, int deviceId = default)
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
                            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Meal/DeleteMealTimingsFromDevice", Method.POST);
                        if (deviceId != default)
                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
                        if (mealTimingIds != null)
                            restRequest.AddJsonBody(mealTimingIds);

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
