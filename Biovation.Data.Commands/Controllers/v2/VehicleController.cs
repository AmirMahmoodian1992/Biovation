using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class VehicleController : ControllerBase
    {
        private readonly VehicleRepository _vehicleRepository;

        public VehicleController(VehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        [HttpPost]
        [Authorize]
        public ResultViewModel InsertVehicle(Vehicle vehicle)
        {
            return _vehicleRepository.InsertVehicle(vehicle);
        }

        [HttpPut]
        [Authorize]
        public ResultViewModel ModifyVehicle(Vehicle vehicle)
        {
            return _vehicleRepository.ModifyVehicle(vehicle);
        }
    }
}