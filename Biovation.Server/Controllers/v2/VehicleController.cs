using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly VehicleService _vehicleService;

        public VehicleController(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ResultViewModel<PagingResult<Vehicle>>> GetVehicle(int vehicleId)
        {
            var token = HttpContext.Items["Token"] as string;
            return await _vehicleService.GetVehicle(vehicleId, token);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResultViewModel> InsertVehicle([FromBody] Vehicle vehicle)
        {
            var token = HttpContext.Items["Token"] as string;
            return await _vehicleService.InsertVehicle(vehicle,token);
        }

        [HttpPut]
        [Authorize]
        public async Task<ResultViewModel> ModifyVehicle([FromBody]Vehicle vehicle)
        {
            var token = HttpContext.Items["Token"] as string;
            return await _vehicleService.InsertVehicle(vehicle,token);
        }
    }
}