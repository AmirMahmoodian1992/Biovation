using Biovation.Domain;
using Biovation.Domain.RelayControllerModels;
using Biovation.Repository.Sql.v2.RelayController;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Controllers.v2.RelayController
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class EntranceController : ControllerBase
    {
        private readonly EntranceRepository _entranceRepository;

        public EntranceController(EntranceRepository entranceRepository)
        {
            _entranceRepository = entranceRepository;
        }

        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddRelay([FromBody] Entrance entrance = default)
        {
            return Task.Run(() => _entranceRepository.CreateEntrance(entrance));
        }

        [HttpPost]
        [Route("Relay")]
        [Authorize]
        public Task<ResultViewModel> UpdateRelay([FromBody] Entrance entrance = default)
        {
            return Task.Run(() => _entranceRepository.UpdateEntrance(entrance));
        }
        [HttpDelete]
        [Authorize]
        public Task<ResultViewModel> UpdateRelay(int id = default)
        {
            return Task.Run(() => _entranceRepository.DeleteEntrance(id));
        }

    }
}