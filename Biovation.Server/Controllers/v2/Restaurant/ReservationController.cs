//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Biovation.CommonClasses.Manager;
//using Biovation.Domain.RestaurantModels;
//using Biovation.Service;
//using Biovation.Service.RestaurantServices;
//using Biovation.Service.SQL.v1;
//using Microsoft.AspNetCore.Mvc;
//using RestSharp;

//namespace Biovation.Server.Controllers.v2.Restaurant
//{
//    [Route("biovation/api/v{version:apiVersion}/[controller]")]
//    [ApiVersion("2.0")]
//    public class ReservationController : Controller
//    {
//        private readonly ReservationService _reservationService;
//        private readonly DeviceService _deviceService;
//        private readonly TaskService _taskService;

//        private readonly RestClient _restClient;

//        public ReservationController(ReservationService reservationService, DeviceService deviceService, TaskService taskService)
//        {
//            _reservationService = reservationService;
//            _deviceService = deviceService;
//            _taskService = taskService;
//            _restClient = new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/");
//        }

//        [HttpGet]
//        [Route("{id}")]
//        public Task<IActionResult> GetReservations(int id = default, int taskItemId = default)
//        {
//            throw null;
//        }

//        [HttpPost]
//        public Task<IActionResult> AddReservations([FromBody]List<Reservation> reservations)
//        {
//            throw null;
//        }

//        [HttpPut]
//        public Task<IActionResult> ModifyReservation([FromBody]Reservation reservation)
//        {
//            throw null;
//        }

//        [HttpDelete]
//        [Route("{id}/{deviceId}")]
//        public Task<IActionResult> DeleteReservationsFromDevice(int id, int deviceId = default)
//        {
//            throw null;
//        }

//        [HttpPut]
//        [Route("ReservationsToDevice/{deviceId}")]
//        public Task<IActionResult> SendReservationsToDevice([FromBody]List<int> reservationIds, int deviceId = default)
//        {
//            throw null;
//        }

//        //batch delete
//        [HttpPost]
//        [Route("DeleteReservationsFromDevice/{deviceId}")]
//        public Task<IActionResult> DeleteReservationsFromDevice([FromBody]List<int> ids, int deviceId = default)
//        {
//            throw null;
//        }
//    }
//}
