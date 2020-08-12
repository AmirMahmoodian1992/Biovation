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
    public class ReservationController : Controller
    {
        private readonly ReservationService _reservationService;
        private readonly DeviceService _deviceService;
        private readonly TaskService _taskService;

        private readonly RestClient _restClient;

        public ReservationController(ReservationService reservationService, DeviceService deviceService, TaskService taskService)
        {
            _reservationService = reservationService;
            _deviceService = deviceService;
            _taskService = taskService;
            _restClient = new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/Biovation/Api/");
        }

        [HttpGet]
        [Route("{id}")]
        public Task<IActionResult> GetReservations(int id = default, int taskItemId = default)
        {
            throw null;
        }

        [HttpPost]
        public Task<IActionResult> AddReservations([FromBody]List<Reservation> reservations)
        {
            throw null;
        }

        [HttpPut]
        public Task<IActionResult> ModifyReservation([FromBody]Reservation reservation)
        {
            throw null;
        }

        [HttpPut]
        [Route("ReservationsToDevice/{deviceId}")]
        public Task<IActionResult> SendReservationsToDevice([FromBody]List<int> reservationIds, int deviceId = default)
        {
            throw null;
        }

        [HttpDelete]
        [Route("{id}")]
        public Task<IActionResult> DeleteReservationsFromDevice(int id, int deviceId = default)
        {
            throw null;
        }
    }
}
