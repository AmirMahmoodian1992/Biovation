﻿//using Biovation.CommonClasses.Manager;
//using Biovation.CommonClasses.Models;
//using Biovation.CommonClasses.Models.ConstantValues;
//using Biovation.CommonClasses.Models.RestaurantModels;
//using Biovation.CommonClasses.Service;
//using Biovation.CommonClasses.Service.RestaurantServices;
//using Newtonsoft.Json;
//using RestSharp;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;
//using System.Web.Http;

//namespace Biovation.Server.Controllers.v1.Restaurant
//{
//[Route("biovation/api/v{version:apiVersion}/[controller]")]
//[ApiVersion("1.0")]
//public class ReservationController : ControllerBase
//{
//    private readonly ReservationService _reservationService;
//    private readonly DeviceService _deviceService;
//    private readonly TaskStatuses _taskStatuses;
//    private readonly TaskService _taskService;

//    private readonly RestClient _restClient;

//    public ReservationController(ReservationService reservationService, DeviceService deviceService, TaskService taskService, TaskStatuses taskStatuses)
//    {
//        _reservationService = reservationService;
//        _deviceService = deviceService;
//        _taskService = taskService;
//        _taskStatuses = taskStatuses;
//        _restClient = new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/");
//    }

//        [HttpGet]
//        public Task<List<Reservation>> GetReservations(int reservationId = default, int taskItemId = default)
//        {
//            if (taskItemId == default)
//                return _reservationService.GetReservations(reservationId);

//return Task.Run(async () =>
//{
//var taskItem = await _taskService.GetTaskItem(taskItemId);
//taskItem.Status = _taskStatuses.Done;
//taskItem.Result = JsonConvert.SerializeObject(new ResultViewModel
//{ Validate = 1, Message = $"Foods retrieved from server. Request from device: {taskItem.DeviceId}" });
//var unused = _taskService.UpdateTaskStatus(taskItem);

//                if (taskItem.Data is null)
//                    return await _reservationService.GetReservations(deviceId: taskItem.DeviceId);

//                var reservationIds = JsonConvert.DeserializeObject<List<int>>(taskItem.Data);

//                if (reservationIds is null || reservationIds.Count <= 0)
//                    return await _reservationService.GetReservations(deviceId: taskItem.DeviceId);

//                var reservations = new List<Reservation>();
//                foreach (var id in reservationIds)
//                    reservations.Add(_reservationService.GetReservations(id, taskItem.DeviceId).Result.FirstOrDefault());

//                return reservations;
//            });
//        }

//        [HttpPost]
//        public Task<ResultViewModel> AddReservations(List<Reservation> reservations)
//        {
//            return _reservationService.AddReservations(reservations);
//        }

//        [HttpPost]
//        public Task<ResultViewModel> ModifyReservation(Reservation reservation)
//        {
//            return _reservationService.ModifyReservation(reservation);
//        }

//        [HttpPost]
//        public Task<List<ResultViewModel>> SendReservationsToDevice([FromBody]List<int> reservationIds, int deviceId = default)
//        {
//            return Task.Run(() =>
//            {
//                var results = new List<ResultViewModel>();
//                var deviceBrands = _deviceService.GetDeviceBrands();

//                var tasks = new List<Task>();
//                foreach (var deviceBrand in deviceBrands)
//                {
//                    tasks.Add(Task.Run(async () =>
//                    {
//                        var restRequest =
//                            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Reservation/SendReservationsToDevice", Method.POST);
//                        if (deviceId != default)
//                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
//                        if (reservationIds != null)
//                            restRequest.AddJsonBody(reservationIds);

//                        var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
//                        lock (results)
//                        {
//                            if (result.StatusCode == HttpStatusCode.OK && result.Data != null)
//                                results.Add(new ResultViewModel
//                                {
//                                    Id = result.Data.Id,
//                                    Code = result.Data.Code,
//                                    Validate = result.IsSuccessful && result.Data.Validate == 1 ? 1 : 0,
//                                    Message = deviceBrand.Name
//                                });
//                        }
//                    }));
//                }

//                Task.WaitAll(tasks.ToArray());
//                return results;
//            });
//        }

//        [HttpPost]
//        public Task<List<ResultViewModel>> DeleteReservationsFromDevice([FromBody]List<int> reservationIds, int deviceId = default)
//        {
//            return Task.Run(() =>
//            {
//                var results = new List<ResultViewModel>();
//                var deviceBrands = _deviceService.GetDeviceBrands();

//                var tasks = new List<Task>();
//                foreach (var deviceBrand in deviceBrands)
//                {
//                    tasks.Add(Task.Run(async () =>
//                    {
//                        var restRequest =
//                            new RestRequest($"{deviceBrand.Name}/{deviceBrand.Name}Reservation/DeleteReservationsFromDevice", Method.POST);
//                        if (deviceId != default)
//                            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
//                        if (reservationIds != null)
//                            restRequest.AddJsonBody(reservationIds);

//                        var result = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
//                        lock (results)
//                        {
//                            if (result.StatusCode == HttpStatusCode.OK && result.Data != null)
//                                results.Add(new ResultViewModel
//                                {
//                                    Id = result.Data.Id,
//                                    Code = result.Data.Code,
//                                    Validate = result.IsSuccessful && result.Data.Validate == 1 ? 1 : 0,
//                                    Message = deviceBrand.Name
//                                });
//                        }
//                    }));
//                }

//                Task.WaitAll(tasks.ToArray());
//                return results;
//            });
//        }
//    }
//}
