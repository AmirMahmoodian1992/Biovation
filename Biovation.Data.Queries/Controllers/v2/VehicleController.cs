using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
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

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<Vehicle>>> GetVehicle(int vehicleId)
        {
            return Task.Run(() => _vehicleRepository.GetVehicle(vehicleId));
        }
    }
}